using Microsoft.UI.Xaml.Media;

namespace VirtualController
{
    public static class Helpers
    {
        public static SolidColorBrush GetBrushForColor(System.Drawing.Color color)
        {
            return new SolidColorBrush(Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }
}
