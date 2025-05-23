#include "pch.h"
#include "CriticalSection.h"

Native::CriticalSection::CriticalSection() : m_cs(nullptr), m_initialized(false)
{
    Initialize();
}

Native::CriticalSection::~CriticalSection()
{
    if (m_cs)
    {
        DeleteCriticalSection(m_cs);
        delete m_cs;
        m_cs = nullptr;
        m_initialized = false;
    }
}

Native::CriticalSection::!CriticalSection()
{
    this->~CriticalSection();
}

void Native::CriticalSection::Initialize()
{
    if (m_initialized)
        return;

    m_cs = new CRITICAL_SECTION{};
    if (!InitializeCriticalSectionEx(m_cs, 0, 0)) {
        delete m_cs;
        m_cs = nullptr;
        throw gcnew System::TypeLoadException("Failed to initialize critical section");
    }

    m_initialized = true;
}

void Native::CriticalSection::Lock()
{
    if (!m_initialized)
        throw gcnew System::InvalidOperationException("CriticalSection not initialized");

    EnterCriticalSection(m_cs);
}

void Native::CriticalSection::Unlock()
{
    if (!m_cs)
        throw gcnew System::NullReferenceException("CriticalSection is not initialized");

    LeaveCriticalSection(m_cs);
}
