#include "pch.h"  
#include "ProcessManager.h"  
#include "Process.h"  

IEnumerable<Native::Process^>^ Native::ProcessManager::GetActiveProcesses()  
{  
   
   auto processes = gcnew List<Process^>();  

   DWORD PIDs[1024], needed;  
   if (EnumProcesses(PIDs, sizeof(PIDs), &needed)) {  
       size_t count = needed / sizeof(DWORD);  
       for (size_t i = 0; i < count; ++i) {  
           DWORD pid = PIDs[i];  
		   processes->Add(gcnew Process(pid));
       }  
   }  

   return processes;  
}
