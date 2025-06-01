#include "pch.h"
#include "ProcessExplorer-Definitions.h"

ULONGLONG FileTimeToULL(PFILETIME ft)
{
	if (!ft) throw gcnew System::NullReferenceException("FILETIME is null!");

	return (((ULONGLONG)ft->dwHighDateTime) << 32) | ft->dwLowDateTime;
}

