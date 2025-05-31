#include "pch.h"
#include "MessageBox.h"

void Native::MessageBox::Show(System::String^ message, int buttons, int icon)
{
    pin_ptr<const wchar_t> wchMessage = PtrToStringChars(message);

    MessageBoxW(NULL, wchMessage, L"Message", buttons | icon);
}

void Native::MessageBox::ShowWarning(System::String^ message)
{
	Show(message, MB_OK, MB_ICONWARNING);
}

void Native::MessageBox::ShowError(System::String^ message)
{
	Show(message, MB_OK, MB_ICONERROR);
}

void Native::MessageBox::ShowInformation(System::String^ message)
{
	Show(message, MB_OK, MB_ICONINFORMATION);
}
