using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Data;
using System;

namespace VirtualController
{
    public partial class BooleanToObjectConverter : ObservableObject, IValueConverter
    {
        [ObservableProperty] private object? trueValue;
        [ObservableProperty] private object? falseValue;

        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool bVal)
            {
                return null;
            }

            return bVal ? this.trueValue : this.falseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
