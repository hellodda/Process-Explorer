#include "pch.h"
#include "ProcessInformation.h"

Native::ProcessInformation::ProcessInformation()
{
	m_memoryCounters = new PROCESS_MEMORY_COUNTERS_EX();
	memset(m_memoryCounters, 0, sizeof(PROCESS_MEMORY_COUNTERS_EX));
}

Native::ProcessInformation::~ProcessInformation()
{
	if (m_memoryCounters)
	{
		delete m_memoryCounters;
		m_memoryCounters = nullptr;
	}
}

Native::ProcessInformation::!ProcessInformation()
{
	this->~ProcessInformation();
}
