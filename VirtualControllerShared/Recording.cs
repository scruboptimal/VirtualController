using System.Text.Json;
using System.Text.Json.Serialization;
using VirtualControllerNative.Interop;

namespace VirtualControllerShared
{
    public class Recording
    {
        public record class RecordingFrame(int FrameIdx, GamepadButton State);

        private int firstFrameIndex = 0;

        [JsonIgnore]
        public int FrameCount
        {
            get
            {
                return this.Frames.Count > 0 ?
                    this.Frames[this.Frames.Count - 1].FrameIdx + 1 : 0;
            }
        }

        public List<RecordingFrame> Frames { get; set; } = new List<RecordingFrame>();

        public void AddState(int frameIdx, GamepadButton state)
        {
            if (this.Frames.Count == 0)
            {
                firstFrameIndex = frameIdx;
            }

            // Offset all frames by the index of the first frame
            this.Frames.Add(new RecordingFrame(frameIdx - firstFrameIndex, state));
        }
    }
}
