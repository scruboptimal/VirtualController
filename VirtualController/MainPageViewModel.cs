using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using VirtualControllerShared;

namespace VirtualController
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty] private bool isRecording;
        [ObservableProperty] private bool canRecord;
        [ObservableProperty] private string lastErrorText = string.Empty;

        public ObservableCollection<Recording> Recordings { get; set; } = new ObservableCollection<Recording>();
    }
}
