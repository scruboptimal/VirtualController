#pragma once

#include "GamepadListener.g.h"

namespace winrt::NativeInput::implementation
{
    struct GamepadListener : GamepadListenerT<GamepadListener>
    {
        GamepadListener() = default;

        HRESULT StartListening(GamepadEventHandler handler, int32_t controllerIndex);
        HRESULT StopListening();

    private:
        std::thread m_listenerThread;
        std::atomic<bool> m_listening{ false };
    };
}

namespace winrt::NativeInput::factory_implementation
{
    struct GamepadListener : GamepadListenerT<GamepadListener, implementation::GamepadListener>
    {
    };
}
