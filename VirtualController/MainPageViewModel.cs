using CommunityToolkit.Mvvm.ComponentModel;
using VirtualControllerShared;

namespace VirtualController
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty] public partial bool IsRecording { get; set; }
        [ObservableProperty] public partial Recording? DisplayRecording { get; set; }
        [ObservableProperty] public partial VirtualController? Controller { get; set; }
        [ObservableProperty] public partial bool OpenInNewWindow { get; set; }

        Recording? currentRecording;

        public void OpenController(string svgPath)
        {
            this.Controller = new VirtualController(svgPath);
        }

        public void ToggleRecording()
        {
            if (this.Controller == null)
            {
                return;
            }

            if (!this.IsRecording)
            {
                this.currentRecording = new Recording();
                this.Controller.ButtonsChanged += Controller_ButtonsChanged;
            }
            else
            {
                this.Controller.ButtonsChanged -= Controller_ButtonsChanged;
                this.DisplayRecording = this.currentRecording;
            }

            this.IsRecording = !this.IsRecording;
        }

        private void Controller_ButtonsChanged(object? sender, (VirtualControllerNative.Interop.GamepadButton, int) e)
        {
            this.currentRecording?.AddFrame(e.Item2, e.Item1);
        }
    }
}
