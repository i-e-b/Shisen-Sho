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
	class ShoGrid {
		protected ShoTiles tiles;

		protected int[,] tile_value;
		protected int difficulty;
		protected int rack_width, rack_height;
		protected Rectangle play_area;
		protected int hilite_X, hilite_Y;		// upto one tile may be selected.
		protected int old_hilite_X, old_hilite_Y;
		protected int last_pw;					// dpi used on last draw command.
		protected Form owner;

		public Form Owner { set { owner = value; } }
		public Rectangle PlayArea { get { return play_area; } }

		/// <summary>
		/// Start a grid and fill with tiles.
		/// If number of tiles would be odd, a tile is missed out.
		/// </summary>
		/// <param name="height">Number of tiles tall</param>
		/// <param name="width">Number of tiles wide</param>
		public ShoGrid(int height, int width, int challenge, ShoTiles tileSet, Rectangle screenArea, int dpi) {
			difficulty = challenge;
			hilite_Y = hilite_X = -1;
			old_hilite_X = old_hilite_Y = -1;

			int tile_count = (height * width);
			int pw = dpi / 96;
			tile_count -= (tile_count % 2);
			tiles = tileSet;

			int total_width = width * ((tiles.TileWidth - tiles.TileOverlap) * pw);
			int total_height = height * ((tiles.TileHeight - tiles.TileOverlap) * pw);

			if ((total_height > screenArea.Height) || (total_width > screenArea.Width))
				throw new Exception("Tiles won't fit into screen area.");

			rack_height = height;
			rack_width = width;

			// Calculate play area
			int mx = screenArea.Left + (screenArea.Width / 2);
			int my = screenArea.Top + (screenArea.Height / 2);
			play_area = new Rectangle(mx - (total_width/2), my - (total_height/2), total_width, total_height);

			// Now create and fill out the tile grid
			tile_value = new int[width,height];
			int i = 0, mod = tiles.MaxTileIndex + 1;

			if (mod > difficulty) mod = difficulty;

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (i >= tile_count) break;
					tile_value[x,y] = (i >> 1) % mod;
					i++;
				}
			}
			if ((height * width) % 2 > 0) tile_value[height - 1, width - 1] = -1;

			// Apply a scale to make easy games less boring to look at
			int scale = (tiles.MaxTileIndex + 1) / challenge;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (tile_value[x, y] >= 0)
						tile_value[x, y] *= scale;
				}
			}

			// Randomize the game:
			ScrambleTiles(height * width * 5); // each pair will get exchanged twice, on average.
		}

		/// <summary>
		/// Draws current tile grid in the given graphics context.
		/// </summary>
		/// <param name="g">Graphics context</param>
		/// <remarks>Grid is drawn from top-right, lefwards then downwards.
		/// This is due to the overlapped area of the tile graphics used to
		/// show the 3D nature of the tiles.</remarks>
		public void DrawGrid(Graphics g) {
			int pen_w = (int)(g.DpiX / 96.0f);
			last_pw = pen_w;
			Rectangle dest = new Rectangle();

			dest.Width = tiles.TileWidth * pen_w;
			dest.Height = tiles.TileHeight * pen_w;
			dest.Y = play_area.Top;

			int shift = dest.Width - (tiles.TileOverlap * pen_w); // amount to shift left
			int drop = dest.Height - (tiles.TileOverlap * pen_w); // amount to drop down

			for (int y = 0; y < rack_height; y++) {
				dest.X = (play_area.Right) - dest.Width;
				for (int x = rack_width - 1; x >= 0; x--) {
					int index = tile_value[x,y];
					if (index >= 0) {
						bool hilite = ((hilite_X == x) && (hilite_Y == y));
						tiles.DrawTile(g, dest, index, hilite);
					}
					dest.X -= shift;
				}
				dest.Y += drop;
			}
		}

		/// <summary>
		/// Draws a part of the current tile grid in the given graphics context.
		/// </summary>
		/// <param name="g">Graphics context</param>
		/// <param name="r">Rectangle to draw.</param>
		public void DrawGrid(Graphics g, Rectangle r) {
			throw new Exception("DrawGrid(Graphics g, Rectangle r)\n\nNot yet implemented");
		}

		/// <summary>
		/// Redraw a single tile. Also redraws part of surrounding tiles.
		/// </summary>
		/// <param name="g">Graphics context</param>
		/// <param name="x">X index of tile</param>
		/// <param name="y">Y index of tile</param>
		/// <param name="background">Brush to draw tile backgrounds</param>
		public void RedrawTile(Graphics g, int x, int y, Brush background) {
			try {
				int pw = (int)(g.DpiX / 96.0f);
				last_pw = pw;

				Rectangle tile_rect = TileRectangle(x, y);
				tile_rect.Inflate(tiles.TileOverlap * pw, tiles.TileOverlap * pw);
				Image dblBuffer = new Bitmap(tile_rect.Width, tile_rect.Height, Program.BufferFormat);
				Graphics gd = Graphics.FromImage(dblBuffer);

				// draw background, tile and it's 8 neighbors
				gd.FillRectangle(background, 0, 0, tile_rect.Width, tile_rect.Height);
				Rectangle TR = new Rectangle((-tiles.TileWidth) * pw,
					(-tiles.TileHeight) * pw,
					tiles.TileWidth * pw, tiles.TileHeight * pw);
				int mx = (tiles.TileWidth - tiles.TileOverlap)*pw;
				int my = (tiles.TileHeight - tiles.TileOverlap)*pw;
				TR.Offset(tiles.TileOverlap * pw*2, tiles.TileOverlap * pw*2);

				for (int dy = 0; dy < 3; dy++) {
					for (int dx = 2; dx >= 0; dx--) {
						Rectangle dest = new Rectangle(TR.X + (dx * mx), TR.Y + (dy * my), TR.Width, TR.Height);
						int tv = safeValue(x+(dx-1), y+(dy-1));
						bool hilite = (x+(dx-1) == hilite_X) && (y+(dy-1) == hilite_Y);

						tiles.DrawTile(gd, dest, tv, hilite);
					}
				}

				g.DrawImage(dblBuffer, tile_rect.X, tile_rect.Y);
				dblBuffer = null;
				GC.Collect();
			} catch (OutOfMemoryException ex) {
				// Try a non-buffered version. It will flicker, but needs a lot less memory.
				// TODO: implement (probably using a mask region)
			}
		}

		public bool SwapHilites(Graphics g, Brush background) {
			if ((hilite_X == old_hilite_X) && (hilite_Y == old_hilite_Y)) return false;

			RedrawTile(g, old_hilite_X, old_hilite_Y, background);
			RedrawTile(g, hilite_X, hilite_Y, background);

			old_hilite_X = hilite_X;
			old_hilite_Y = hilite_Y;

			return true;
		}

		/// <summary>
		/// React to a pen tap. Returns true if screen should redraw.
		/// </summary>
		/// <param name="location">Container-based location of pen-tap</param>
		/// <returns>True if screen should redraw</returns>
		public bool TapAction(Point location) {
			if (!play_area.Contains(location)) return false;

			// figure out which tile index got clicked:
			int px, py;
			px = (location.X - play_area.X) / ((tiles.TileWidth - tiles.TileOverlap) * last_pw);
			py = (location.Y - play_area.Y) / ((tiles.TileHeight - tiles.TileOverlap) * last_pw);

			if (safeValue(px, py) < 0) return false;

			old_hilite_X = hilite_X;
			old_hilite_Y = hilite_Y;

			// Check tiles, clear if possible:
			if ((hilite_X != px || hilite_Y != py)) {
				Point[] cons = ConnectPoints(hilite_X, hilite_Y, px, py);
				if (cons != null) {
					ShowConnection(cons);
					tile_value[px, py] = -1;
					tile_value[hilite_X, hilite_Y] = -1;
					hilite_X = hilite_Y = -1;
					old_hilite_X = old_hilite_Y = -1;
				} else {
					hilite_X = px;
					hilite_Y = py;
				}
			} else {
				hilite_X = hilite_Y = -1;
			}

			return true;
		}

		/// <summary>
		/// Test for available moves. Returns true if there is at least one move available.
		/// </summary>
		/// <remarks>Makes heavy use of ConnectPoints()</remarks>
		/// <returns>true if there is at least one move available</returns>
		public bool ValidMovesAvailable() {
			for (int ay = 0; ay < rack_height; ay++) {
				for (int ax = 0; ax < rack_width; ax++) {
					//  now check it's pair
					for (int by = ay; by < rack_height; by++) {
						for (int bx = 0; bx < rack_width; bx++) {
							//  now check it's pair

							if ((ax == bx) && (ay == by)) continue;

							if (ConnectPoints(ax, ay, bx, by) != null) return true;
						}
					}

				}
			}
			return false;
		}

		/// <summary>
		/// Show graphic view of an available move.
		/// </summary>
		/// <remarks>Makes heavy use of ConnectPoints()</remarks>
		public void ShowHint() {
			for (int ay = 0; ay < rack_height; ay++) {
				for (int ax = 0; ax < rack_width; ax++) {
					//  now check it's pair
					for (int by = ay; by < rack_height; by++) {
						for (int bx = 0; bx < rack_width; bx++) {
							//  now check it's pair

							if ((ax == bx) && (ay == by)) continue;
							Point[] conn = ConnectPoints(ax, ay, bx, by);
							if (conn != null) {
								ShowConnection(conn);
								System.Threading.Thread.Sleep(250);
								owner.Invalidate();
								return;
							}
						}
					}

				}
			}
		}

		/// <summary>
		/// Checks grid for tiles. Returns false if there are tiles.
		/// </summary>
		public bool BoardClear() {
			for (int y = 0; y < rack_height; y++) {
				for (int x = 0; x < rack_width; x++) {
					if (tile_value[x, y] >= 0) return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Try to connect two tiles using no more than 3 straight lines,
		/// each of one of the four compass directions. Returns a series
		/// of *GRAPHICS* points for drawing connection confirmations.
		/// </summary>
		/// <remarks>This is the core game logic. If a game rule needs changing,
		/// the code is probably in this method.</remarks>
		/// <param name="ax">X index of first tile</param>
		/// <param name="ay">Y index of first tile</param>
		/// <param name="bx">X index of second tile</param>
		/// <param name="by">Y index of second tile</param>
		/// <returns>NULL if no legal connection, otherwise a list of points to draw lines with.</returns>
		protected Point[] ConnectPoints(int ax, int ay, int bx, int by) {
			#region notes
			// The strategy is:
			// find the north, east, south & west extents of both A & B.
			// check cases, first one is returned:
			//   -> if A & B share extents (same row or column and no obstructions
			//      they are connected: points are A, B;
			//   -> if extents overlap, they are connected: points are A, overlap, B;
			//   -> if extents run parallel, check for clear lines between them,
			//      starting with  middle between A & B. If one exists, 
			//      A & B are connected: points are A, departure from extent, entrance to extent, B.
			#endregion

			// Check that points are valid
			if ((ax < 0) || (ax >= rack_width)) return null;
			if ((bx < 0) || (bx >= rack_width)) return null;
			if ((ay < 0) || (ay >= rack_height)) return null;
			if ((by < 0) || (by >= rack_height)) return null;

			// check that tiles are the same type
			if (tile_value[ax, ay] != tile_value[bx, by]) return null;
			if (tile_value[ax, ay] < 0) return null; // don't connect empty spaces.

			int aN, aW, aS, aE;	// North, south, east & west of A.
			int bN, bW, bS, bE;	// and of B.

			#region Find extents
			aN = aS = ay;
			aW = aE = ax;
			bN = bS = by;
			bW = bE = bx;
			// A-west
			for (int i = ax; i >= -1; i--) {
				if ((i == ax) || (i == -1)) aW = i;
				else if (tile_value[i, ay] < 0) aW = i;
				else break;
			}
			// B-west
			for (int i = bx; i >= -1; i--) {
				if ((i == bx) || (i == -1)) bW = i;
				else if (tile_value[i, by] < 0) bW = i;
				else break;
			}
			// A-east
			for (int i = ax; i <= rack_width; i++) {
				if ((i == ax) || (i == rack_width)) aE = i;
				else if (tile_value[i, ay] < 0) aE = i;
				else break;
			}
			// B-east
			for (int i = bx; i <= rack_width; i++) {
				if ((i == bx) || (i == rack_width)) bE = i;
				else if (tile_value[i, by] < 0) bE = i;
				else break;
			}
			// A-north
			for (int i = ay; i >= -1; i--) {
				if ((i == ay) || (i == -1)) aN = i;
				else if (tile_value[ax, i] < 0) aN = i;
				else break;
			}
			// B-north
			for (int i = by; i >= -1; i--) {
				if ((i == by) || (i == -1)) bN = i;
				else if (tile_value[bx, i] < 0) bN = i;
				else break;
			}
			// A-south
			for (int i = ay; i <= rack_height; i++) {
				if ((i == ay) || (i == rack_height)) aS = i;
				else if (tile_value[ax, i] < 0) aS = i;
				else break;
			}
			// B-south
			for (int i = by; i <= rack_height; i++) {
				if ((i == by) || (i == rack_height)) bS = i;
				else if (tile_value[bx, i] < 0) bS = i;
				else break;
			}
			#endregion

			#region check single connection
			if (ax == bx) { // same column
				if (ay > by) {
					if ((bS == ay - 1) && (aN == by + 1)) {
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(bx, by) };
					}
				} else {
					if ((bN == ay + 1) && (aS == by - 1)) {
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(bx, by) };
					}
				}
			} else if (ay == by) { // same row
				if (ax > bx) {
					if ((bE == ax - 1) && (aW == bx + 1)) {
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(bx, by) };
					}
				} else {
					if ((bW == ax + 1) && (aE == bx - 1)) {
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(bx, by) };
					}
				}
			}
			#endregion

			#region check double connection
			// Simple brute-force check
			if (ay < by) {
				if (ax < bx) {
					if ((aS >= by) && (bW <= ax))
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(ax, by), CentreOfTile(bx, by) };
					if ((aE >= bx) && (bN <= ay))
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(bx, ay), CentreOfTile(bx, by) };
				} else {
					if ((aS >= by) && (bE >= ax))
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(ax, by), CentreOfTile(bx, by) };
					if ((aW <= bx) && (bN <= ay))
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(bx, ay), CentreOfTile(bx, by) };
				}
			} else if (ay > by) {
				if (ax < bx) {
					if ((aN <= by) && (bW <= ax))
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(ax, by), CentreOfTile(bx, by) };
					if ((aE >= bx) && (bS >= ay))
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(bx, ay), CentreOfTile(bx, by) };
				} else {
					if ((aW <= bx) && (bS >= ay))
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(bx, ay), CentreOfTile(bx, by) };
					if ((aN <= by) && (bE >= ax))
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(ax, by), CentreOfTile(bx, by) };
				}
			}
			#endregion

			#region check triple connection
			Point[] mids = null;
			// If we get to this point, then the only valid connections
			// use three points: two extents and a bridge between.
			#region  U-bends
			if ((aN < by) && (bN < ay)) { // both north
				int near = (ay < by) ? (ay) : (by);
				int far = (aN < bN) ? (bN) : (aN);

				for (int i = near; i >= far; i--) {
					mids = CheckSingleConnection(ax, i, bx, i);
					if (mids != null)
						return new Point[] { CentreOfTile(ax, ay), mids[0], mids[1], CentreOfTile(bx, by) };
				}
			}
			if ((aS > by) && (bS > ay)) { // both south
				int near = (ay > by) ? (ay) : (by);
				int far = (aS > bS) ? (bS) : (aS);

				for (int i = near; i <= far; i++) {
					mids = CheckSingleConnection(ax, i, bx, i);
					if (mids != null)
						return new Point[] { CentreOfTile(ax, ay), mids[0], mids[1], CentreOfTile(bx, by) };
				}
			}
			if ((aE > bx) && (bE > ax)) { // both east
				int near = (ax > bx) ? (ax) : (bx);
				int far = (aE > bE) ? (bE) : (aE);

				for (int i = near; i <= far; i++) {
					mids = CheckSingleConnection(i, ay, i, by);
					if (mids != null)
						return new Point[] { CentreOfTile(ax, ay), mids[0], mids[1], CentreOfTile(bx, by) };
				}
			} if ((aW < bx) && (bW < ax)) { // both west
				int near = (ax < bx) ? (ax) : (bx);
				int far = (aW < bW) ? (bW) : (aW);

				for (int i = near; i >= far; i--) {
					mids = CheckSingleConnection(i, ay, i, by);
					if (mids != null)
						return new Point[] { CentreOfTile(ax, ay), mids[0], mids[1], CentreOfTile(bx, by) };
				}
			}
			#endregion
			#region S-bends
			// Run lines between extent overlaps.
			if ((ax > bx) && (aW <= bE)) {
				int near = (bx > aW) ? (bx) : (aW);
				int far = (ax < bE) ? (ax) : (bE);
				for (int i = near; i <= far; i++) {
					mids = CheckSingleConnection(i, ay, i, by);
					if (mids != null)
						return new Point[] { CentreOfTile(ax, ay), mids[0], mids[1], CentreOfTile(bx, by) };
				}
			}
			if ((ax < bx) && (aE >= bW)) {
				int near = (ax > bW) ? (ax) : (bW);
				int far = (bx < aE) ? (bx) : (aE);
				for (int i = near; i <= far; i++) {
					mids = CheckSingleConnection(i, ay, i, by);
					if (mids != null)
						return new Point[] { CentreOfTile(ax, ay), mids[0], mids[1], CentreOfTile(bx, by) };
				}
			}
			if ((ay > by) && (aN <= bS)) {
				int near = (by > aN) ? (by) : (aN);
				int far = (ay < bS) ? (ay) : (bS);
				for (int i = near; i <= far; i++) {
					mids = CheckSingleConnection(ax, i, bx, i);
					if (mids != null)
						return new Point[] { CentreOfTile(ax, ay), mids[0], mids[1], CentreOfTile(bx, by) };
				}
			}
			if ((ay < by) && (aS >= bN)) {
				int near = (ay > bN) ? (ay) : (bN);
				int far = (by < aS) ? (by) : (aS);
				for (int i = near; i <= far; i++) {
					mids = CheckSingleConnection(ax, i, bx, i);
					if (mids != null)
						return new Point[] { CentreOfTile(ax, ay), mids[0], mids[1], CentreOfTile(bx, by) };
				}
			}
			#endregion
			#endregion

			return null;
		}

		/// <summary>
		/// Tests for a straight-line only connection between two points
		/// </summary>
		/// <remarks>This is a duplicate of the first part of ConnectPoints(),
		/// but calculates only the extents that are needed for single line checking.</remarks>
		/// <param name="ax">X index of first tile</param>
		/// <param name="ay">Y index of first tile</param>
		/// <param name="bx">X index of second tile</param>
		/// <param name="by">Y index of second tile</param>
		/// <returns>NULL if no legal connection, otherwise a list of points to draw lines with.</returns>
		private Point[] CheckSingleConnection(int ax, int ay, int bx, int by) {
			int aN, aW, aS, aE;	// North, south, east & west of A.
			int bN, bW, bS, bE;	// and of B.

			#region Find extents
			aN = aS = ay;
			aW = aE = ax;
			bN = bS = by;
			bW = bE = bx;
			// A-west
			/*if (bx <= ax)*/ for (int i = ax; i >= -1; i--) {
					if ((i == ax) || (i == -1)) aW = i;
					else if (safeValue(i, ay) < 0) aW = i;
					else break;
				}
			// B-west
			/*if (ax <= bx)*/ for (int i = bx; i >= -1; i--) {
					if ((i == bx) || (i == -1)) bW = i;
					else if (safeValue(i, by) < 0) bW = i;
					else break;
				}
			// A-east
			/*if (bx >= ax)*/ for (int i = ax; i <= rack_width; i++) {
					if ((i == ax) || (i == rack_width)) aE = i;
					else if (safeValue(i, ay) < 0) aE = i;
					else break;
				}
			// B-east
			/*if (ax >= bx)*/ for (int i = bx; i <= rack_width; i++) {
					if ((i == bx) || (i == rack_width)) bE = i;
					else if (safeValue(i, by) < 0) bE = i;
					else break;
				}
			// A-north
			/*if (by <= ay)*/ for (int i = ay; i >= -1; i--) {
					if ((i == ay) || (i == -1)) aN = i;
					else if (safeValue(ax, i) < 0) aN = i;
					else break;
				}
			// B-north
			/*if (ay <= by)*/ for (int i = by; i >= -1; i--) {
					if ((i == by) || (i == -1)) bN = i;
					else if (safeValue(bx, i) < 0) bN = i;
					else break;
				}
			// A-south
			/*if (by >= ay)*/ for (int i = ay; i <= rack_height; i++) {
					if ((i == ay) || (i == rack_height)) aS = i;
					else if (safeValue(ax, i) < 0) aS = i;
					else break;
				}
			// B-south
			/*if (ay >= by)*/ for (int i = by; i <= rack_height; i++) {
					if ((i == by) || (i == rack_height)) bS = i;
					else if (safeValue(bx, i) < 0) bS = i;
					else break;
				}
			#endregion

			#region check single connection
			if (ax == bx) { // same column
				if (ay > by) {
					if ((bS >= ay - 1) && (aN <= by + 1)) {
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(bx, by) };
					}
				} else {
					if ((bN <= ay + 1) && (aS >= by - 1)) {
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(bx, by) };
					}
				}
			} else if (ay == by) { // same row
				if (ax > bx) {
					if ((bE >= ax - 1) && (aW <= bx + 1)) {
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(bx, by) };
					}
				} else {
					if ((bW <= ax + 1) && (aE >= bx - 1)) {
						return new Point[] { CentreOfTile(ax, ay), CentreOfTile(bx, by) };
					}
				}
			}
			#endregion

			return null;
		}

		/// <summary>
		/// Get tile values, regardless of grid size (all tiles not in the grid are empty)
		/// </summary>
		/// <param name="x">Tile X index</param>
		/// <param name="y">Tile Y index</param>
		/// <returns>Tile value</returns>
		protected int safeValue(int x, int y) {
			if ((x < 0) || (x >= rack_width)) return -1;
			if ((y < 0) || (y >= rack_height)) return -1;
			return tile_value[x, y];
		}

		protected void ShowConnection(Point[] lines) {
			if (owner == null) return;
			Graphics g = owner.CreateGraphics();

			Pen p = new Pen(owner.ForeColor, last_pw * 5);

			g.DrawLines(p, lines);

			System.Threading.Thread.Sleep(250);

			g.Dispose();
		}

		protected Point CentreOfTile(int x, int y) {
			int h = (tiles.TileHeight - tiles.TileOverlap) * last_pw;
			int w = (tiles.TileWidth - tiles.TileOverlap) * last_pw;

			int dx = (w * x) + play_area.Left + (w / 2);
			int dy = (h * y) + play_area.Top + (h / 2);

			return new Point(dx, dy);
		}

		/// <summary>
		/// Returns the graphic bounds of a tile index.
		/// </summary>
		protected Rectangle TileRectangle(int x, int y) {
			int h = (tiles.TileHeight - tiles.TileOverlap) * last_pw;
			int w = (tiles.TileWidth - tiles.TileOverlap) * last_pw;

			int L = ((w * x) + play_area.Left) - (tiles.TileOverlap * last_pw);
			int T = (h * y) + play_area.Top;

			return new Rectangle(L, T, tiles.TileWidth * last_pw, tiles.TileHeight * last_pw);
		}

		/// <summary>
		/// Scramble the tiles by repeatedly exchanging random pairs of tiles.
		/// </summary>
		/// <param name="rounds">Number of pairs to exchange.</param>
		private void ScrambleTiles(int rounds) {
			Random r = new Random();
			int ax, ay, bx, by;
			int tmp, mod = tiles.MaxTileIndex + 1;

			for (int i = 0; i < rounds; i++) {
				ax = r.Next(rack_width);
				ay = r.Next(rack_height);
				bx = r.Next(rack_width);
				by = r.Next(rack_height);

				tmp = tile_value[bx,by];
				tile_value[bx,by] = tile_value[ax,ay];
				tile_value[ax,ay] = tmp;
			}
		}
	}
}
