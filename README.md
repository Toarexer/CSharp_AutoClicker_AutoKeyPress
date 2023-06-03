# CSharp_AutoClicker_AutoKeyPress

### Important!
The whole thing is very badly put together and isn't inteded for actual use.\
If you want something like this in C# then it is better to make it yourself.\
I have a C# wrapper for WinAPI input that can help: https://github.com/Toarexer/WinInput

### About
Simulates a specified keyboard event/mouse event in a specified process every x milliseconds.\
Keys are specified via virtual key codes from the winuser.h file (https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes). \
In the end I used my c dll, because it was more reliable than what I managed to create in c#. (https://github.com/Toarexer/windows-mouse-keyboard-input-dll)
