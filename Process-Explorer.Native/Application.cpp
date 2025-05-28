#include "pch.h"
#include "Application.h"
#include "Handle.h"

void Native::Application::SetPrivileges()
{
	Handle^ hToken = gcnew Native::Handle();
	pin_ptr<HANDLE> pHandle = hToken->GetPtr();

    if (OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, pHandle)) {
        TOKEN_PRIVILEGES tokenPrivileges;
        if (!LookupPrivilegeValue(NULL, SE_DEBUG_NAME, &tokenPrivileges.Privileges[0].Luid)) {
            hToken->Close();
			throw gcnew System::ApplicationException("Failed to lookup privilege value.");
        }

        tokenPrivileges.PrivilegeCount = 1;
        tokenPrivileges.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

        if (!AdjustTokenPrivileges(hToken, FALSE, &tokenPrivileges, sizeof(tokenPrivileges), NULL, NULL)) {
			hToken->Close();
			throw gcnew System::ApplicationException("Failed to adjust token privileges.");
        }
        if (GetLastError() == ERROR_NOT_ALL_ASSIGNED) {
			hToken->Close();
            throw gcnew System::ApplicationException("Pls run as admin"); // adminov krkni ara
        }
		hToken->Close();
    }
}
