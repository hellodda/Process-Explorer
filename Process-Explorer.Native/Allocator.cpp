#include "pch.h"
#include "Allocator.h"

void Native::Allocator::Deallocate(void* buffer)
{
	if (buffer)
	{
		if (!VirtualFree(buffer, 0, MEM_RELEASE))
			throw gcnew System::NullReferenceException("mem error");
	}
}
