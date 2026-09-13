using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Data;
using System;

namespace VirtualController
{
    public partial class BooleanToObjectConverter : ObservableObject, IValueConverter
    {
        [ObservableProperty] public partial object? TrueValue { get; set; }
        [ObservableProperty] public partial object? FalseValue { get; set; }

        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool bVal)
            {
                return null;
            }

            return bVal ? this.TrueValue : this.FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
