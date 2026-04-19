#include "pch.h"
#include "GamepadListener.h"
#if __has_include("GamepadListener.g.cpp")
#include "GamepadListener.g.cpp"
#endif

namespace winrt::NativeInput::implementation
{
    int32_t GamepadListener::MyProperty()
    {
        throw hresult_not_implemented();
    }

    void GamepadListener::MyProperty(int32_t /*value*/)
    {
        throw hresult_not_implemented();
    }
}
