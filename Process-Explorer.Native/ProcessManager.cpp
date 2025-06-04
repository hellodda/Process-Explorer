#include "pch.h"  
#include "ProcessManager.h"  
#include "Process.h"  
#include "ProcessEx.h"
#include "CriticalSection.h"
#include "Handle.h"
#include "ProcessInformation.h"
#include "ProcessExplorer-Definitions.h"
#include "Allocator.h"

using namespace System::Collections::Generic;


Native::ProcessManager::ProcessManager()
{
    m_processes = gcnew Dictionary<DWORD, Process^>(350);
	m_processesEx = gcnew Dictionary<DWORD, ProcessEx^>(350);
}

System::Threading::Tasks::Task^ Native::ProcessManager::InitializeProcessesListAsync()
{
    DWORD PIDs[1024]{};
    DWORD needed{ 0 };

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


System::Threading::Tasks::Task^ Native::ProcessManager::NtInitializeProcessesListAsync()
{
    ULONG BUFFER_SIZE = 0;
    PBYTE buffer = nullptr;

    
    auto status = NtQuerySystemInformation(SystemProcessInformation, nullptr, 0, &BUFFER_SIZE);
    /*if (status != STATUS_INFO_LENGTH_MISMATCH)
        throw gcnew System::Exception("NtQuerySystemInformation.");*/

    buffer = Allocator::Allocate<BYTE>(BUFFER_SIZE);
    if (!buffer)
        throw gcnew System::OutOfMemoryException("Failed to allocate memory for process buffer.");

    try
    {
        status = NtQuerySystemInformation(SystemProcessInformation, buffer, BUFFER_SIZE, nullptr);
        if (!NT_SUCCESS(status))
            throw gcnew System::Exception("NtQuerySystemInformation returned an error.");

        PSYSTEM_PROCESS_INFORMATION_EX pspi_ex = reinterpret_cast<PSYSTEM_PROCESS_INFORMATION_EX>(buffer);

        auto newPids = gcnew HashSet<DWORD>();
        auto oldPids = gcnew List<DWORD>(m_processesEx->Keys);

        while (true)
        {
            DWORD pid = static_cast<DWORD>(reinterpret_cast<ULONG_PTR>(pspi_ex->UniqueProcessId));
            newPids->Add(pid);

            if (!m_processesEx->ContainsKey(pid))
            {
                m_processesEx->Add(pid, gcnew ProcessEx(pspi_ex));
            }
            else
            {
                m_processesEx[pid]->SaveOrUpdateProcessInformation(pspi_ex);
            }

            if (pspi_ex->NextEntryOffset == 0)
                break;

            pspi_ex = reinterpret_cast<PSYSTEM_PROCESS_INFORMATION_EX>(
                reinterpret_cast<PBYTE>(pspi_ex) + pspi_ex->NextEntryOffset);
        }

        for each (auto oldPid in oldPids)
        {
            if (!newPids->Contains(oldPid))
            {
                m_processesEx->Remove(oldPid);
            }
        }
    }
    finally
    {
        Allocator::Deallocate(buffer);
    }

    return System::Threading::Tasks::Task::CompletedTask;
}



System::Threading::Tasks::Task<IEnumerable<Native::Process^>^>^ Native::ProcessManager::GetActiveProcessesAsync()
{  
	InitializeProcessesListAsync()->Wait();
	

    IEnumerable<Native::Process^>^ result = m_processes->Values;

    return System::Threading::Tasks::Task::FromResult(result);
}

System::Threading::Tasks::Task<IEnumerable<Native::ProcessEx^>^>^ Native::ProcessManager::NtGetActiveProcessesAsync()
{
	NtInitializeProcessesListAsync()->Wait();

    IEnumerable<Native::ProcessEx^>^ result = m_processesEx->Values;

	return System::Threading::Tasks::Task::FromResult(result);
}

