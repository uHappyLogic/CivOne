// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using CivOne.Enums;
using CivOne.Events;
using CivOne.Interfaces;
using CivOne.GFX;

namespace CivOne.Templates
{
	public abstract class BaseScreen : BaseInstance, IScreen
	{
		private readonly List<Screens.Menu> _menus = new List<Screens.Menu>();

		protected Picture _canvas = new Picture(320, 200);
		
		protected Player HumanPlayer
		{
			get
			{
				return Game.Instance.HumanPlayer;
			}
		}
		
		protected void AddLayer(IScreen screen, Point point)
		{
			AddLayer(screen, point.X, point.Y);
		}
		protected void AddLayer(IScreen screen, int x = 0, int y = 0)
		{
			_canvas.AddLayer(screen.Canvas.Image, x, y);
		}
		protected void AddLayer(Picture picture, Point point)
		{
			AddLayer(picture, point.X, point.Y);
		}
		protected void AddLayer(Picture picture, int x = 0, int y = 0)
		{
			_canvas.AddLayer(picture.Image, x, y);
		}
		protected void AddLayer(Bitmap bitmap, Point point)
		{
			AddLayer(bitmap, point.X, point.Y);
		}
		protected void AddLayer(Bitmap bitmap, int x = 0, int y = 0)
		{
			_canvas.AddLayer(bitmap, x, y);
		}

		protected void DrawButton(string text, byte colour, byte colourDark, int x, int y, int width)
		{
			_canvas.FillRectangle(7, x, y, width, 1);
			_canvas.FillRectangle(7, x, y + 1, 1, 8);
			_canvas.FillRectangle(colourDark, x + 1, y + 8, width - 1, 1);
			_canvas.FillRectangle(colourDark, x + width - 1, y, 1, 8);
			_canvas.FillRectangle(colour, x + 1, y + 1, width - 2, 7);
			_canvas.DrawText(text, 1, colourDark, x + (int)Math.Ceiling((double)width / 2), y + 2, TextAlign.Center);
		}
		
		protected void MouseArgsOffset(ref ScreenEventArgs args, int offsetX, int offsetY)
		{
			args = new ScreenEventArgs(args.X - offsetX, args.Y - offsetY, args.Buttons);
		}

		public event EventHandler Closed;

		protected void HandleClose()
		{
			if (Closed == null)
				return;
			Closed(this, null);
		}
		
		public virtual Picture Canvas
		{
			get
			{
				return _canvas;
			}
		}
		public Color[] Palette
		{
			get
			{
				return Canvas.Image.Palette.Entries;
			}
		}
		public virtual MouseCursor Cursor { get; protected set; }
		public abstract bool HasUpdate(uint gameTick);
		public virtual bool KeyDown(KeyboardEventArgs args)
		{
			return false;
		}
		public virtual bool MouseDown(ScreenEventArgs args)
		{
			return false;
		}
		public virtual bool MouseUp(ScreenEventArgs args)
		{
			return false;
		}
		public virtual bool MouseDrag(ScreenEventArgs args)
		{
			return false;
		}
		
		protected bool HasMenu
		{
			get
			{
				return _menus.Any();
			}
		}
		protected void AddMenu(Screens.Menu menu)
		{
			_menus.Add(menu);
			Common.AddScreen(menu);
		}
		protected void CloseMenus()
		{
			foreach (Screens.Menu menu in _menus)
			{
				menu.Close();
			}
			_menus.Clear();
		}
		protected void Destroy()
		{
			CloseMenus();
			Common.DestroyScreen(this);
		}
	}
}