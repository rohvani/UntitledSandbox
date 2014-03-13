using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using UntitledSandbox.Common.UI;

namespace UntitledSandbox.Managers
{
	public class UIManager
	{
		private static List<Component> activeWindows = new List<Component>();
		public static bool IsDragging = false;

		public static void DrawWindows()
		{
			for (int i = activeWindows.Count - 1; i >= 0; i--)
			{
				activeWindows[i].Draw();
			}
		}

		public static void RegisterComponent(Component component)
		{
			activeWindows.Insert(0, component);
		}

		public static void UnregisterComponent(Component component)
		{
			activeWindows.Remove(component);
		}

		public static Component GetWindowAt(Vector2 location)
		{
			Component activeWindow = null;

			foreach (Component window in activeWindows)
			{
				if (window.Contains(location))
				{
					activeWindow = window;
					break;
				}
			}

			return activeWindow;
		}

		public static void FocusWindow(Component component)
		{

			if (component != null && activeWindows.Contains(component))
			{
				activeWindows.Remove(component);
				activeWindows.Insert(0, component);
			}
		}

		public static void HandleClick(Vector2 clickPosition)
		{
			Component window = GetWindowAt(clickPosition);

			if (window != null)
			{
				window.RaiseClickEvent(clickPosition);
				FocusWindow(window);
			}
		}

		public static void HandleDrag(Vector2 from, Vector2 to, bool firstDrag = false)
		{
			Component window = firstDrag ? GetWindowAt(from) : activeWindows[0];

			if (window != null)
			{
				FocusWindow(window);
				window.RaiseDragEvent(from, to);		
			}
		}

		public static void CheckDragging(Vector2 from)
		{
			bool lol = GetWindowAt(from) != null;
			Console.WriteLine("isDragging = {0}", lol);
			if (lol) IsDragging = true;
		}
	}
}
