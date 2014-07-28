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

namespace Shisen_Sho.Tilesets {
	class TraditionalTiles : ShoTiles {

		public override int MaxTileIndex { get { return 42; } }

		public TraditionalTiles() {
			loadTileImage(@"Images\traditional.png");
		}

		public override void DrawTile(System.Drawing.Graphics g, System.Drawing.Rectangle Destination, int TileIndex, bool Highlighted) {
			int x, y, ti, tx, ty;

			if ((TileIndex < 0) || (TileIndex > MaxTileIndex))
				return;

			// Draw the tile underneath:
			ti = (Highlighted) ? (44) : (43);
			tx = 40 * (ti % 9);
			ty = 56 * (int)(ti / 9);

			x = 40 * (TileIndex % 9);
			y = 56 * (int)(TileIndex / 9);

			g.DrawImage(TileImage, Destination, tx, ty, 40, 56, GraphicsUnit.Pixel, imgAttr);
			g.DrawImage(TileImage, Destination, x, y, 40, 56, GraphicsUnit.Pixel, imgAttr);
		}

		public override void DrawBlankTile(Graphics g, Rectangle Destination, bool Highlighted) {
			int x, y, ti, tx, ty;

			// Draw the tile underneath:
			ti = (Highlighted) ? (44) : (43);
			tx = 40 * (ti % 9);
			ty = 56 * (int)(ti / 9);

			g.DrawImage(TileImage, Destination, tx, ty, 40, 56, GraphicsUnit.Pixel, imgAttr);
		}
	}
}
