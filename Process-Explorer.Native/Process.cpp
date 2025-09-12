#include "pch.h"
#include "Process.h"
#include "Handle.h"
#include "ProcessInformation.h"
#include "ProcessExplorer-Definitions.h"
#include "CpuUsageCalculator.h"

Native::Process::Process()
    : m_handle(nullptr), m_usageCalculator(nullptr) {}

Native::Process::Process(DWORD pid)
    : m_handle(gcnew Native::Handle(OpenProcess(PROCESS_ALL_ACCESS , FALSE, pid))), m_usageCalculator(gcnew Native::CpuUsageCalculator) {}
Native::Process::Process(Handle^ handle)
    : m_handle(handle), m_usageCalculator(gcnew Native::CpuUsageCalculator) {}

DWORD Native::Process::GetProcessId()
{
    return ::GetProcessId(m_handle);
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

double Native::Process::GetCpuUsage()
{
    return m_usageCalculator->GetCpuUsage(m_handle);
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