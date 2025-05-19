#include "pch.h"
#include "Process.h"
#include "Handle.h"
#include "ProcessInformation.h"


struct LANGANDCODEPAGE {
    WORD wLanguage;
    WORD wCodePage;
};

Native::Process::Process(DWORD pid)
{
	m_handle = gcnew Native::Handle(OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, pid));
}
Native::Process::Process(Handle^ handle) : m_handle(handle) {}
Native::ProcessInformation^ Native::Process::GetProcessInformation()
{
    auto info = gcnew ProcessInformation();

    DWORD pid = GetProcessId(m_handle);
    info->PID = pid;

    WCHAR exeName[MAX_PATH] = { 0 };
    if (GetProcessImageFileName(m_handle, exeName, MAX_PATH))
    {
        info->Name = gcnew System::String(PathFindFileNameW(exeName));
    }

    PROCESS_MEMORY_COUNTERS_EX memCounters = {};
    if (GetProcessMemoryInfo(m_handle, (PROCESS_MEMORY_COUNTERS*)&memCounters, sizeof(memCounters)))
    {
        info->WorkingSet = (DWORD)memCounters.WorkingSetSize;
        info->PrivateBytes = (DWORD)memCounters.PrivateUsage;
    }

    WCHAR fullPath[MAX_PATH] = { 0 };
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
                        info->Description = gcnew System::String((wchar_t*)lpBuffer);

                    swprintf_s(subBlock, L"\\StringFileInfo\\%04x%04x\\CompanyName", lpTranslate[0].wLanguage, lpTranslate[0].wCodePage);
                    if (VerQueryValue(versionData, subBlock, &lpBuffer, &dwBytes))
                        info->Company = gcnew System::String((wchar_t*)lpBuffer);
                }
            }
            if (versionData)
			{
				delete[] versionData;
			}
        }
    }
    return info;
}

Native::Process::~Process()
{
	if (m_handle->IsValid())
	{
		m_handle->Close();
	}
}