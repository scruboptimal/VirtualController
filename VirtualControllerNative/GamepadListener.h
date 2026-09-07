#pragma once

namespace VirtualControllerNative
{
    enum class GamepadButton
    {
        A = 0,
        B = 1,
        X = 2,
        Y = 3,
        DPadUp = 4,
        DPadDown = 5,
        DPadLeft = 6,
        DPadRight = 7,
        Start = 8,
        Back = 9,
        LeftShoulder = 10,
        RightShoulder = 11,
        LeftTrigger = 12,
        RightTrigger = 13,
        LeftThumb = 14,
        RightThumb = 15,
    };

    using GamepadEventHandler = void(*)(GamepadButton button, bool isPressed);

    class GamepadListener
    {
    public:
        HRESULT StartListening(GamepadEventHandler handler, int32_t controllerIndex);
        HRESULT StopListening();

    private:
        bool m_listening = false;
        std::thread m_listenerThread;
    };
}