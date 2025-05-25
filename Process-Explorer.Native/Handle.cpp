#include "pch.h"
#include "Handle.h"

Native::Handle::Handle() : m_handle(INVALID_HANDLE_VALUE) {}
Native::Handle::Handle(HANDLE handle) : m_handle(handle) {}
Native::Handle::Handle(Handle^ other) : m_handle(other->Duplicate()) {}
Native::Handle::Handle(Handle^% other) : m_handle(other->Detach()) {}

Native::Handle^ Native::Handle::operator=(Handle^ other)
{
	if (this != other)
	{
		Close();
		m_handle = other->Duplicate();
	}
	return this;
}

Native::Handle^ Native::Handle::operator=(Handle^% other)
{
	if (this != other)
	{
		Close();
		m_handle = other->Detach();
	}
	return this;
}

Native::Handle::~Handle()
{
	Close();
}

Native::Handle::!Handle()
{
	this->~Handle();
}

bool Native::Handle::IsValid()
{
	return m_handle != INVALID_HANDLE_VALUE || m_handle;
}

HANDLE Native::Handle::Get()
{
	return m_handle;
}

HANDLE Native::Handle::Detach()
{
	HANDLE temp = m_handle;
	m_handle = INVALID_HANDLE_VALUE;
	return temp;
}

HANDLE Native::Handle::Duplicate()
{
	HANDLE duplicateHandle = nullptr;
	if (IsValid() &&
		DuplicateHandle(GetCurrentProcess(), m_handle, GetCurrentProcess(), &duplicateHandle, 0, FALSE, DUPLICATE_SAME_ACCESS))
	{
		return duplicateHandle;
	}
	return INVALID_HANDLE_VALUE;
}

void Native::Handle::Close()
{
	if (IsValid())
	{
		CloseHandle(m_handle);
		m_handle = INVALID_HANDLE_VALUE;
	}
}

System::String^ Native::Handle::Address()
{
	if (!IsValid()) throw gcnew System::NullReferenceException("Handle is not valid.");

	return System::String::Format("0x{0:X}", reinterpret_cast<size_t>(m_handle));
}

Native::Handle::operator bool()
{
	return m_handle;
}

Native::Handle::operator HANDLE()
{
	return m_handle;
}
