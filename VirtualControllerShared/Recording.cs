using VirtualControllerNative.Interop;

namespace VirtualControllerShared
{
    public class Recording
    {
        public List<RecordingFrame> Frames = new List<RecordingFrame>();
    }

    public record class RecordingFrame(int frameIdx, GamepadButton state);
}
