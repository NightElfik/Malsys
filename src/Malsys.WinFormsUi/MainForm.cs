using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Malsys.Compilers;
using Malsys.Evaluators;
using Malsys.Processing;
using Malsys.Processing.Output;
using Malsys.Reflection;
using Malsys.Resources;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.WinFormsUi {
	public partial class MainForm : Form {


		private string stdLibStr;
		private InputBlockEvaled stdLib;
		private KnownConstOpProvider knownStuffProvider;
		private ComponentResolver componentResolver;
		private IExpressionEvaluatorContext globalEec;
		private ICompilersContainer compilersContainer;
		private IEvaluatorsContainer evaluatorsContainer;


		public MainForm() {
			InitializeComponent();

			initializeMalsys();


			tsslStatus.Text = "Working directory set to \"" + Properties.Settings.Default.WorkingDirectory + "\"";
		}

		private void initializeMalsys() {

			// load std lib
			using (var stream = new ResourcesReader().GetResourceStream(ResourcesHelper.StdLibResourceName)) {
				using (TextReader reader = new StreamReader(stream)) {
					stdLibStr = reader.ReadToEnd();
				}
			}

			knownStuffProvider = new KnownConstOpProvider();
			componentResolver = new ComponentResolver();
			globalEec = new ExpressionEvaluatorContext();

			var logger = new MessageLogger();
			var loader = new MalsysLoader();

			loader.LoadMalsysStuffFromAssembly(Assembly.GetAssembly(typeof(MalsysLoader)),
				knownStuffProvider, knownStuffProvider, ref globalEec, componentResolver, logger);

			compilersContainer = new CompilersContainer(knownStuffProvider, knownStuffProvider);
			evaluatorsContainer = new EvaluatorsContainer(globalEec);

			var inCompiled = compilersContainer.CompileInput(stdLibStr, ResourcesHelper.StdLibResourceName, logger);
			stdLib = evaluatorsContainer.EvaluateInput(inCompiled, logger);

			loader.LoadMalsysStuffFromAssembly(Assembly.LoadFile(Path.GetFullPath(@".\ExamplePlugin.dll")),
					knownStuffProvider, knownStuffProvider, ref globalEec, componentResolver, logger);
			loader.LoadMalsysStuffFromAssembly(Assembly.LoadFile(Path.GetFullPath(@".\Malsys.BitmapRenderers.dll")),
					knownStuffProvider, knownStuffProvider, ref globalEec, componentResolver, logger);


		}

		private void tsmiProcess_Click(object sender, EventArgs e) {


			Directory.CreateDirectory(Properties.Settings.Default.WorkingDirectory);

			string backupDir = folderBrowserDialog.SelectedPath + @"\Backup\";
			Directory.CreateDirectory(backupDir);

			File.WriteAllText(backupDir + DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss"),
				tbSourceCode.Text);


			var logger = new MessageLogger();
			var fileMgr = new FileOutputProvider(Properties.Settings.Default.WorkingDirectory);

			var sw = new Stopwatch();
			sw.Start();
			bool result = process(tbSourceCode.Text, fileMgr, logger);
			sw.Stop();

			fileMgr.CloseAllOutputStreams();

			tsslStatus.Text = string.Format("{0} files produced in {1} seconds.", fileMgr.OutputsCount, sw.Elapsed.TotalSeconds);
			tbMessages.Text = logger.AllMessagesToFullString();

			if (!result) {
				MessageBox.Show(this, logger.AllMessagesToFullString());
			}

		}

		private void tsmiSetOutput_Click(object sender, EventArgs e) {

			if (folderBrowserDialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK) {
				return;
			}

			Properties.Settings.Default.WorkingDirectory = folderBrowserDialog.SelectedPath;
			Properties.Settings.Default.Save();

			tsslStatus.Text = "Working directory set to \"" + Properties.Settings.Default.WorkingDirectory + "\"";


		}


		private bool process(string sourceCode, IOutputProvider outProvider, IMessageLogger logger) {


			InputBlockEvaled evaledInput;

			var processManager = new ProcessManager(compilersContainer, evaluatorsContainer, componentResolver);



			evaledInput = processManager.CompileAndEvaluateInput(sourceCode, "input", logger);

			if (logger.ErrorOccurred) {
				return false;
			}


			evaledInput.Append(stdLib);

			if (evaledInput.ProcessStatements.Count > 0) {
				processManager.ProcessInput(evaledInput, outProvider, logger, TimeSpan.MaxValue);
			}

			if (logger.ErrorOccurred) {
				return false;
			}

			return true;
		}

		private void tsmiPaste_Click(object sender, EventArgs e) {
			tbSourceCode.Text = Clipboard.GetText();
		}
	}
}
