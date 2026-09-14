using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using VirtualControllerShared;

namespace VirtualController
{
    public partial class ComboPlaybackPageViewModel : ObservableObject
    {
        [ObservableProperty] public partial Recording Recording { get; set; }
        [ObservableProperty] public partial VirtualControllerDisplay Controller { get; set; }

        public ComboPlaybackPageViewModel(VirtualControllerData controllerData, Recording recording, GamepadListener listener)
        {
            Controller = new VirtualControllerDisplay(controllerData, listener);
            this.Recording = recording;
        }
    }
}
