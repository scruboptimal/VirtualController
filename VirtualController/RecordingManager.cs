using System;
using System.Collections.Generic;
using System.Text;
using VirtualControllerShared;

namespace VirtualController
{
    internal class RecordingManager
    {
        private Recording recording = new Recording();
        private GamepadListener? listener;

        public static RecordingManager Start(GamepadListener listener)
        {
            return new RecordingManager(listener);
        }

        private RecordingManager(GamepadListener listener)
        {
            this.listener = listener;
            listener.OnGamepadEvent += OnGamepadEvent;
        }

        public Recording Stop()
        {
            if (this.listener != null)
            {
                this.listener.OnGamepadEvent -= OnGamepadEvent;
                this.listener = null;
            }

            return this.recording;
        }

        private void OnGamepadEvent(VirtualControllerNative.Interop.GamepadButton state, int frameIndex)
        {
            this.recording.AddState(frameIndex, state);
        }
    }
}
