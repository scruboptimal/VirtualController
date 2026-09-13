using CommunityToolkit.Mvvm.ComponentModel;
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

        public MainPageViewModel()
        {
            this.gamepadListener = new GamepadListener(0);
            this.gamepadListener.StartListening();
        }

        public void OpenController(string svgPath)
        {
            this.Controller = new VirtualControllerDisplay(svgPath, this.gamepadListener);
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
            }

            this.IsRecording = !this.IsRecording;
        }
    }
}
