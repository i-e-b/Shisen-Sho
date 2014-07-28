namespace Shisen_Sho {
	partial class NewGameOptions {
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
			this.label1 = new System.Windows.Forms.Label();
			this.difficultyBar = new System.Windows.Forms.TrackBar();
			this.label2 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
			this.label1.Location = new System.Drawing.Point(4, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 20);
			this.label1.Text = "Options";
			// 
			// difficultyBar
			// 
			this.difficultyBar.LargeChange = 16;
			this.difficultyBar.Location = new System.Drawing.Point(4, 52);
			this.difficultyBar.Maximum = 42;
			this.difficultyBar.Minimum = 10;
			this.difficultyBar.Name = "difficultyBar";
			this.difficultyBar.Size = new System.Drawing.Size(233, 39);
			this.difficultyBar.SmallChange = 4;
			this.difficultyBar.TabIndex = 1;
			this.difficultyBar.TickFrequency = 4;
			this.difficultyBar.Value = 26;
			this.difficultyBar.ValueChanged += new System.EventHandler(this.difficultyBar_ValueChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(4, 30);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 19);
			this.label2.Text = "Difficulty";
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(3, 126);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(233, 20);
			this.checkBox1.TabIndex = 3;
			this.checkBox1.Text = "Guarantee Solution";
			this.checkBox1.CheckStateChanged += new System.EventHandler(this.checkBox1_CheckStateChanged);
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Italic);
			this.label3.Location = new System.Drawing.Point(26, 149);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(210, 52);
			this.label3.Text = "If selected, new games will be created by constructing a game in reverse. This ma" +
				"y take some time on slower devices.";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(4, 84);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(42, 20);
			this.label4.Text = "Easy";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(194, 84);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(42, 20);
			this.label5.Text = "Hard";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// NewGameOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(240, 294);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.difficultyBar);
			this.Controls.Add(this.label1);
			this.MinimizeBox = false;
			this.Name = "NewGameOptions";
			this.Text = "Shisen-Sho";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.NewGameOptions_Closing);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TrackBar difficultyBar;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
	}
}