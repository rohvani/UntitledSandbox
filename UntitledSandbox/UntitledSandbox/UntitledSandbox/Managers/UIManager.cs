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
		public List<UIComponent> activeWindows;

		public UIManager()
		{
			activeWindows = new List<UIComponent>();
		}

		public void drawWindows()
		{
			for (int i = activeWindows.Count - 1; i >= 0; i--) activeWindows[i].Draw();
		}

		public void registerUIComponent(UIComponent component)
		{
			activeWindows.Insert(0, component);
		}

		public void unregisterUIComponent(UIComponent component)
		{
			activeWindows.Remove(component);
		}

		public void handleClick(Vector2 clickPosition)
		{
			UIComponent activeWindow = null;

			foreach (UIComponent window in activeWindows)
			{
				if (window.containsClick(clickPosition))
				{
					window.handleClick(clickPosition);
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
