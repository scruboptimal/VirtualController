#pragma once

#include "GamepadListener.g.h"

namespace winrt::NativeInput::implementation
{
    struct GamepadListener : GamepadListenerT<GamepadListener>
    {
        GamepadListener() = default;

        int32_t MyProperty();
        void MyProperty(int32_t value);
    };
}

namespace winrt::NativeInput::factory_implementation
{
    struct GamepadListener : GamepadListenerT<GamepadListener, implementation::GamepadListener>
    {
    };
}
