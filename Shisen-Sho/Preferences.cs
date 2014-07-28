using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Shisen_Sho {

	class Preferences {
		protected int difficulty;
		protected bool sure_game;

		public int Difficulty {
			get { return difficulty; }
			set {
				if (value < 10) difficulty = 10;
				else if (value > 42) difficulty = 42;
				else difficulty = value;
			}
		}

		public bool GuaranteeGame { get { return sure_game; } set { sure_game = value; } }

		public Preferences() {
			difficulty = 26;
			sure_game = false;
		}

		public static string SettingFilePath() {
			return "";
		}

		public static Preferences Load() {
			return new Preferences();
		}

		public void Save() {
			
		}
	}
}
