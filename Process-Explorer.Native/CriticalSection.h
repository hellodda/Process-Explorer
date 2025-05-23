#pragma once

namespace Native
{
    public ref class CriticalSection {
    private:
        LPCRITICAL_SECTION m_cs;
        bool m_initialized;

    public:
        CriticalSection();
        ~CriticalSection();
        !CriticalSection();

        void Initialize();
        void Lock();
        void Unlock();

        operator CRITICAL_SECTION* () { return m_cs; }
    };
}
