#include "pch.h"
#include "Process.h"
#include "Handle.h"

Native::Process::Process(DWORD pid)
{
	m_handle = gcnew Handle(OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, pid));
}
Native::Process::Process(Handle^ handle) : m_handle(handle) {}
Native::Process::~Process()
{
	if (m_handle->IsValid())
	{
		m_handle->Close();
	}
}