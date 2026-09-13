using System.Runtime.InteropServices;
using VirtualControllerNative.Interop;

namespace VirtualControllerShared
{
    public delegate void GamepadEventHandler(GamepadButton state, int frameIndex);

    public class GamepadListener : IGamepadEventHandler
    {
        private IGamepadListener m_listener;
        private int m_index;

        public event GamepadEventHandler? OnGamepadEvent;

        public GamepadListener(int index)
        {
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
            OnGamepadEvent?.Invoke(state, frameIndex);
        }

        static class NativeMethods
        {
            [DllImport("VirtualControllerNative.dll")]
            public static extern void CreateGamepadListener(out IGamepadListener listener);
        }
    }
}
