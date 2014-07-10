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
using System.Windows.Controls.Primitives;

namespace ToonGui
{
	public class ImageToggleButton : ToggleButton
	{

		public ImageToggleButton()
		{
			this.IsTabStop = false;
			this.DefaultStyleKey = typeof(ImageToggleButton);
		}

		#region IconName: Custom DependencyProperty Implementation
		// Reference: http://stackoverflow.com/questions/3319527/how-to-bind-to-a-custom-property-in-a-silverlight-custom-control
		public string IconName
		{
			get { return (string)GetValue(IconNameProperty); }
			set { SetValue(IconNameProperty, value); }
		}
		public static readonly DependencyProperty IconNameProperty =
			DependencyProperty.Register("IconName", typeof(string), typeof(ImageToggleButton), new PropertyMetadata(default(string), (d, e) =>
			{
				ImageToggleButton button = d as ImageToggleButton;
				string iconName = e.NewValue as string;

				ImageSourceConverter isc = new ImageSourceConverter();

				button.ImageNormal = (ImageSource)isc.ConvertFromString(string.Format("Images/{0}_Off.png", iconName));
				button.ImageMouseOver = (ImageSource)isc.ConvertFromString(string.Format("Images/{0}_Over.png", iconName));
				button.ImagePressed = (ImageSource)isc.ConvertFromString(string.Format("Images/{0}_Down.png", iconName));
				button.ImageChecked = (ImageSource)isc.ConvertFromString(string.Format("Images/{0}_Depres.png", iconName));
				button.ImageDisabled = (ImageSource)isc.ConvertFromString(string.Format("Images/{0}_Greyed.png", iconName));
			}));
		#endregion

		public ImageSource ImageNormal
		{
			get { return (ImageSource)GetValue(ImageNormalProperty); }
			set { SetValue(ImageNormalProperty, value); }
		}
		public static readonly DependencyProperty ImageNormalProperty =
			DependencyProperty.Register("ImageNormal", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(default(ImageSource)));

		public ImageSource ImageMouseOver
		{
			get { return (ImageSource)GetValue(ImageMouseOverProperty); }
			set { SetValue(ImageMouseOverProperty, value); }
		}
		public static readonly DependencyProperty ImageMouseOverProperty =
			DependencyProperty.Register("ImageMouseOver", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(default(ImageSource)));

		public ImageSource ImagePressed
		{
			get { return (ImageSource)GetValue(ImagePressedProperty); }
			set { SetValue(ImagePressedProperty, value); }
		}
		public static readonly DependencyProperty ImagePressedProperty =
			DependencyProperty.Register("ImagePressed", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(default(ImageSource)));

		public ImageSource ImageChecked
		{
			get { return (ImageSource)GetValue(ImageCheckedProperty); }
			set { SetValue(ImageCheckedProperty, value); }
		}
		public static readonly DependencyProperty ImageCheckedProperty =
			DependencyProperty.Register("ImageChecked", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(default(ImageSource)));

		public ImageSource ImageDisabled
		{
			get { return (ImageSource)GetValue(ImageDisabledProperty); }
			set { SetValue(ImageDisabledProperty, value); }
		}
		public static readonly DependencyProperty ImageDisabledProperty =
			DependencyProperty.Register("ImageDisabled", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(default(ImageSource)));
	}
}
