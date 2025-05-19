#include "pch.h"
#include "Process.h"
#include "Handle.h"
#include "ProcessInformation.h"

Native::Process::Process(DWORD pid)
{
	m_handle = gcnew Native::Handle(OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, pid));
}
Native::Process::Process(Handle^ handle) : m_handle(handle) {}
Native::ProcessInformation^ Native::Process::GetProcessInformation()
{
	auto info = gcnew ProcessInformation();
	info->PID = GetProcessId(m_handle);
	return info;
}
Native::Process::~Process()
{
	if (m_handle->IsValid())
	{
		m_handle->Close();
	}
}