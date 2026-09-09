#include "pch.h"
#include "GamepadListener_h.h"
#include "GamepadListener.h"

#include <Xinput.h>

namespace VirtualControllerNative
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

    static int frameIdx = 0;
    HRESULT GamepadListener::StartListening(IGamepadEventHandler* handler, int controllerIndex)
    {
        if (!handler) { return E_POINTER; }
        if (m_listening) { return S_OK; }

        m_frameIndex = 0;
        m_listening = true;
        m_listenerThread = std::thread(
            [this, controllerIndex, handler]()
            {
                XINPUT_STATE prevState = {};
                while (m_listening)
                {
                    XINPUT_STATE state = {};
                    if (XInputGetState(controllerIndex, &state) == ERROR_SUCCESS &&
                        state.dwPacketNumber != prevState.dwPacketNumber)
                    {
                        GamepadButton result = GamepadButton::None;
                        for (const auto& pair : ButtonMap)
                        {
                            const GamepadButton button = pair.first;
                            const DWORD mask = pair.second;

                            if (state.Gamepad.wButtons & mask)
                            {
                                result = (GamepadButton)(result | button);
                            }
                        }

                        if (state.Gamepad.bLeftTrigger > XINPUT_GAMEPAD_TRIGGER_THRESHOLD)
                        {
                            result = (GamepadButton)(result | GamepadButton::LeftTrigger);
                        }

                        if (state.Gamepad.bRightTrigger > XINPUT_GAMEPAD_TRIGGER_THRESHOLD)
                        {
                            result = (GamepadButton)(result | GamepadButton::RightTrigger);
                        }

                        if (result != GamepadButton::None)
                        {
                            (void)handler->HandleGamepadState(result, m_frameIndex);
                        }

                        prevState = state;
                    }

                    m_frameIndex++;
                    Sleep(16); // Polling interval
                }
            });

        return S_OK;
    }

    HRESULT GamepadListener::StopListening()
    {
        if (!m_listening)
        {
            return S_OK;
        }

        m_listening = false;

        if (m_listenerThread.joinable())
        {
            m_listenerThread.join();
        }

        return S_OK;
    }
}

EXTERN_C
__declspec(dllexport)
HRESULT CreateGamepadListener(IGamepadListener** listener)
{
    Microsoft::WRL::ComPtr<VirtualControllerNative::GamepadListener> tmpListener;
    RETURN_IF_FAILED(MakeAndInitialize<VirtualControllerNative::GamepadListener>(&tmpListener));

    *listener = tmpListener.Detach();
    return S_OK;
}
