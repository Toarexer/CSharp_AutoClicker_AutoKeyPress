using System;
using System.Runtime.InteropServices;

namespace AutoClicker
{
    static class User32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);
    }

    static class Kernel32
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string libFileName);
        [DllImport("kernel32.dll")]
        public static extern void FreeLibrary(IntPtr libModule);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr module, string procName);
    }
}
