using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AutoClicker
{
    static class Program
    {
        public static Thread clickingThread = new Thread(new ThreadStart(IOThread));
        public static Process targetProcess = null;
        public static AppForm appForm;
        public static VirtualKeys KeyToSend = VirtualKeys.VK_LBUTTON;
        public static VirtualKeys KeyToScan = VirtualKeys.VK_END;
        public static bool suspendClickingThread = true;

        // External funtions
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKeyA(uint uCode, uint uMapType);

        static void IOThread()
        {
            while (true)
            {
                do
                {
                    if (appForm.intervalNumericUpDown != null)
                        Thread.Sleep((int)appForm.intervalNumericUpDown.Value);
                    if ((GetAsyncKeyState((short)KeyToScan) & 1) > 0)
                        suspendClickingThread = !suspendClickingThread;
                } while (suspendClickingThread || targetProcess == null || targetProcess.MainWindowHandle != GetForegroundWindow());

                void createMouseEvent(MouseEvents dwFlags)
                {
                    mouse_event((uint)dwFlags, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, 0);
                }

                switch (KeyToSend)
                {
                    case VirtualKeys.VK_LBUTTON:
                        createMouseEvent(MouseEvents.MOUSEEVENTF_LEFTDOWN | MouseEvents.MOUSEEVENTF_LEFTUP);
                        break;
                    case VirtualKeys.VK_RBUTTON:
                        createMouseEvent(MouseEvents.MOUSEEVENTF_RIGHTDOWN | MouseEvents.MOUSEEVENTF_RIGHTUP);
                        break;
                    case VirtualKeys.VK_MBUTTON:
                        createMouseEvent(MouseEvents.MOUSEEVENTF_MIDDLEDOWN | MouseEvents.MOUSEEVENTF_MIDDLEUP);
                        break;
                    case VirtualKeys.VK_XBUTTON1:
                        createMouseEvent(MouseEvents.MOUSEEVENTF_XDOWN | MouseEvents.MOUSEEVENTF_XUP);
                        break;
                    case VirtualKeys.VK_XBUTTON2:   // same as VK_XBUTTON1
                        createMouseEvent(MouseEvents.MOUSEEVENTF_XDOWN | MouseEvents.MOUSEEVENTF_XUP);
                        break;
                    default:
                        keybd_event((byte)KeyToSend, (byte)MapVirtualKeyA((uint)KeyToSend, 0), (uint)KeyEvents.KEYEVENTF_EXTENDEDKEY, 0);
                        break;
                }
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            appForm = new AppForm();

            clickingThread.IsBackground = true;
            clickingThread.SetApartmentState(ApartmentState.STA);
            clickingThread.Start();

            Application.Run(appForm);
        }
    }
}
