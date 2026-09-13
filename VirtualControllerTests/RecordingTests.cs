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
            recording.AddFrame(5, GamepadButton.DPadDown);
            recording.AddFrame(10, GamepadButton.None);
            Assert.AreEqual(2, recording.Frames.Count);
            Assert.AreEqual(0, recording.Frames[0].FrameIdx);
            Assert.AreEqual(5, recording.Frames[1].FrameIdx);
        }
    }
}
