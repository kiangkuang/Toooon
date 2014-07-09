using System.Windows;
using System.Windows.Controls.Primitives;

namespace ToonGui
{
	// Binding to a ToggleButton.IsChecked property will be lost after clicking the button
	// so we need a solution:
	//     http://stackoverflow.com/questions/3771431/oneway-binding-for-togglebuttons-ischecked-property-in-wpf
	public class ToggleButtonExtender
	{
		public static readonly DependencyProperty IsCheckedProperty =
			DependencyProperty.RegisterAttached("IsChecked", typeof(bool), typeof(ToggleButtonExtender), new PropertyMetadata(default(bool), (d, e) =>
			{
				ToggleButton button = d as ToggleButton;
				bool value = (bool)e.NewValue;

				if (button != null)
				{
					button.IsChecked = value;
				}
			}));

		public static bool GetIsChecked(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsCheckedProperty);
		}
		public static void SetIsChecked(DependencyObject obj, bool value)
		{
			obj.SetValue(IsCheckedProperty, value);
		}
	}
}
