using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Shisen_Sho {
	static class Program {

		public static PixelFormat BufferFormat { get { return PixelFormat.Format16bppRgb565; } }

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[MTAThread]
		static void Main() {
			Application.Run(new Playfield());
		}
	}
}