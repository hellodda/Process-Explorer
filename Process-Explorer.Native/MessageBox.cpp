#include "pch.h"
#include "MessageBox.h"

void Native::MessageBox::Show(System::String^ message)
{
    pin_ptr<const wchar_t> wchMessage = PtrToStringChars(message);

    MessageBoxW(NULL, wchMessage, L"Message", MB_OK | MB_ICONEXCLAMATION);
}
