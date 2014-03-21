using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using Malsys.BitmapRenderers.Png;
using Malsys.BitmapRenderers.Png.Chunks;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.SemanticModel;
using Microsoft.FSharp.Collections;
using System;

namespace Malsys.BitmapRenderers.Components {
	/// <summary>
	/// Crates animation from PNG images.
	/// </summary>
	/// <name>Png animation renderer</name>
	/// <group>Renderers</group>
	public class PngAnimationRenderer : GdiBitmapRenderer {

		private List<Stream> frames = new List<Stream>();

		#region User settable properties

		/// <summary>
		/// Delay between frames in seconds.
		/// </summary>
		/// <expected>Positive number.</expected>
		/// <default>0.5</default>
		[AccessName("frameDelay")]
		[UserSettable]
		public Constant FrameDelay { get; set; }

		#endregion

		public override void Reset() {
			base.Reset();
			FrameDelay = new Constant(0.5);
		}

		public override void Cleanup() {
			createAnimation();

			base.Cleanup();
			frames.Clear();
		}

		public override void Initialize(ProcessContext context) {
			OutputBitmapType = Constant.Zero;  // PNG
			base.Initialize(context);
		}

		protected override Stream createStream(FSharpMap<string, object> metadata) {
			var stream = context.OutputProvider.GetOutputStream<GdiBitmapRenderer>("Animation frame #" + frames.Count, outputMimeType, true, metadata);
			//int wid = metadata.TryGetValue(OutputMetadataKeyHelper.OutputWidth, 0);
			//int hei = metadata.TryGetValue(OutputMetadataKeyHelper.OutputHeight, 0);
			//var stream = new FileStream(@"I:\Malsys\src\Malsys.Web\Img\DevDiary\apng\Frames\AFrmae{0}-{1}x{2}.png".Fmt(frames.Count, wid, hei), FileMode.Create, FileAccess.Write);
			frames.Add(stream);
			return stream;
		}

		private void createAnimation() {

			int framesCount = frames.Count;

			if (framesCount == 0) {
				Logger.LogMessage(Message.NoFrames);
				return;
			}

			PngHelper.Initialize();

			var dataReaders = new PngReader.ChunkReader[framesCount];
			var framePositions = new System.Drawing.Rectangle[framesCount];
			ChunkIHDR referenceIhdr = null;
			uint animationWidth = 0;
			uint animationHeight = 0;
			double frameDuration = MathHelper.Clamp(FrameDelay.Value, 0.001, 100);
			double animationDuration = framesCount * frameDuration;

			for (int i = 0; i < framesCount; i++) {
				var frameStream = frames[i];
				frameStream.Seek(0, SeekOrigin.Begin);
				var pngReader = new PngReader(frameStream);
				pngReader.ReadPngHeader();
				PngReader.ChunkReader chunkReader;
				do {  // seek to IHDR chunk
					chunkReader = pngReader.ReadChunk();
				} while (chunkReader.Name != ChunkIHDR.Name);

				var ihdrChunk = chunkReader.ReadIHDR();

				var meta = context.OutputProvider.GetMetadata(frameStream);
				int offT = meta.TryGetValue(OutputMetadataKeyHelper.OffsetTop, 0);
				int offR = meta.TryGetValue(OutputMetadataKeyHelper.OffsetRight, 0);
				int offB = meta.TryGetValue(OutputMetadataKeyHelper.OffsetBottom, 0);
				int offL = meta.TryGetValue(OutputMetadataKeyHelper.OffsetLeft, 0);

				if (offT < 0 || offR < 0 || offB < 0 || offL < 0) {
					Logger.LogMessage(Message.NegativeOffset, i);
					return;
				}

				uint frameWidth = (uint)offL + ihdrChunk.Width + (uint)offR;
				uint frameHeight = (uint)offT + ihdrChunk.Height + (uint)offB;
				framePositions[i] = new System.Drawing.Rectangle(offL, offT, (int)ihdrChunk.Width, (int)ihdrChunk.Height);

				if (i == 0) {
					referenceIhdr = ihdrChunk;
					animationWidth = frameWidth;
					animationHeight = frameHeight;
					if (referenceIhdr.ColourType == 3) {  // we do not support indexed colors
						Logger.LogMessage(Message.PngFormatNotSupported, referenceIhdr.ColourType);
						return;
					}
				}
				else {
					if (frameWidth != animationWidth || frameHeight != animationHeight) {
						Logger.LogMessage(Message.InconsistentSize);
						return;
					}
					if (ihdrChunk.BitDepth != referenceIhdr.BitDepth || ihdrChunk.ColourType != referenceIhdr.ColourType
							|| ihdrChunk.CompressionMethod != referenceIhdr.CompressionMethod || ihdrChunk.FilterMethod != referenceIhdr.FilterMethod
							|| ihdrChunk.InterlaceMethod != referenceIhdr.InterlaceMethod) {
						Logger.LogMessage(Message.InconsistentFormat);
						return;
					}
				}

				do {  // seek to IDAT chunk
					chunkReader = pngReader.ReadChunk();
				} while (chunkReader.Name != PngHelper.IDAT);

				dataReaders[i] = chunkReader;
			}

			var metadata = MapModule.Empty<string, object>();
			metadata = metadata.Add(OutputMetadataKeyHelper.OutputWidth, (int)animationWidth);
			metadata = metadata.Add(OutputMetadataKeyHelper.OutputHeight, (int)animationHeight);
			metadata = metadata.Add(OutputMetadataKeyHelper.OutputInPngAnimation, true);
			var resultStream = context.OutputProvider.GetOutputStream<PngAnimationRenderer>("PNG animation", MimeType.Image.Png, false, metadata);

			referenceIhdr.Width = animationWidth;
			referenceIhdr.Height = animationHeight;

			var writer = new PngWriter(resultStream);
			writer.WritePngHeader();
			writer.WriteIHDR(referenceIhdr);
			writer.WriteTEXt(new ChunkTEXt(ChunkTEXt.PredefinedKeyword.Software, "Malsys"));
			writer.WriteAcTL(new ChunkAcTL() { FramesCount = (uint)framesCount, PlaysCount = 0 });

			uint sequenceNum = 0;
			var fctl = new ChunkFcTL() {
				DelayNumerator = (ushort)MathHelper.Clamp(frameDuration * 100, 1, ushort.MaxValue),
				DelayDenominator = 100,
				DisposeOperation = 1,
				BlendOperation = 0
			};

			// preview frame
			int middleFrame = framesCount / 2;
			using (var previewFrame = (Bitmap)Bitmap.FromStream(frames[middleFrame])) {
				using (var previewImage = createPrewievImage(animationWidth, animationHeight, previewFrame,
						framePositions[middleFrame].X, framePositions[middleFrame].Y, framesCount, animationDuration)) {
					writePreviewFrame(previewImage, writer, referenceIhdr);
				}
			}
			dataReaders[middleFrame].Reset();  // restore stream position


			// frames
			for (int i = 0; i < framesCount; i++) {
				fctl.SequenceNumber = sequenceNum++;
				fctl.Width = (uint)framePositions[i].Width;
				fctl.Height = (uint)framePositions[i].Height;
				fctl.OffsetX = (uint)framePositions[i].X;
				fctl.OffsetY = (uint)framePositions[i].Y;
				writer.WriteFcTL(fctl);

				var idatReader = dataReaders[i];
				// fdAT – Frame Data Chunk
				do {
					var chunkWriter = writer.StartChunk("fdAT", idatReader.Length + 4);
					chunkWriter.Write(sequenceNum++);
					chunkWriter.CopyFrom(idatReader);
					idatReader = idatReader.Next();
				} while (idatReader.Name == PngHelper.IDAT);
			}

			writer.WriteIEND();

		}

		private Bitmap createPrewievImage(uint width, uint height, Bitmap previewFrame, int offsetX, int offsetY, int framesCount, double durationInSec) {

			var preview = new Bitmap((int)width, (int)height, pixelFormat);

			using (var g = Graphics.FromImage(preview)) {
				g.SmoothingMode = SmoothingMode.HighQuality;

				g.Clear(Color.FromArgb((int)bgColor.ToArgb()));

				g.DrawImage(previewFrame, new Rectangle(offsetX, offsetY, previewFrame.Width, previewFrame.Height));

				string text = "This image is animated PNG\n{0} frames, duration {1} s\nUse supported viewer to see it correctly"
					.Fmt(framesCount, Math.Round(durationInSec, 1));
				Font font = SystemFonts.DefaultFont;
				var textSize = g.MeasureString(text, font);

				float textX = 10;
				float textY = height - textSize.Height - 10;

				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
				g.DrawString(text, font, Brushes.White, textX + 1, textY + 1);
				g.DrawString(text, font, Brushes.Black, textX, textY);
			}

			return preview;

		}

		private void writePreviewFrame(Bitmap previewImage, PngWriter writer, ChunkIHDR referenceIhdr) {

			using (var ms = new MemoryStream()) {
				previewImage.Save(ms, outputImageFormat);
				ms.Seek(0, SeekOrigin.Begin);
				var previewReader = new PngReader(ms);
				previewReader.ReadPngHeader();

				PngReader.ChunkReader previewChunkReader;
				do {  // seek to IHDR chunk
					previewChunkReader = previewReader.ReadChunk();
				} while (previewChunkReader.Name != ChunkIHDR.Name);

				var previewIhdrChunk = previewChunkReader.ReadIHDR();

				Debug.Assert(previewIhdrChunk.Width == referenceIhdr.Width && previewIhdrChunk.Height == referenceIhdr.Height
						&& previewIhdrChunk.BitDepth == referenceIhdr.BitDepth && previewIhdrChunk.ColourType == referenceIhdr.ColourType
						&& previewIhdrChunk.CompressionMethod == referenceIhdr.CompressionMethod && previewIhdrChunk.FilterMethod == referenceIhdr.FilterMethod
						&& previewIhdrChunk.InterlaceMethod == referenceIhdr.InterlaceMethod);

				do {  // seek to IDAT chunk
					previewChunkReader = previewReader.ReadChunk();
				} while (previewChunkReader.Name != PngHelper.IDAT);

				do {
					writer.StartChunk(PngHelper.IDAT, previewChunkReader.Length).CopyFrom(previewChunkReader);
					previewChunkReader = previewChunkReader.Next();
				} while (previewChunkReader.Name == PngHelper.IDAT);
			}
		}

		public enum Message {

			[Message(MessageType.Error, "All animation frames do not have the same size as the first frame.")]
			InconsistentSize,
			[Message(MessageType.Error, "All animation frames do not have the same attributes (color depth, filter method, etc.) as the first frame.")]
			InconsistentFormat,
			[Message(MessageType.Error, "Invalid (neagtive) offset of frame #{0} (zero-based).")]
			NegativeOffset,
			[Message(MessageType.Error, "PNG format `{0}` not supported.")]
			PngFormatNotSupported,

			[Message(MessageType.Warning, "No images for animation produced.")]
			NoFrames,

		}


	}
}
