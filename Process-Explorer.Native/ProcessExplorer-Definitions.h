#pragma once

#define KB 1024
#define MB 1024 * KB
#define GB 1024 * MB
#define TB 1024 * GB

typedef struct LANGANDCODEPAGE {
    WORD wLanguage;
    WORD wCodePage;
} *PLANGANDCODEPAGE, FAR* LPLANGANDCODEPAGE;

typedef struct _SYSTEM_PROCESS_INFORMATION_EX {
    ULONG NextEntryOffset;
    ULONG NumberOfThreads;
    LARGE_INTEGER Reserved[3];
    LARGE_INTEGER CreateTime;
    LARGE_INTEGER UserTime;     
    LARGE_INTEGER KernelTime;   
    UNICODE_STRING ImageName;
    KPRIORITY BasePriority;
    HANDLE UniqueProcessId;
    HANDLE InheritedFromUniqueProcessId;
    ULONG HandleCount;
    ULONG SessionId;
    ULONG_PTR PageDirectoryBase;
    SIZE_T PeakVirtualSize;
    SIZE_T VirtualSize;
    ULONG PageFaultCount;
    SIZE_T PeakWorkingSetSize;
    SIZE_T WorkingSetSize;
    SIZE_T QuotaPeaedPoolUsage;
    SIZE_T QuotaPeakPagedPoolUsage;
    SIZE_T QuotaPagkNonPagedPoolUsage;
    SIZE_T QuotaNonPagedPoolUsage;
    SIZE_T PagefileUsage;
    SIZE_T PeakPagefileUsage;
    SIZE_T PrivatePageCount;
    LARGE_INTEGER ReadOperationCount;
    LARGE_INTEGER WriteOperationCount;
    LARGE_INTEGER OtherOperationCount;
    LARGE_INTEGER ReadTransferCount;
    LARGE_INTEGER WriteTransferCount;
    LARGE_INTEGER OtherTransferCount;
} SYSTEM_PROCESS_INFORMATION_EX, * PSYSTEM_PROCESS_INFORMATION_EX, FAR* LPSYSTEM_PROCESS_INFORMATION_EX;



ULONGLONG FileTimeToULL(PFILETIME ft);

//-----------
// NtQueryInformationProcess 0_0
//-----------