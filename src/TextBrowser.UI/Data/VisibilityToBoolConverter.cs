using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace TextBrowser.UI.Data
{
    public class VisibilityToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Visibility? visibility = value as Visibility?;
            if (visibility == null || visibility == Visibility.Collapsed)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool? isChecked = value as bool?;
            if (isChecked == null || !isChecked.Value)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }
    }
}
