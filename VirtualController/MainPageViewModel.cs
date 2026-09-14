using CommunityToolkit.Mvvm.ComponentModel;
using System;
using VirtualControllerShared;

namespace VirtualController
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty] public partial bool IsRecording { get; set; }
        [ObservableProperty] public partial Recording? DisplayRecording { get; set; }
        [ObservableProperty] public partial VirtualControllerDisplay? Controller { get; set; }
        [ObservableProperty] public partial bool OpenInNewWindow { get; set; }

        private GamepadListener gamepadListener;
        private RecordingManager? recordingManager;
        private VirtualControllerData? controllerData;

        public MainPageViewModel()
        {
            this.gamepadListener = new GamepadListener(0);
            this.gamepadListener.StartListening();
        }

        public void OpenController(string svgPath)
        {
            this.controllerData = VirtualControllerData.LoadFromSvg(svgPath);
            this.Controller = new VirtualControllerDisplay(this.controllerData, this.gamepadListener);
        }

        public void ToggleRecording()
        {
            if (this.gamepadListener == null)
            {
                return;
            }

            if (this.recordingManager == null)
            {
                this.recordingManager = RecordingManager.Start(this.gamepadListener);
            }
            else
            {
                this.DisplayRecording = this.recordingManager.Stop();
                this.recordingManager = null;
            }

            this.IsRecording = !this.IsRecording;
        }

        public ComboPlaybackPageViewModel CreatePlaybackVM()
        {
            if (this.controllerData == null || 
                this.DisplayRecording == null)
            {
                throw new InvalidOperationException();
            }

            return new ComboPlaybackPageViewModel(this.controllerData, this.DisplayRecording, this.gamepadListener);
        }
    }
}
