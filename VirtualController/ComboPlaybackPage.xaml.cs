using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VirtualControllerShared;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace VirtualController
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ComboPlaybackPage : Page
    {
        public ComboPlaybackPage(ComboPlaybackPageViewModel vm)
        {
            InitializeComponent();
            this.DataContext = this.ViewModel = vm;
        }

        public ComboPlaybackPageViewModel ViewModel { get; }
    }
}
