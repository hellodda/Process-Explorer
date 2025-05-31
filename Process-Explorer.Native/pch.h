#ifndef PCH_H
#define PCH_H

#define WIN32_LEAN_AND_MEAN
#define _WIN32_WINNT 0x0601

#include <Windows.h>
#include <psapi.h>
#include <tlhelp32.h>
#include <Shlwapi.h>
#include <wchar.h>
#include <Winternl.h>
#include <vcclr.h>  

#pragma comment(lib, "Version.lib")  
#pragma comment(lib, "Shlwapi.lib") 
#pragma comment(lib, "Advapi32.lib")
#pragma comment(lib, "User32.lib")


#endif //PCH_H
