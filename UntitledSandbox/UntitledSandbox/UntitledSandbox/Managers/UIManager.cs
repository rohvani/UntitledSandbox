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

		public static void HandleClick(Vector2 clickPosition)
		{
			Component activeWindow = null;

			foreach (Component window in activeWindows)
			{
				if (window.Contains(clickPosition))
				{
					window.HandleClick(clickPosition);
					activeWindow = window;
					break;
				}
			}

			if (activeWindow != null)
			{
				activeWindows.Remove(activeWindow);
				activeWindows.Insert(0, activeWindow);
			}
		}
	}
}
