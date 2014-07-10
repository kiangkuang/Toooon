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
 public GuiCoupling GuiCouplingProvider
        {
            get { return GuiCoupling.Instance; }
        }

        public ToolBar()
        {
            InitializeComponent();

            MouseUtils.AddHandlerForMouseButtonFocusableComponent(this, (ss, ee) =>
            {
                GlobalStates.Instance.FocusArea = TnFocusArea.ToolBar;
            });
        }

        private TnUserData UserData
        {
            get { return wsController.Data.Core.Project.RawObject.UserData.RawObject; }
        }
        private TnSessionData SessionData
        {
            get { return wsController.Data.Session; }
        }

        private WorkspaceController wsController = null;
        public WorkspaceController WorkspaceController
        {
            get { return wsController; }
            set
            {
                if (value == wsController)
                    return;

                wsController = value;
                NotifyPropertyChanged("WorkspaceController");

                if (wsController != null)
                {
                    Messenger.Default.Register<SelectedElementsUpdatedMessage>(this, mess =>
                    {
                        if (mess.SelectedElements.IsNullOrEmpty())
                        {
                            StrokeWidthSelectorView.PropertyState = PropertyState.Normal;
                        }
                        else
                        {
                            // Virtualize a TnGroup element to make use of TnGroup.GetAllWidths
                            TnGroup group = new TnGroup(wsController.SelectedElements_R);
                            HashSet<double?> widths = group.GetAllWidths();

                            // Check if contains a non-TnGroup element without Width property
                            if (widths.Contains(null))
                            {
                                StrokeWidthSelectorView.PropertyState = PropertyState.CrossMark;
                                return;
                            }

                            StrokeWidthSelectorView.PropertyState = (widths.Count == 1) ? PropertyState.Normal : PropertyState.QuestionMark;
                            StrokeWidthSelectorView.SetStrokeWidth((int)(widths.First()));
                        }
                    });
                }
            }
        }

        public void Initialize(Workspace ws)
        {
            WorkspaceController = ws.WorkspaceController;

            bmiAnimate.Tag         = new ToolAnimate(ws);
            bmiPivot.Tag           = new ToolPivot(ws);
            bmiSelect.Tag          = new ToolSelect(ws);
            bmiPencil.Tag          = new ToolPencil(ws);
            bmiInvisibleInk.Tag    = new ToolInvisibleInk(ws);
            bmiBucket.Tag          = new ToolBucket(ws);
            bmiDropper.Tag         = new ToolDropper(ws);
            bmiPenknife.Tag        = new ToolPenknife(ws);
            bmiText.Tag            = new ToolText(ws);
            bmiHand.Tag            = new ToolHand(ws);
            bmiMagnify.Tag         = new ToolMagnify(ws);

            bmiShape.Tag = "ToolShape"; // NOTE: this is special see ToolIsCheckedConverter

            bmiShape_Rectangle.Tag    = new ToolRectangle(ws);
            bmiShape_Triangle.Tag     = new ToolTriangle(ws);
            bmiShape_Ellipse.Tag      = new ToolEllipse(ws);
            bmiShape_Star.Tag         = new ToolStar(ws);
            bmiShape_SpeechBubble.Tag = new ToolSpeechBubble(ws);
            bmiShape_Pen.Tag          = new ToolPen(ws);

            lastShapeTool = bmiShape_Rectangle.Tag as ToolShape;

            GuiCouplingProvider.ActiveTool = bmiPencil.Tag as ToolPencil;

// TODO: The user can still manually switch to another tool by simply clicking
//       the tool.  We should disable a lot of tools as well as other features (Playing Senario)
            ViewModelLocator.AppVMLocator.PlayBackMenuVM.PropertyChanged += new PropertyChangedEventHandler((s, e) =>
            {
                if (e.PropertyName == "IsPlaying" && ViewModelLocator.AppVMLocator.PlayBackMenuVM.IsPlaying)
                {
                    // When it's in Playing Mode, the current tool should switch to Animate Tool
                    // (if is not already), and the Animate Selection Box should be hidden (NOT deselected).
                    if (GuiCouplingProvider.ActiveTool.ToolType != TnToolType.Pivot)
                    {
                        bmiTool_Click(bmiPivot, null);
                    }
                }
            });
        }

        private void bmiTool_Click(object sender, RoutedEventArgs e)
        {
            ImageToggleButton bmi  = sender as ImageToggleButton;
            ToolBase          tool = bmi.Tag as ToolBase;

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

        private ToolShape lastShapeTool = null;
        private void bmiShape_Click(object sender, RoutedEventArgs e)
        {
            // Special Handing: when the bmiShape is clicked it should always be checked
            bmiShape.IsChecked = true;

// TODO: maybe this is redundant, because in GuiCoupling.ActiveTool, we did it
            // Deactivate the current tool
            if (GuiCouplingProvider.ActiveTool != null && GuiCouplingProvider.ActiveTool.ToolState != ToolState.Idle)
            {
                GuiCouplingProvider.ActiveTool.Deactivate();
            }

            GuiCouplingProvider.ActiveTool = lastShapeTool;

            blkShapeSubTool.ReverseVisibility();
        }

        private void bmiShape_SubTool_Click(object sender, RoutedEventArgs e)
        {
            ImageButton bmi  = sender as ImageButton;
            ToolShape   tool = bmi.Tag as ToolShape;

            // Step 1: Check if the sender has the tool of the currently active tool
            if (tool == GuiCouplingProvider.ActiveTool)
            {
                return;
            }

// TODO: maybe this is redundant, because in GuiCoupling.ActiveTool, we did it
            // Deactivate the current tool
            if (GuiCouplingProvider.ActiveTool != null && GuiCouplingProvider.ActiveTool.ToolState != ToolState.Idle)
            {
                GuiCouplingProvider.ActiveTool.Deactivate();
            }

            bmiShape.IconName = "ToolBar_ShapeTool_WhiteArrow_" + tool.ToolType.ToString() + "Tool";
            GuiCouplingProvider.ActiveTool = tool;
            lastShapeTool = tool;

// TODO: Maybe we don't need this, because this should be handled by ShortcutManager
            if (StrokeWidthSelectorView != null)
                StrokeWidthSelectorView.Focus();
        }

        private void StrokeWidthSelectorView_StrokeWidthChanged(object sender, ValueChangedEventArgs<double> e)
        {
            if (wsController != null)
            {
                SessionData.StrokeWidth = e.NewValue;
                wsController.UpdateSelectedElementsStrokeWidth(e.NewValue);
            }
        }

        public void MainPageKeyDown(object sender, KeyEventArgs e)
        {
            
        }

        public void MainPageKeyUp(object sender, KeyEventArgs e)
        {

        }

        public void InvokeMagnifyTool(ToolMagnify.ZoomType zoomType)
        {
            ToolMagnify toolMagnify = bmiMagnify.Tag as ToolMagnify;

            if (toolMagnify != null)
                toolMagnify.ZoomDoIt(zoomType);
        }
    }

    #region Converters

    public class ToolBarButtonIconNameConverter : MarkupExtensionSingleValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            ToolBase activeTool = value as ToolBase;
            
            string core = activeTool is ToolShape ? "ShapeTool_" + activeTool.ToolType.ToString() : activeTool.ToolType.ToString();
            return "ToolBar_Button_" + core + "Tool";
        }
    }

    public class ToolIsCheckedConverter : MarkupExtensionMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(item => item == null))
                return null;

            ToolBase activeTool = values[1] as ToolBase;

            if ((values[0] as string) == "ToolShape")
            {
                return activeTool is ToolShape;
            }

            ToolBase senderTool = values[0] as ToolBase;
            return senderTool == activeTool;
        }
    }

    public class PencilSmoothnessSliderConverter : MarkupExtensionSingleValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            ToolBase activeTool = value as ToolBase;
            return (activeTool is ToolPencil) && !(activeTool is ToolInvisibleInk) ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public class HiddenStrokeWidthSelectorBackgroundConverter : MarkupExtensionMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ToolBase    v0 = values[0] as ToolBase;
            Visibility? v1 = values[1] as Visibility?;

// TODO: sort this out
            bool       relevant = (v0 == null) ? false : ToolHasWidth.HasSafeRealWidth(v0);
            Visibility hpv      = (v1 == null) ? Visibility.Collapsed : v1.Value; // hidablePanel.Visibility

            return (hpv == Visibility.Visible) ? relevant.ToVisibility() : Visibility.Collapsed;
        }
    }

    public class DisplayedStrokeWidthSelectorBackgroundConverter : MarkupExtensionMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ToolBase   v0 = values[0] as ToolBase;
            Visibility? v1 = values[1] as Visibility?;
            bool       relevant = (v0 == null) ? false : ToolHasWidth.HasSafeRealWidth(v0);
            Visibility hpv      = (v1 == null) ? Visibility.Collapsed : v1.Value; // hidablePanel.Visibility

            return (hpv == Visibility.Visible) ? Visibility.Collapsed : relevant.ToVisibility();
        }
    }

    public class StrokeWidthSelectorVisibilityConverter : MarkupExtensionSingleValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            ToolBase activeTool = value as ToolBase;

            return ToolHasWidth.HasSafeRealWidth(activeTool).ToVisibility();
        }
    }
    #endregion
}
