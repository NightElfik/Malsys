using System;
using System.IO;
using Malsys;

namespace LsystemSyntaxHighlighter {
	class Program {
		static void Main(string[] args) {
			if (args.Length != 2) {
				Console.Error.WriteLine("Excpected 2 args, in and out file.");
				return;
			}

			toHtml(args[0], args[1]);
		}


		private static void toHtml(string filePath, string outPath) {

			using (Stream outStream = File.Open(outPath, FileMode.Create, FileAccess.Write)) {
				using (StreamWriter writer = new StreamWriter(outStream)) {
					writer.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
					writer.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
					writer.WriteLine();
					writer.WriteLine("<head>");
					writer.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />".Fmt(filePath));
					writer.WriteLine("<title>Highlighted L-system from `{0}`</title>".Fmt(filePath));
					writer.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"lsystem.css\" />");
					writer.WriteLine("</head>");
					writer.WriteLine();
					writer.WriteLine("<body>");
					writer.WriteLine("<ol class=\"lsrc\">");

					var result = HtmlHighlighter.HighlightFromString(File.ReadAllText(filePath), filePath);
					writer.WriteLine(result);

					writer.WriteLine("</ol>");
					writer.WriteLine();
					writer.WriteLine("</body>");
					writer.WriteLine("</html>");
				}
			}
		}
	}
}
