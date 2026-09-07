#pragma once

namespace VirtualControllerNative
{
    class GamepadListener : public RuntimeClass<RuntimeClassFlags<ClassicCom>, 
        IGamepadListener>
    {
    public:
        GamepadListener() = default;
        virtual HRESULT StartListening(IGamepadEventHandler* handler, int controllerIndex) override;
        virtual HRESULT StopListening() override;

    private:
        int m_frameIndex = 0;
        bool m_listening = false;
        std::thread m_listenerThread;
    };
}