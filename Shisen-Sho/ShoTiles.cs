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
	class ShoTiles {
		protected Bitmap tile_image;
		protected ImageAttributes imgAttr;

		public virtual Bitmap TileImage { get { return tile_image; } }
		public virtual int MaxTileIndex { get { return 35; } }
		/// <summary>Tile edge overlap. Multiply by (DPI/96).</summary>
		public virtual int TileOverlap { get { return 2; } }
		public virtual int TileWidth { get { return 20; } }
		public virtual int TileHeight { get { return 28; } }

		public ShoTiles() {
			loadTileImage(@"Images\ksho_set.png");
		}

		/// <summary>
		/// Draw a tile into a rectangle in a graphics context.
		/// </summary>
		/// <param name="g">Target graphics context</param>
		/// <param name="Destination">Destination rectangle. Image is scaled to fit
		/// (using a nearest-neighbor algorithm)</param>
		/// <param name="TileIndex">Zero-based index of tile value</param>
		/// <param name="Highlighted">If true, tile will be drawn with an overlay effect
		/// to show highlighting.</param>
		public virtual void DrawTile(Graphics g, Rectangle Destination, int TileIndex, bool Highlighted) {
			int x, y;

			if ((TileIndex < 0) || (TileIndex > MaxTileIndex))
				return;

			x = 40 * (TileIndex % 9);
			y = 56 * (int)(TileIndex / 9); //... which is why integer math always truncates and never rounds!

			g.DrawImage(TileImage, Destination, x, y, 40, 56, GraphicsUnit.Pixel, imgAttr);

			if (!Highlighted) return;

			// Hatch the rectangle with diagonal lines of navy blue.
			Pen hatch = new Pen(Color.Navy, 1.0f);
			int T = Destination.Y;
			int L = Destination.X;
			int B = (T + Destination.Height) - 2;
			int R = (L + Destination.Width) - 1;

			int dx, dy, dd;

			dy = B - T; // Top edge
			for (int i = L; i < R; i += 2) {
				dx = R - i;
				dd = (dy > dx) ? (dx) : (dy);
				g.DrawLine(hatch, i, T, i + dd, T + dd);
			}

			dx = R - L; // Left edge
			for (int i = T; i < B; i += 2) {
				dy = B - i;
				dd = (dy > dx) ? (dx) : (dy);
				g.DrawLine(hatch, L, i, L + dd, i + dd);
			}
		}

		public virtual void DrawBlankTile(Graphics g, Rectangle Destination, bool Highlighted) {
			g.DrawImage(TileImage, Destination, 0, 0, 40, 56, GraphicsUnit.Pixel, imgAttr);

			if (!Highlighted) return;

			// Hatch the rectangle with diagonal lines of navy blue.
			Pen hatch = new Pen(Color.Navy, 1.0f);
			int T = Destination.Y;
			int L = Destination.X;
			int B = (T + Destination.Height) - 2;
			int R = (L + Destination.Width) - 1;

			int dx, dy, dd;

			dy = B - T; // Top edge
			for (int i = L; i < R; i += 2) {
				dx = R - i;
				dd = (dy > dx) ? (dx) : (dy);
				g.DrawLine(hatch, i, T, i + dd, T + dd);
			}

			dx = R - L; // Left edge
			for (int i = T; i < B; i += 2) {
				dy = B - i;
				dd = (dy > dx) ? (dx) : (dy);
				g.DrawLine(hatch, L, i, L + dd, i + dd);
			}
		}

		protected void loadTileImage(string fileName) {
			Assembly asm = Assembly.GetExecutingAssembly();
			string full_path = asm.ManifestModule.FullyQualifiedName.Replace(asm.ManifestModule.Name, fileName);
			Bitmap temp_image = new Bitmap(full_path);

			if ((temp_image.Height < 1) || (temp_image.Width < 1)) {
				throw new FormatException("Tileset image did not load properly.");
			}

			// convert image to a good format now, to speed up drawing later.
			tile_image = new Bitmap(temp_image.Width, temp_image.Height, Program.BufferFormat);
			Graphics g = Graphics.FromImage(tile_image);
			g.DrawImage(temp_image, 0, 0);
			g.Dispose();

			// Set attributes (FF00FF is transparent... why are Micro$oft so against alpha channels?)
			imgAttr = new ImageAttributes();
			imgAttr.SetColorKey(Color.Fuchsia, Color.Fuchsia);
		}
	}
}
