#include "pch.h"
#include "Process.h"
#include "Handle.h"
#include "ProcessInformation.h"
#include "CriticalSection.h"
#include "ProcessExplorer-Definitions.h"
#include "CpuUsageCalculator.h"

Native::Process::Process(DWORD pid)
    : m_handle(gcnew Native::Handle(OpenProcess(PROCESS_ALL_ACCESS , FALSE, pid))), m_cs(gcnew Native::CriticalSection()), m_info(gcnew Native::ProcessInformation()), m_usageCalculator(gcnew Native::CpuUsageCalculator) {}
Native::Process::Process(Handle^ handle)
    : m_handle(handle), m_info(gcnew Native::ProcessInformation), m_cs(gcnew Native::CriticalSection()), m_usageCalculator(gcnew Native::CpuUsageCalculator) {}

Native::ProcessInformation^ Native::Process::GetProcessInformation()
{
    m_cs->Lock();

    if (!m_handle->IsValid())
        throw gcnew System::NullReferenceException("Process handle is null.");

    if (!m_dataReceived)
    {
        m_info->PID = GetProcessId(m_handle);
        m_info->Name = GetProcessName();
        m_info->Description = GetProcessDescription();
        m_info->Company = GetProcessCompany();
        m_info->FilePath = GetProcessFilePath();
		m_dataReceived = true;
    }

	auto memCounters = GetProcessMemoryCounters();

	m_info->WorkingSet = memCounters.WorkingSetSize;
	m_info->PrivateBytes = memCounters.PrivateUsage;

	m_info->CpuUsage = m_usageCalculator->GetCpuUsage(m_handle);

    m_cs->Unlock();

    return m_info;
}

System::String^ Native::Process::GetProcessName()
{
    if (!m_handle->IsValid()) throw gcnew System::NullReferenceException("Process handle is null.");

    WCHAR exeName[MAX_PATH]{ 0 };
    if (GetProcessImageFileName(m_handle, exeName, MAX_PATH))
        return gcnew System::String(PathFindFileNameW(exeName));
}

System::String^ Native::Process::GetProcessDescription()
{
    return gcnew System::String(GetFileVersionString(m_handle, L"FileDescription"));
}

System::String^ Native::Process::GetProcessCompany()
{
    return gcnew System::String(GetFileVersionString(m_handle, L"CompanyName"));
}

System::String^ Native::Process::GetProcessFilePath()
{
    if (!m_handle->IsValid()) return "";

    TCHAR path[MAX_PATH]{ 0 };
    DWORD size{ MAX_PATH };
    if (!QueryFullProcessImageName(m_handle, 0, path, &size))
        return "";
 
    return gcnew System::String(path);
}


Native::Handle^ Native::Process::GetHandle()
{
	if (!m_handle->IsValid()) throw gcnew System::NullReferenceException("Process handle is null.");

	return m_handle;
}

PROCESS_MEMORY_COUNTERS_EX Native::Process::GetProcessMemoryCounters()
{
    if (!m_handle->IsValid()) throw gcnew System::NullReferenceException("Process handle is null.");

    PROCESS_MEMORY_COUNTERS_EX memCounters{};
    if (GetProcessMemoryInfo(m_handle, (PROCESS_MEMORY_COUNTERS*)&memCounters, sizeof(memCounters)))
    {
		return memCounters;
    }
}

void Native::Process::Terminate()
{
	if (!m_handle->IsValid()) throw gcnew System::NullReferenceException("Process handle is null.");

	if (!TerminateProcess(m_handle, 0))
	{
		throw gcnew System::Exception("Failed to terminate process.");
	}
}

Native::Process::~Process()
{
	if (m_handle->IsValid())
	{
        m_handle->Close();
	}
}