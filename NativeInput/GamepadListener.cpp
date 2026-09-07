#include "pch.h"
#include "GamepadListener.h"
#if __has_include("GamepadListener.g.cpp")
#include "GamepadListener.g.cpp"
#endif

#include <Xinput.h>

namespace winrt::NativeInput::implementation
{
    // Button mask mapping
    const std::unordered_map<GamepadButton, int32_t> ButtonMap = {
        {GamepadButton::DPadUp,         XINPUT_GAMEPAD_DPAD_UP},
        {GamepadButton::DPadDown,       XINPUT_GAMEPAD_DPAD_DOWN},
        {GamepadButton::DPadLeft,       XINPUT_GAMEPAD_DPAD_LEFT},
        {GamepadButton::DPadRight,      XINPUT_GAMEPAD_DPAD_RIGHT},
        {GamepadButton::Start,          XINPUT_GAMEPAD_START},
        {GamepadButton::Back,           XINPUT_GAMEPAD_BACK},
        {GamepadButton::LeftShoulder,   XINPUT_GAMEPAD_LEFT_SHOULDER},
        {GamepadButton::RightShoulder,  XINPUT_GAMEPAD_RIGHT_SHOULDER},
        {GamepadButton::LeftThumb,      XINPUT_GAMEPAD_LEFT_THUMB},
        {GamepadButton::RightThumb,     XINPUT_GAMEPAD_RIGHT_THUMB},
        {GamepadButton::A,              XINPUT_GAMEPAD_A},
        {GamepadButton::B,              XINPUT_GAMEPAD_B},
        {GamepadButton::X,              XINPUT_GAMEPAD_X},
        {GamepadButton::Y,              XINPUT_GAMEPAD_Y}
    };

    HRESULT GamepadListener::StartListening(GamepadEventHandler handler, int32_t controllerIndex)
    {
        if (!handler) { return E_POINTER; }
        if (m_listening) { return S_OK; }

        m_listening = true;
        m_listenerThread = std::thread(
            [this, controllerIndex, handler]()
            {
                XINPUT_STATE prevState = {};
                while (m_listening)
                {
                    XINPUT_STATE state = {};
                    if (XInputGetState(controllerIndex, &state) == ERROR_SUCCESS)
                    {
                        for (const auto& pair : ButtonMap)
                        {
                            const GamepadButton button = pair.first;
                            const DWORD mask = pair.second;

                            const bool wasPressed = (prevState.Gamepad.wButtons & mask) != 0;
                            const bool isPressed = (state.Gamepad.wButtons & mask) != 0;
                            if (wasPressed != isPressed)
                            {
                                handler(button, isPressed);
                            }
                        }

                        {
                            // Left trigger
                            const bool wasPressed = prevState.Gamepad.bLeftTrigger > XINPUT_GAMEPAD_TRIGGER_THRESHOLD;
                            const bool isPressed = state.Gamepad.bLeftTrigger > XINPUT_GAMEPAD_TRIGGER_THRESHOLD;
                            if (wasPressed != isPressed)
                            {
                                handler(GamepadButton::LeftTrigger, isPressed);
                            }
                        }
                        {
                            // Right trigger
                            const bool wasPressed = prevState.Gamepad.bRightTrigger > XINPUT_GAMEPAD_TRIGGER_THRESHOLD;
                            const bool isPressed = state.Gamepad.bRightTrigger > XINPUT_GAMEPAD_TRIGGER_THRESHOLD;
                            if (wasPressed != isPressed)
                            {
                                handler(GamepadButton::RightTrigger, isPressed);
                            }
                        }

                        prevState = state;
                    }

                    Sleep(10); // Polling interval
                }
            });

        return S_OK;
    }

    HRESULT GamepadListener::StopListening()
    {
        if (!m_listening) return S_FALSE;
        m_listening = false;

        if (m_listenerThread.joinable())
        {
            m_listenerThread.join();
        }

        return S_OK;
    }
}
