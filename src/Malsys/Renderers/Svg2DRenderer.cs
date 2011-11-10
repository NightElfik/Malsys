using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Media;

namespace Malsys.Renderers {
	public class Svg2DRenderer : IBasic2DRenderer {

		[UserSettable]
		public string FileHeader { get; set; }

		[UserSettable]
		public string SvgHeader { get; set; }

		[UserSettable]
		public string SvgFooter { get; set; }


		public Svg2DRenderer() {
			FileHeader = "<?xml version=\"1.0\" standalone=\"no\"?>\n"+
				"<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">";
			SvgHeader = "<svg viewBox=\"{0} {1} {2} {3}\" width=\"{4}\" height=\"{5}\" xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">";
			SvgFooter = "</svg>";
		}

		#region IRenderer Members

		public void Initialize() {

		}

		public void EndRendering() {

		}

		public string GetResultsFilePaths() {
			throw new NotImplementedException();
		}

		#endregion

		#region IBasic2DRenderer Members

		public void MoveTo(PointF Point, float Width, ColorF Color) {
			throw new NotImplementedException();
		}

		public void LineTo(PointF Point, float Width, ColorF Color) {
			throw new NotImplementedException();
		}

		public void DrawPolygon(IEnumerable<PointF> Points, ColorF Color) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
