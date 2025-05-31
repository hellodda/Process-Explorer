#include "pch.h"  
#include "ProcessManager.h"  
#include "Process.h"  
#include "CriticalSection.h"

using namespace System::Collections::Generic;


Native::ProcessManager::ProcessManager()
{
    m_processes = gcnew Dictionary<DWORD, Process^>();
}

System::Threading::Tasks::Task^ Native::ProcessManager::InitializeProcessesListAsync()
{
    DWORD PIDs[1024], needed;
    if (EnumProcesses(PIDs, sizeof(PIDs), &needed)) {
        size_t count = needed / sizeof(DWORD);
        for (size_t i = 0; i < count; ++i) {
            
            auto newPIDs = gcnew HashSet<DWORD>();
            for (size_t i = 0; i < count; ++i)
            {
                if (PIDs[i] != 0)
                    newPIDs->Add(PIDs[i]);
            }

			auto oldPIDs = gcnew HashSet<DWORD>(m_processes->Keys);
            for each(auto pid in oldPIDs)
            {
                if (!newPIDs->Contains(pid))
                {
					m_processes->Remove(pid);
                }
			}

            if (PIDs[i] == 0) continue;

            else if (!m_processes->ContainsKey(PIDs[i]))
            {
                m_processes->Add(PIDs[i], gcnew Native::Process(PIDs[i]));
            }
        }
    }

	return System::Threading::Tasks::Task::CompletedTask;
}

System::Threading::Tasks::Task<IEnumerable<Native::Process^>^>^ Native::ProcessManager::GetActiveProcessesAsync()
{  
	InitializeProcessesListAsync()->Wait();

    IEnumerable<Native::Process^>^ result = m_processes->Values;

    return System::Threading::Tasks::Task::FromResult(result);
}

