using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualControllerShared;
using VirtualControllerNative.Interop;
using System.Text.Json;

namespace VirtualControllerTests
{
    [TestClass]
    public partial class RecordingTests
    {
        [TestMethod]
        public void OffsetsFromFirstFrame()
        {
            var recording = new Recording();
            recording.AddState(5, GamepadButton.DPadDown);
            recording.AddState(10, GamepadButton.None);
            Assert.AreEqual(2, recording.Frames.Count);
            Assert.AreEqual(0, recording.Frames[0].FrameIdx);
            Assert.AreEqual(5, recording.Frames[1].FrameIdx);
        }

        [TestMethod]
        public void FrameCount()
        {
            var recording = new Recording();
            Assert.AreEqual(0, recording.FrameCount);

            recording.AddState(5, GamepadButton.DPadDown);
            Assert.AreEqual(1, recording.FrameCount); // One frame at idx 0

            recording.AddState(10, GamepadButton.DPadDown);
            Assert.AreEqual(6, recording.FrameCount); // Frames 0-5 inc.
        }

        [TestMethod]
        public void Serialization()
        {
            var r1 = new Recording();
            r1.AddState(5, GamepadButton.DPadDown);

            string json = JsonSerializer.Serialize(r1);
            Assert.AreEqual(@"{""Frames"":[{""FrameIdx"":0,""State"":32}]}", json);

            var r2 = JsonSerializer.Deserialize<Recording>(json);
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1.FrameCount, r2.FrameCount);
            Assert.AreEqual(r1.Frames[0], r2.Frames[0]);
        }
    }
}
