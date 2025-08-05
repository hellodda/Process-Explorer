#pragma once

typedef struct LANGANDCODEPAGE {
    WORD wLanguage;
    WORD wCodePage;
} *PLANGANDCODEPAGE, FAR* LPLANGANDCODEPAGE;

ULONGLONG FileTimeToULL(PFILETIME ft);

PWCHAR GetFileVersionString(HANDLE handle, LPCWSTR key);

//-----------
// NtQueryInformationProcess 0_0
//-----------