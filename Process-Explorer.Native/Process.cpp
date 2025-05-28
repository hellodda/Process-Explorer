#include "pch.h"
#include "Process.h"
#include "Handle.h"
#include "ProcessInformation.h"
#include "CriticalSection.h"

typedef struct LANGANDCODEPAGE {
    WORD wLanguage;
    WORD wCodePage;
} *PLANGANDCODEPAGE, FAR *LPLANGANDCODEPAGE;

ULONGLONG FileTimeToULL (PFILETIME ft){
    if (!ft) throw gcnew System::NullReferenceException("FILETIME is null!");

    return (((ULONGLONG)ft->dwHighDateTime) << 32) | ft->dwLowDateTime;
};

Native::Process::Process(DWORD pid) : m_handle(gcnew Native::Handle(OpenProcess(PROCESS_ALL_ACCESS , FALSE, pid))), m_cs(gcnew Native::CriticalSection()), m_info(gcnew Native::ProcessInformation()), m_firstTimeMeasured(false)
{
    if (GetProcessId(m_handle) != 0)
        InitializeProcessTimes();
}
Native::Process::Process(Handle^ handle) : m_handle(handle), m_info(gcnew Native::ProcessInformation), m_cs(gcnew Native::CriticalSection()), m_firstTimeMeasured(false)
{
    if (GetProcessId(m_handle) != 0)
        InitializeProcessTimes();
}
Native::ProcessInformation^ Native::Process::GetProcessInformation()
{
    m_cs->Lock();

    if (!m_handle->IsValid()) throw gcnew System::NullReferenceException("Process handle is null.");

    m_info->PID = GetProcessId(m_handle);
	m_info->Name = GetProcessName();
    
	auto memCounters = GetProcessMemoryCounters();

	m_info->WorkingSet = memCounters.WorkingSetSize;
	m_info->PrivateBytes = memCounters.PrivateUsage;

	m_info->Description = GetProcessDescription();
	m_info->Company = GetProcessCompany();

    UpdateProcessCPUUsage();

	m_info->CpuUsage = m_cpuUsage;

    m_cs->Unlock();

    return m_info;
}

System::String^ Native::Process::GetProcessName()
{
    if (!m_handle->IsValid()) throw gcnew System::NullReferenceException("Process handle is null.");

    WCHAR exeName[MAX_PATH]{ 0 };
    if (GetProcessImageFileName(m_handle, exeName, MAX_PATH))
    {
        return gcnew System::String(PathFindFileNameW(exeName));
    }
}

System::String^ Native::Process::GetProcessDescription()
{
    if (!m_handle->IsValid()) throw gcnew System::NullReferenceException("Process handle is null.");

    WCHAR fullPath[MAX_PATH]{ 0 };
    if (GetModuleFileNameEx(m_handle, nullptr, fullPath, MAX_PATH))
    {
        DWORD dummy;
        DWORD size = GetFileVersionInfoSize(fullPath, &dummy);
        if (size)
        {
            PLANGANDCODEPAGE lpTranslate;
            BYTE* versionData = new BYTE[size];
            if (GetFileVersionInfo(fullPath, 0, size, versionData))
            {
                UINT cbTranslate = 0;
                if (VerQueryValue(versionData, L"\\VarFileInfo\\Translation", (LPVOID*)&lpTranslate, &cbTranslate))
                {
                    WCHAR subBlock[64];

                    swprintf_s(subBlock, L"\\StringFileInfo\\%04x%04x\\FileDescription", lpTranslate[0].wLanguage, lpTranslate[0].wCodePage);
                    LPVOID lpBuffer;
                    UINT dwBytes;
                    if (VerQueryValue(versionData, subBlock, &lpBuffer, &dwBytes))
                    {
                        System::String^ company = gcnew System::String((wchar_t*)lpBuffer);
                        delete[] versionData;
                        return company;
                    }
                }
            }
            delete[] versionData;
        }
    }
}

System::String^ Native::Process::GetProcessCompany()
{
	if (!m_handle->IsValid()) throw gcnew System::NullReferenceException("Process handle is null.");

    WCHAR fullPath[MAX_PATH]{ 0 };
    if (GetModuleFileNameEx(m_handle, nullptr, fullPath, MAX_PATH))
    {
        DWORD dummy;
        DWORD size = GetFileVersionInfoSize(fullPath, &dummy);
        if (size)
        {
            LANGANDCODEPAGE* lpTranslate;
            BYTE* versionData = new BYTE[size];
            if (GetFileVersionInfo(fullPath, 0, size, versionData))
            {
                UINT cbTranslate = 0;
                if (VerQueryValue(versionData, L"\\VarFileInfo\\Translation", (LPVOID*)&lpTranslate, &cbTranslate))
                {
                    WCHAR subBlock[64];

                    swprintf_s(subBlock, L"StringFileInfo\\%04x%04x\\CompanyName", lpTranslate[0].wLanguage, lpTranslate[0].wCodePage);
                    LPVOID lpBuffer;
                    UINT dwBytes;
                    if (VerQueryValue(versionData, subBlock, &lpBuffer, &dwBytes))
                    {
                        System::String^ description = gcnew System::String((wchar_t*)lpBuffer);
                        delete[] versionData;
                        return description;
                    }
                }
            }
            delete[] versionData; 
        }
    }
}

void Native::Process::UpdateProcessCPUUsage()
{
    if (!m_handle->IsValid())
        throw gcnew System::NullReferenceException("Process handle is null.");

    FILETIME ftSysIdle, ftSysKernel, ftSysUser;
    FILETIME ftProcCreation, ftProcExit, ftProcKernel, ftProcUser;

    if (!GetSystemTimes(&ftSysIdle, &ftSysKernel, &ftSysUser))
        return;

    if (!GetProcessTimes(m_handle, &ftProcCreation, &ftProcExit, &ftProcKernel, &ftProcUser))
        return;

    if (!m_firstTimeMeasured)
    {
        *m_prevSysKernelTime = ftSysKernel;
        *m_prevSysUserTime = ftSysUser;
        *m_prevProcKernelTime = ftProcKernel;
        *m_prevProcUserTime = ftProcUser;
        m_firstTimeMeasured = true;
        m_cpuUsage = 0.0;
        return;
    }

    ULONGLONG sysKernelDiff = FileTimeToULL(&ftSysKernel) - FileTimeToULL(m_prevSysKernelTime);
    ULONGLONG sysUserDiff = FileTimeToULL(&ftSysUser) - FileTimeToULL(m_prevSysUserTime);
    ULONGLONG procKernelDiff = FileTimeToULL(&ftProcKernel) - FileTimeToULL(m_prevProcKernelTime);
    ULONGLONG procUserDiff = FileTimeToULL(&ftProcUser) - FileTimeToULL(m_prevProcUserTime);

    ULONGLONG sysTimeDelta = sysKernelDiff + sysUserDiff;
    ULONGLONG procTimeDelta = procKernelDiff + procUserDiff;

    if (sysTimeDelta == 0)
        m_cpuUsage = 0.0;
    else
        m_cpuUsage = (double)(procTimeDelta) / (double)(sysTimeDelta) * 100.0;

    *m_prevSysKernelTime = ftSysKernel;
    *m_prevSysUserTime = ftSysUser;
    *m_prevProcKernelTime = ftProcKernel;
    *m_prevProcUserTime = ftProcUser;
}

void Native::Process::InitializeProcessTimes()
{
	m_prevProcKernelTime = new FILETIME();
	m_prevProcUserTime = new FILETIME();
	m_prevSysKernelTime = new FILETIME();
	m_prevSysUserTime = new FILETIME();
}

void Native::Process::DeinitializeProcessTimes()
{
    if (m_prevSysKernelTime) delete m_prevSysKernelTime;
    if (m_prevSysUserTime) delete m_prevSysUserTime;
    if (m_prevProcKernelTime) delete m_prevProcKernelTime;
    if (m_prevProcUserTime) delete m_prevProcUserTime;
}

Native::Handle^ Native::Process::GetHandle()
{
	if (!m_handle->IsValid()) throw gcnew System::NullReferenceException("Process handle is null.");

	return m_handle;
}

PROCESS_MEMORY_COUNTERS_EX Native::Process::GetProcessMemoryCounters()
{
    if (!m_handle->IsValid()) throw gcnew System::NullReferenceException("Process handle is null.");

    PROCESS_MEMORY_COUNTERS_EX memCounters = {};
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
		DeinitializeProcessTimes();
	}
}