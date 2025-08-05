#include "pch.h"
#include "ProcessExplorer-Definitions.h"

ULONGLONG FileTimeToULL(PFILETIME ft)
{
	if (!ft) throw gcnew System::NullReferenceException("FILETIME is null!");

	return (((ULONGLONG)ft->dwHighDateTime) << 32) | ft->dwLowDateTime;
}

PWCHAR GetFileVersionString(HANDLE handle, LPCWSTR key)
{
    if (handle == nullptr || handle == INVALID_HANDLE_VALUE)
        return nullptr;

    WCHAR fullPath[MAX_PATH]{};
    if (!GetModuleFileNameEx(handle, nullptr, fullPath, MAX_PATH))
        return nullptr;

    DWORD dummy{};
    DWORD size = GetFileVersionInfoSize(fullPath, &dummy);
    if (!size)
        return nullptr;

    BYTE* versionData = new BYTE[size];
    PWCHAR result = nullptr;

    if (GetFileVersionInfo(fullPath, 0, size, versionData))
    {
        PLANGANDCODEPAGE lpTranslate{};
        UINT cbTranslate{};

        if (VerQueryValue(versionData, L"\\VarFileInfo\\Translation",
            (LPVOID*)&lpTranslate, &cbTranslate))
        {
            WCHAR subBlock[64]{};
            swprintf_s(subBlock, L"\\StringFileInfo\\%04x%04x\\%s",
                lpTranslate[0].wLanguage,
                lpTranslate[0].wCodePage,
                key);

            LPVOID lpBuffer{};
            UINT dwBytes{};
            if (VerQueryValue(versionData, subBlock, &lpBuffer, &dwBytes))
            {
                size_t len = wcslen((wchar_t*)lpBuffer) + 1;
                result = new wchar_t[len];
                wcscpy_s(result, len, (wchar_t*)lpBuffer);
            }
        }
    }

    delete[] versionData; 
    return result;      
}


