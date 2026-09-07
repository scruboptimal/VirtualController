using System.Runtime.InteropServices;
using VirtualControllerNative.Interop;

namespace VirtualControllerShared
{
    public delegate void GamepadEventHandler(GamepadButton state, int frameIndex);

    public class GamepadListener : IGamepadEventHandler
    {
        private IGamepadListener m_listener;
        private GamepadEventHandler m_handler;
        private int m_index;

        public GamepadListener(GamepadEventHandler handler, int index)
        {
            m_handler = handler;
            m_index = index;
            NativeMethods.CreateGamepadListener(out m_listener);
        }

        public void StartListening()
        {
            m_listener.StartListening(this, m_index);
        }

        public void StopListening()
        {
            m_listener.StopListening();
        }

        public void HandleGamepadState(GamepadButton state, int frameIndex)
        {
            m_handler(state, frameIndex);
        }

        static class NativeMethods
        {
            [DllImport("VirtualControllerNative.dll")]
            public static extern void CreateGamepadListener(out IGamepadListener listener);
        }
    }
}
