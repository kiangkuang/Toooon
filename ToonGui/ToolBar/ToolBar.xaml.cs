using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ToonGui
{
	public partial class ToolBar : UserControl
	{
		public ToolBar()
		{
			InitializeComponent();
		}

		private void bmiTool_Click(object sender, RoutedEventArgs e)
		{
			ImageToggleButton bmi = sender as ImageToggleButton;
			ToolBase tool = bmi.Tag as ToolBase;

			// Step 1: Check if the sender has the tool of the currently active tool
			if (tool == GuiCouplingProvider.ActiveTool)
			{
				bmi.IsChecked = true; // Do NOT let the user to click to deselect
				return;
			}

			// TODO: maybe this is redundant, because in GuiCoupling.ActiveTool, we did it
			// Step 2: Deactivate the current tool
			if (GuiCouplingProvider.ActiveTool != null && GuiCouplingProvider.ActiveTool.ToolState != ToolState.Idle)
			{
				GuiCouplingProvider.ActiveTool.Deactivate();
			}

			GuiCouplingProvider.ActiveTool = tool;

			// TODO: Maybe we don't need this, because this should be handled by ShortcutManager
			// Special Handling for StrokeWidthRelevant tools
			if (wsController != null && GuiCouplingProvider.ActiveTool is ToolHasWidth)
				StrokeWidthSelectorView.Focus();

			blkShapeSubTool.Visibility = Visibility.Collapsed;
		}


	}
}
