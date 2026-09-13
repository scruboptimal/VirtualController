using VirtualControllerNative.Interop;

namespace VirtualControllerShared
{
    public class Recording
    {
        public record class RecordingFrame(int FrameIdx, GamepadButton State);

        private List<RecordingFrame> frames = new List<RecordingFrame>();
        private int firstFrameIndex = 0;

        public int FrameCount
        {
            get
            {
                return frames.Count > 0 ?
                    frames[frames.Count - 1].FrameIdx + 1 : 0;
            }
        }

        public IReadOnlyList<RecordingFrame> Frames { get => this.frames; }

        public void AddState(int frameIdx, GamepadButton state)
        {
            if (frames.Count == 0)
            {
                firstFrameIndex = frameIdx;
            }

            // Offset all frames by the index of the first frame
            this.frames.Add(new RecordingFrame(frameIdx - firstFrameIndex, state));
        }
    }
}
