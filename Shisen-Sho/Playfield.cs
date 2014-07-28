using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing.Imaging;

namespace Shisen_Sho {
	partial class Playfield : Form {
		protected ShoTiles tile_set;
		protected ShoGrid play_grid;
		protected Preferences prefs;

		public Playfield() {
			InitializeComponent();
			prefs = Preferences.Load();
		}

		/// <summary>
		/// Paint background and tileset. Handles double buffering
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaintBackground(PaintEventArgs e) {
			// TODO: re-draw only needed areas
			if (play_grid.SwapHilites(e.Graphics, new SolidBrush(BackColor))) return;

			try {
				GC.Collect();
				Image dblBuffer = new Bitmap(this.Width, this.Height, Program.BufferFormat);
				Graphics g = Graphics.FromImage(dblBuffer);

				g.FillRectangle(new SolidBrush(BackColor), Bounds);
				PaintEventArgs args = new PaintEventArgs(g, this.Bounds);
				//OnPaint(args);
				Playfield_Paint(null, args);
				g.Dispose();
				e.Graphics.DrawImage(dblBuffer, 0, 0);
				dblBuffer = null;
				GC.Collect();
			} catch (OutOfMemoryException ex) {
				// Try a non-buffered version. It will flicker, but needs a lot less memory.
				GC.Collect();
				base.OnPaintBackground(e);
				base.OnPaint(e);
				e.Graphics.DrawString(ex.Message, this.Font, new SolidBrush(Color.White), 0, 0);
			}
		}

		/// <summary>
		/// Override paint behaviour. All paint response is done in the OnPaintBackground() method.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e) {
			// Do nothing. All drawing done in OnPaintBackground()
		}

		private void Playfield_Paint(object sender, PaintEventArgs e) {
			Graphics g = e.Graphics;

			// Draw battery stats
			string bat_stat = (BatteryStatus.OnCharge()) ? (BatteryStatus.ACPowerStatus()) : (BatteryStatus.BatteryLifePercent());
			int h = (int)g.MeasureString(bat_stat, this.Font).Height;
			g.DrawString(bat_stat, this.Font, new SolidBrush(Color.White), 10, this.Height - (h + 3));
			
			if ((play_grid != null) && (!play_grid.BoardClear())) {
				play_grid.DrawGrid(g);
			} else {
				Font f = new Font("Tahoma", 10, FontStyle.Bold);
				g.DrawString("Game Complete!", f, new SolidBrush(Color.White), g.DpiX / 2, g.DpiY / 2);
			}
		}

		private void quitButton_Click(object sender, EventArgs e) {
			Close();
		}

		/// <summary>
		/// Load game media (images of tiles etc.)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Playfield_Load(object sender, EventArgs e) {
			tile_set = new Tilesets.TraditionalTiles();
			Graphics g = this.CreateGraphics();
			play_grid = new ShoGrid(11, 12, prefs.Difficulty, tile_set, this.Bounds, (int)(g.DpiX));
			play_grid.Owner = this;
			g.Dispose();
		}

		/// <summary>
		/// Try to make tile moves. Passed directly on to the play grid.
		/// </summary>
		private void Playfield_MouseDown(object sender, MouseEventArgs e) {
			Point ep = new Point(e.X, e.Y);
			if (play_grid.TapAction(ep)) {
				Refresh();
				if (!play_grid.ValidMovesAvailable()) {
					MessageBox.Show("No Moves Left!", "Shisen-Sho", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
				}
			}
		}

		private void newGameButton_Click(object sender, EventArgs e) {
			Graphics g = this.CreateGraphics();
			play_grid = new ShoGrid(11, 12, prefs.Difficulty, tile_set, this.Bounds, (int)(g.DpiX));
			play_grid.Owner = this;
			g.Dispose();
			Refresh();
		}

		private void optionsButton_Click(object sender, EventArgs e) {
			(new NewGameOptions(prefs)).ShowDialog();
		}

		private void hintButton_Click(object sender, EventArgs e) {
			play_grid.ShowHint();
		}

		private void refreshTimer_Tick(object sender, EventArgs e) {
			this.Invalidate();
		}
	}
}