#include "pch.h"
#include "Handle.h"
#include "Process.h"
#include "ProcessInformation.h"
#include "ProcessInformationProvider.h"

Native::ProcessInformation^ Native::ProcessInformationProvider::GetProcessInformation(Native::Process^ process)
{
    auto handle = process->GetHandle();

    if (!handle->IsValid())
        throw gcnew System::NullReferenceException("Process handle is null.");

    auto info = gcnew Native::ProcessInformation;
   
    info->PID = GetProcessId(handle);
    info->Name = process->GetProcessName();
    info->Description = process->GetProcessDescription();
    info->Company = process->GetProcessCompany();
    info->FilePath = process->GetProcessFilePath();

    auto memCounters = process->GetProcessMemoryCounters();

    info->WorkingSet = memCounters.WorkingSetSize;
    info->PrivateBytes = memCounters.PrivateUsage;

    info->CpuUsage = process->GetCpuUsage();

    return info;
}
