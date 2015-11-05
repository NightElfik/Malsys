namespace Malsys.WinFormsUi {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.tbSourceCode = new System.Windows.Forms.TextBox();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.tsmiProcess = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiSetOutput = new System.Windows.Forms.ToolStripMenuItem();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.tbMessages = new System.Windows.Forms.TextBox();
			this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiAuthor = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiWeb = new System.Windows.Forms.ToolStripMenuItem();
			this.tsEmail = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiGithub = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbSourceCode
			// 
			this.tbSourceCode.AcceptsReturn = true;
			this.tbSourceCode.AcceptsTab = true;
			this.tbSourceCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbSourceCode.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.tbSourceCode.Location = new System.Drawing.Point(0, 24);
			this.tbSourceCode.Multiline = true;
			this.tbSourceCode.Name = "tbSourceCode";
			this.tbSourceCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbSourceCode.Size = new System.Drawing.Size(602, 307);
			this.tbSourceCode.TabIndex = 1;
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiProcess,
            this.tsmiPaste,
            this.tsmiSetOutput,
            this.tsmiAbout});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(602, 24);
			this.menuStrip.TabIndex = 2;
			this.menuStrip.Text = "menuStrip1";
			// 
			// tsmiProcess
			// 
			this.tsmiProcess.Name = "tsmiProcess";
			this.tsmiProcess.Size = new System.Drawing.Size(59, 20);
			this.tsmiProcess.Text = "Process";
			this.tsmiProcess.Click += new System.EventHandler(this.tsmiProcess_Click);
			// 
			// tsmiPaste
			// 
			this.tsmiPaste.Name = "tsmiPaste";
			this.tsmiPaste.Size = new System.Drawing.Size(47, 20);
			this.tsmiPaste.Text = "Paste";
			this.tsmiPaste.Click += new System.EventHandler(this.tsmiPaste_Click);
			// 
			// tsmiSetOutput
			// 
			this.tsmiSetOutput.Name = "tsmiSetOutput";
			this.tsmiSetOutput.Size = new System.Drawing.Size(134, 20);
			this.tsmiSetOutput.Text = "Set working derectory";
			this.tsmiSetOutput.Click += new System.EventHandler(this.tsmiSetOutput_Click);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus});
			this.statusStrip.Location = new System.Drawing.Point(0, 402);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(602, 22);
			this.statusStrip.TabIndex = 3;
			this.statusStrip.Text = "statusStrip1";
			// 
			// tsslStatus
			// 
			this.tsslStatus.Name = "tsslStatus";
			this.tsslStatus.Size = new System.Drawing.Size(39, 17);
			this.tsslStatus.Text = "Status";
			// 
			// tbMessages
			// 
			this.tbMessages.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tbMessages.Location = new System.Drawing.Point(0, 331);
			this.tbMessages.Multiline = true;
			this.tbMessages.Name = "tbMessages";
			this.tbMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbMessages.Size = new System.Drawing.Size(602, 71);
			this.tbMessages.TabIndex = 4;
			// 
			// tsmiAbout
			// 
			this.tsmiAbout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAuthor,
            this.tsmiWeb,
            this.tsEmail,
            this.tsmiGithub});
			this.tsmiAbout.Name = "tsmiAbout";
			this.tsmiAbout.Size = new System.Drawing.Size(52, 20);
			this.tsmiAbout.Text = "About";
			// 
			// tsmiAuthor
			// 
			this.tsmiAuthor.Name = "tsmiAuthor";
			this.tsmiAuthor.Size = new System.Drawing.Size(274, 22);
			this.tsmiAuthor.Text = "By Marek Fišer";
			this.tsmiAuthor.Click += new System.EventHandler(this.tsmiAuthor_Click);
			// 
			// tsmiWeb
			// 
			this.tsmiWeb.Name = "tsmiWeb";
			this.tsmiWeb.Size = new System.Drawing.Size(274, 22);
			this.tsmiWeb.Text = "http://malsys.cz";
			this.tsmiWeb.Click += new System.EventHandler(this.tsmiWeb_Click);
			// 
			// tsEmail
			// 
			this.tsEmail.Name = "tsEmail";
			this.tsEmail.Size = new System.Drawing.Size(274, 22);
			this.tsEmail.Text = "malsys@marekfiser.cz";
			this.tsEmail.Click += new System.EventHandler(this.tsEmail_Click);
			// 
			// tsmiGithub
			// 
			this.tsmiGithub.Name = "tsmiGithub";
			this.tsmiGithub.Size = new System.Drawing.Size(274, 22);
			this.tsmiGithub.Text = "https://github.com/NightElfik/Malsys";
			this.tsmiGithub.Click += new System.EventHandler(this.tsmiGithub_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(602, 424);
			this.Controls.Add(this.tbSourceCode);
			this.Controls.Add(this.tbMessages);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.menuStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MainForm";
			this.Text = "Malsys UI";
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbSourceCode;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem tsmiProcess;
		private System.Windows.Forms.ToolStripMenuItem tsmiSetOutput;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
		private System.Windows.Forms.ToolStripMenuItem tsmiPaste;
		private System.Windows.Forms.TextBox tbMessages;
		private System.Windows.Forms.ToolStripMenuItem tsmiAbout;
		private System.Windows.Forms.ToolStripMenuItem tsmiAuthor;
		private System.Windows.Forms.ToolStripMenuItem tsmiWeb;
		private System.Windows.Forms.ToolStripMenuItem tsEmail;
		private System.Windows.Forms.ToolStripMenuItem tsmiGithub;
	}
}

