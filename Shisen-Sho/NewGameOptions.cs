using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Shisen_Sho {
	partial class NewGameOptions : Form {
		Preferences prefs;

		public NewGameOptions(Preferences ApplicationPreferences) {
			InitializeComponent();
			prefs = ApplicationPreferences;
			difficultyBar.Value = prefs.Difficulty;
			checkBox1.CheckState = (prefs.GuaranteeGame) ? (CheckState.Checked) : (CheckState.Unchecked);
		}

		private void NewGameOptions_Closing(object sender, CancelEventArgs e) {
			prefs.Save();
		}

		private void difficultyBar_ValueChanged(object sender, EventArgs e) {
			prefs.Difficulty = difficultyBar.Value;
		}

		private void checkBox1_CheckStateChanged(object sender, EventArgs e) {
			prefs.GuaranteeGame = (checkBox1.CheckState == CheckState.Checked);
		}
	}
}