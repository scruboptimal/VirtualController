using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using VirtualControllerShared;
using VirtualControllerNative.Interop;

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
    }
}
