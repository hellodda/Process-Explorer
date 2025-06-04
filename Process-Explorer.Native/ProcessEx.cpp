#include "pch.h"
#include "ProcessEx.h"
#include "ProcessExplorer-Definitions.h"
#include "Handle.h"
#include "CriticalSection.h"
#include "ProcessInformationEx.h"

Native::ProcessEx::ProcessEx(PSYSTEM_PROCESS_INFORMATION_EX pInfo) : m_handle(gcnew Native::Handle(OpenProcess(PROCESS_ALL_ACCESS, FALSE, (ULONG_PTR)pInfo->UniqueProcessId))), m_cs(gcnew Native::CriticalSection()), m_info(gcnew Native::ProcessInformationEx())
{
    SaveOrUpdateProcessInformation(pInfo);
}

Native::ProcessEx::~ProcessEx()
{
	
}

Native::ProcessEx::!ProcessEx()
{
	this->~ProcessEx();
}

Native::ProcessInformationEx^ Native::ProcessEx::GetProcessInformation()
{
    return m_info;
}

System::String^ Native::ProcessEx::GetProcessDescription()
{
    if (!m_handle->IsValid()) return "";

    WCHAR fullPath[MAX_PATH]{ 0 };
    if (GetModuleFileNameEx(m_handle, nullptr, fullPath, MAX_PATH))
    {
        DWORD dummy;
        DWORD size = GetFileVersionInfoSize(fullPath, &dummy);
        if (size)
        {
            PLANGANDCODEPAGE lpTranslate;
            BYTE* versionData = new BYTE[size];
            if (GetFileVersionInfo(fullPath, NULL, size, versionData))
            {
                UINT cbTranslate{ 0 };
                if (VerQueryValue(versionData, L"\\VarFileInfo\\Translation", (LPVOID*)&lpTranslate, &cbTranslate))
                {
                    WCHAR subBlock[64]{};

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

System::String^ Native::ProcessEx::GetProcessCompany()
{
    if (!m_handle->IsValid()) return "";

    WCHAR fullPath[MAX_PATH]{ 0 };
    if (GetModuleFileNameEx(m_handle, nullptr, fullPath, MAX_PATH))
    {
        DWORD dummy;
        DWORD size = GetFileVersionInfoSize(fullPath, &dummy);
        if (size)
        {
            PLANGANDCODEPAGE lpTranslate;
            BYTE* versionData = new BYTE[size];
            if (GetFileVersionInfo(fullPath, NULL, size, versionData))
            {
                UINT cbTranslate{ 0 };
                if (VerQueryValue(versionData, L"\\VarFileInfo\\Translation", (LPVOID*)&lpTranslate, &cbTranslate))
                {
                    WCHAR subBlock[64]{};

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

System::String^ Native::ProcessEx::GetProcessFilePath()
{
    if (!m_handle->IsValid()) return "";

    TCHAR path[MAX_PATH]{ 0 };
    DWORD size{ MAX_PATH };
    if (!QueryFullProcessImageName(m_handle, 0, path, &size))
        throw gcnew System::Exception(">:{ failed.");

    return gcnew System::String(path);
}


void Native::ProcessEx::UpdateProcessCpuUsage(PSYSTEM_PROCESS_INFORMATION_EX pspi)
{

    auto processorTime = pspi->KernelTime.QuadPart + pspi->UserTime.QuadPart;
    auto now = System::DateTime::UtcNow;

    if (!m_firstTimeMeasured)
    {
        m_info->ProcessorInfo->ProcessorTime = pspi->KernelTime.QuadPart + pspi->UserTime.QuadPart;
        m_info->ProcessorInfo->LastCheckTime = now;
        m_firstTimeMeasured = true;
        return;
    }

    System::TimeSpan deltaTime = now - m_info->ProcessorInfo->LastCheckTime;
    
    auto elapsedTime = deltaTime.TotalMilliseconds;
    
    auto deltaProcessorTime = processorTime - m_info->ProcessorInfo->ProcessorTime;

    auto cpuUsage = (deltaProcessorTime / (elapsedTime * 10'000.0 * System::Environment::ProcessorCount)) * 100;
    
    m_info->ProcessorInfo->ProcessorTime = processorTime;
    m_info->ProcessorInfo->LastCheckTime = now;
    m_info->CpuUsage = System::Math::Min(System::Math::Max(cpuUsage, 0.0), 100.0);
}


void Native::ProcessEx::SaveOrUpdateProcessInformation(PSYSTEM_PROCESS_INFORMATION_EX pspi)
{
    m_cs->Lock();

    if (!m_dataReceived)
    {
        m_info->PID = (ULONG_PTR)pspi->UniqueProcessId;
        m_info->PPID = (ULONG_PTR)pspi->InheritedFromUniqueProcessId;
        m_info->Name = pspi->ImageName.Buffer ? gcnew System::String(pspi->ImageName.Buffer, 0, pspi->ImageName.Length / sizeof(WCHAR)) : "Unknown";
        
        if (m_handle->IsValid())
        {
            m_info->Description = GetProcessDescription();
            m_info->Company = GetProcessCompany();
            m_info->FilePath = GetProcessFilePath();
        }
        m_dataReceived = true;
    }

    m_info->WorkingSet = pspi->WorkingSetSize;
    m_info->PrivateBytes = pspi->PrivatePageCount;

    m_info->HandlesCount = pspi->HandleCount;
    m_info->ThreadsCount = pspi->NumberOfThreads;

    UpdateProcessCpuUsage(pspi);

    m_cs->Unlock();
}
