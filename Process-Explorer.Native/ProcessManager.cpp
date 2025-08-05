#include "pch.h"  
#include "ProcessManager.h"  
#include "Process.h"  
#include "CriticalSection.h"
#include "Handle.h"

using namespace System::Collections::Generic;


Native::ProcessManager::ProcessManager()
{
    m_processes = gcnew Dictionary<DWORD, Process^>();
}

System::Threading::Tasks::Task^ Native::ProcessManager::InitializeProcessesListAsync()
{
    DWORD PIDs[1024];
    DWORD needed = 0;

    if (!EnumProcesses(PIDs, sizeof(PIDs), &needed))
        throw gcnew System::Exception("Failed to get process pids.");

    const size_t count = needed / sizeof(DWORD);

    auto newPIDs = gcnew HashSet<DWORD>(count);
    for (size_t i = 0; i < count; ++i)
    {
        if (PIDs[i] != 0)
            newPIDs->Add(PIDs[i]);
    }

    auto oldPIDs = gcnew List<DWORD>(m_processes->Keys);
    for each (auto oldPid in oldPIDs)
    {
        if (!newPIDs->Contains(oldPid))
        {
            m_processes->Remove(oldPid);
        }
    }

    for each (auto pid in newPIDs)
    {
        if (!m_processes->ContainsKey(pid))
        {
            Native::Handle^ hProcess = gcnew Native::Handle(OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, FALSE, pid));
            if (hProcess->IsValid())
            {
                m_processes->Add(pid, gcnew Native::Process(hProcess));
            }
        }
    }

    return System::Threading::Tasks::Task::CompletedTask;
}


System::Threading::Tasks::Task<IEnumerable<Native::Process^>^>^ Native::ProcessManager::GetActiveProcessesAsync()
{  
	InitializeProcessesListAsync();

    IEnumerable<Native::Process^>^ result = m_processes->Values;

    return System::Threading::Tasks::Task::FromResult(result);
}

