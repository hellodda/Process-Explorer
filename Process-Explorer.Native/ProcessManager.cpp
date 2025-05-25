#include "pch.h"  
#include "ProcessManager.h"  
#include "Process.h"  
#include "CriticalSection.h"

using namespace System::Collections::Generic;


Native::ProcessManager::ProcessManager()
{
	m_cs = gcnew Native::CriticalSection();
    m_processes = gcnew Dictionary<DWORD, Process^>();
}

void Native::ProcessManager::InitializeProcessesList()
{
    m_cs->Lock();
 
    DWORD PIDs[1024], needed;
    if (EnumProcesses(PIDs, sizeof(PIDs), &needed)) {
        size_t count = needed / sizeof(DWORD);
        for (size_t i = 0; i < count; ++i) {
            if (PIDs[i] == 0) continue;
            else if (!m_processes->ContainsKey(PIDs[i]))
            {
                m_processes->Add(PIDs[i], gcnew Native::Process(PIDs[i]));
            }
        }
    }

    m_cs->Unlock();
}

IEnumerable<Native::Process^>^ Native::ProcessManager::GetActiveProcesses()
{  
    InitializeProcessesList();
    return m_processes->Values;
}

