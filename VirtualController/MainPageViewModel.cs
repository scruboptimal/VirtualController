using CommunityToolkit.Mvvm.ComponentModel;
using VirtualControllerShared;

namespace VirtualController
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty] private bool isRecording;
        [ObservableProperty] private string lastErrorText = string.Empty;
        [ObservableProperty] private Recording? displayRecording;
        [ObservableProperty] private VirtualController? controller;
        [ObservableProperty] private bool openInNewWindow;

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
            this.currentRecording.Frames.Add(new RecordingFrame(e.Item2, e.Item1));
        }
    }
}
