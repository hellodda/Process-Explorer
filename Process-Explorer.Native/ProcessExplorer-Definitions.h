#pragma once

typedef struct LANGANDCODEPAGE {
    WORD wLanguage;
    WORD wCodePage;
} *PLANGANDCODEPAGE, FAR* LPLANGANDCODEPAGE;

ULONGLONG FileTimeToULL(PFILETIME ft);

//-----------
// NtQueryInformationProcess 0_0
//-----------