using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
//using ToonController;
//using ToonData;
using ToonExtension;
//using ToonSettings;
//using ToonUtilities;

namespace ToonGui
{
	public partial class ToolBar : NotifyPropertyChangedUserControl
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
			//if (wsController != null && GuiCouplingProvider.ActiveTool is ToolHasWidth)
			//	StrokeWidthSelectorView.Focus();

			//blkShapeSubTool.Visibility = Visibility.Collapsed;
		}


	}
}
