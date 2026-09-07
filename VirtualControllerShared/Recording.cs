using System;
using System.Collections.Generic;
using System.Text;
using VirtualControllerNative.Interop;

namespace VirtualControllerShared
{
    public class Recording
    {
    }

    public record class RecordingFrame(int frameIdx, List<GamepadButton> pressed);
}
