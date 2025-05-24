#include "pch.h"
#include "Process.h"
#include "Handle.h"
#include "ProcessInformation.h"
#include "CriticalSection.h"

struct LANGANDCODEPAGE {
    WORD wLanguage;
    WORD wCodePage;
};

ULONGLONG FileTimeToULL (FILETIME ft){
    return (((ULONGLONG)ft.dwHighDateTime) << 32) | ft.dwLowDateTime;
};

Native::Process::Process(DWORD pid)
{
	m_handle = gcnew Native::Handle(OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, pid));
	m_cs = gcnew Native::CriticalSection();
}
Native::Process::Process(Handle^ handle) : m_handle(handle), m_cs(gcnew Native::CriticalSection()) {}
Native::ProcessInformation^ Native::Process::GetProcessInformation()
{
    m_cs->Lock();

    if (!m_handle) throw gcnew System::NullReferenceException("Process handle is null.");

    auto info = gcnew ProcessInformation();

    info->PID = GetProcessId(m_handle);
	info->Name = GetProcessName();
    
	auto memCounters = GetProcessMemoryCounters();
	info->WorkingSet = memCounters.WorkingSetSize;
	info->PrivateBytes = memCounters.PrivateUsage;

	info->Description = GetProcessDescription();
	info->Company = GetProcessCompany();

    m_cs->Unlock();

    return info;
}

System::String^ Native::Process::GetProcessName()
{
    if (!m_handle) throw gcnew System::NullReferenceException("Process handle is null.");

    WCHAR exeName[MAX_PATH]{ 0 };
    if (GetProcessImageFileName(m_handle, exeName, MAX_PATH))
    {
        return gcnew System::String(PathFindFileNameW(exeName));
    }
}

System::String^ Native::Process::GetProcessDescription()
{
    if (!m_handle) throw gcnew System::NullReferenceException("Process handle is null.");

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
	if (!m_handle) throw gcnew System::NullReferenceException("Process handle is null.");

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

double Native::Process::GetProcessCPUUsage()
{
    if (!m_handle) throw gcnew System::NullReferenceException("Process handle is null.");

    FILETIME ftSysIdleStart, ftSysKernelStart, ftSysUserStart;
    FILETIME ftProcCreationStart, ftProcExitStart, ftProcKernelStart, ftProcUserStart;

    if (!GetSystemTimes(&ftSysIdleStart, &ftSysKernelStart, &ftSysUserStart))
        throw gcnew System::Exception("Failed to get system times.");

    if (!GetProcessTimes(m_handle, &ftProcCreationStart, &ftProcExitStart, &ftProcKernelStart, &ftProcUserStart))
        throw gcnew System::Exception("Failed to get process times.");

    Sleep(1000);

    FILETIME ftSysIdleEnd, ftSysKernelEnd, ftSysUserEnd;
    FILETIME ftProcCreationEnd, ftProcExitEnd, ftProcKernelEnd, ftProcUserEnd;

    if (!GetSystemTimes(&ftSysIdleEnd, &ftSysKernelEnd, &ftSysUserEnd))
        throw gcnew System::Exception("Failed to get system times.");

    if (!GetProcessTimes(m_handle, &ftProcCreationEnd, &ftProcExitEnd, &ftProcKernelEnd, &ftProcUserEnd))
        throw gcnew System::Exception("Failed to get process times.");

    ULONGLONG sysKernelStart = FileTimeToULL(ftSysKernelStart);
    ULONGLONG sysUserStart = FileTimeToULL(ftSysUserStart);
    ULONGLONG sysKernelEnd = FileTimeToULL(ftSysKernelEnd);
    ULONGLONG sysUserEnd = FileTimeToULL(ftSysUserEnd);

    ULONGLONG procKernelStart = FileTimeToULL(ftProcKernelStart);
    ULONGLONG procUserStart = FileTimeToULL(ftProcUserStart);
    ULONGLONG procKernelEnd = FileTimeToULL(ftProcKernelEnd);
    ULONGLONG procUserEnd = FileTimeToULL(ftProcUserEnd);

    ULONGLONG sysTotal = (sysKernelEnd - sysKernelStart) + (sysUserEnd - sysUserStart);
    ULONGLONG procTotal = (procKernelEnd - procKernelStart) + (procUserEnd - procUserStart);

    if (sysTotal == 0) return 0.0;

    return (double)(100.0 * procTotal) / sysTotal;
}


PROCESS_MEMORY_COUNTERS_EX Native::Process::GetProcessMemoryCounters()
{
    if (!m_handle) throw gcnew System::NullReferenceException("Process handle is null.");

    PROCESS_MEMORY_COUNTERS_EX memCounters = {};
    if (GetProcessMemoryInfo(m_handle, (PROCESS_MEMORY_COUNTERS*)&memCounters, sizeof(memCounters)))
    {
		return memCounters;
    }
}

Native::Process::~Process()
{
	if (m_handle->IsValid())
	{
		m_handle->Close();
	}
}