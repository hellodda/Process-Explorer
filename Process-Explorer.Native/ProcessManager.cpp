#include "pch.h"  
#include "ProcessManager.h"  
#include "Process.h"  
#include "CriticalSection.h"

using namespace System::Collections::Generic;


Native::ProcessManager::ProcessManager()
{
	m_cs = gcnew Native::CriticalSection();
}

IEnumerable<Native::Process^>^ Native::ProcessManager::GetActiveProcesses()
{  
   m_cs->Lock();

   auto processes = gcnew List<Native::Process^>();  

   DWORD PIDs[1024], needed;  
   if (EnumProcesses(PIDs, sizeof(PIDs), &needed)) {  
       size_t count = needed / sizeof(DWORD);  
       for (size_t i = 0; i < count; ++i) {  
		   if (PIDs[i] == 0) continue;
           processes->Add(gcnew Native::Process(PIDs[i]));
       }  
   }  

   m_cs->Unlock();

   return processes;  
}

