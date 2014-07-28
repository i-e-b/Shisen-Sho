namespace Shisen_Sho {
	partial class Playfield {
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
			this.quitButton = new System.Windows.Forms.Button();
			this.newGameButton = new System.Windows.Forms.Button();
			this.optionsButton = new System.Windows.Forms.Button();
			this.hintButton = new System.Windows.Forms.Button();
			this.refreshTimer = new System.Windows.Forms.Timer();
			this.SuspendLayout();
			// 
			// quitButton
			// 
			this.quitButton.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Bold);
			this.quitButton.Location = new System.Drawing.Point(225, 0);
			this.quitButton.Name = "quitButton";
			this.quitButton.Size = new System.Drawing.Size(15, 15);
			this.quitButton.TabIndex = 0;
			this.quitButton.Text = "X";
			this.quitButton.Click += new System.EventHandler(this.quitButton_Click);
			// 
			// newGameButton
			// 
			this.newGameButton.Font = new System.Drawing.Font("Courier New", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.newGameButton.Location = new System.Drawing.Point(21, 0);
			this.newGameButton.Name = "newGameButton";
			this.newGameButton.Size = new System.Drawing.Size(15, 15);
			this.newGameButton.TabIndex = 1;
			this.newGameButton.Text = "!";
			this.newGameButton.Click += new System.EventHandler(this.newGameButton_Click);
			// 
			// optionsButton
			// 
			this.optionsButton.Font = new System.Drawing.Font("Courier New", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
			this.optionsButton.Location = new System.Drawing.Point(0, 0);
			this.optionsButton.Name = "optionsButton";
			this.optionsButton.Size = new System.Drawing.Size(15, 15);
			this.optionsButton.TabIndex = 2;
			this.optionsButton.Text = "?";
			this.optionsButton.Click += new System.EventHandler(this.optionsButton_Click);
			// 
			// hintButton
			// 
			this.hintButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.hintButton.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Bold);
			this.hintButton.Location = new System.Drawing.Point(225, 305);
			this.hintButton.Name = "hintButton";
			this.hintButton.Size = new System.Drawing.Size(15, 15);
			this.hintButton.TabIndex = 3;
			this.hintButton.Text = "H";
			this.hintButton.Click += new System.EventHandler(this.hintButton_Click);
			// 
			// refreshTimer
			// 
			this.refreshTimer.Enabled = true;
			this.refreshTimer.Interval = 30000;
			this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
			// 
			// Playfield
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.BackColor = System.Drawing.Color.DarkOliveGreen;
			this.ClientSize = new System.Drawing.Size(240, 320);
			this.ControlBox = false;
			this.Controls.Add(this.hintButton);
			this.Controls.Add(this.optionsButton);
			this.Controls.Add(this.newGameButton);
			this.Controls.Add(this.quitButton);
			this.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Regular);
			this.ForeColor = System.Drawing.Color.Red;
			this.Location = new System.Drawing.Point(0, 0);
			this.MinimizeBox = false;
			this.Name = "Playfield";
			this.Text = "Shisen-Sho";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Playfield_Paint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Playfield_MouseDown);
			this.Load += new System.EventHandler(this.Playfield_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button quitButton;
		private System.Windows.Forms.Button newGameButton;
		private System.Windows.Forms.Button optionsButton;
		private System.Windows.Forms.Button hintButton;
		private System.Windows.Forms.Timer refreshTimer;
	}
}

