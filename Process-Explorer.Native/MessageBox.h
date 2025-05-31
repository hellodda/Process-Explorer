#pragma once

namespace Native
{
	public ref class MessageBox abstract sealed
	{
	private:

		static void Show(System::String^ message, int buttons, int icon);

	public:
		static void ShowWarning(System::String^ message);
		static void ShowError(System::String^ message);
		static void ShowInformation(System::String^ message);
	};
}

