using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
        public const string tempFolder = @"C:\Windows\Temp\";
        public const string dllName = "AutoClicker.simInputLib.dll";

        // External funtions
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        static extern void GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern int ToUnicode(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)] char[] pwszBuff, int cchBuff, uint wFlags);
        // help from https://stackoverflow.com/questions/6929275/how-to-convert-a-virtual-key-code-to-a-character-according-to-the-current-keyboa

        [DllImport(tempFolder + dllName)]
        static extern void test();

        [DllImport(tempFolder + dllName)]
        static extern void keyboardKeyPress(byte virtualKey, uint timeMs);

        [DllImport(tempFolder + dllName)]
        static extern void mouseInput(long x, long y, ulong dwFlags, ulong mouseData);

        static void IOThread()
        {
            byte[] keyState = new byte[256];
            char[] str = { '\0' };
            while (true)
            {
                do
                {
                    if (appForm.intervalNumericUpDown != null)
                        Thread.Sleep((int)appForm.intervalNumericUpDown.Value);
                    if ((GetAsyncKeyState((short)KeyToScan) & 1) > 0)
                        suspendClickingThread = !suspendClickingThread;
                } while (suspendClickingThread || targetProcess == null || targetProcess.MainWindowHandle != GetForegroundWindow());

                void createMouseEvent(MouseEvents dwFlagDown, MouseEvents dwFlagUp, uint dwData = 0)
                {
                    mouseInput(Cursor.Position.X, Cursor.Position.Y, (uint)dwFlagDown, dwData);
                    Thread.Sleep(1);
                    mouseInput(Cursor.Position.X, Cursor.Position.Y, (uint)dwFlagUp, dwData);
                }

                switch (KeyToSend)
                {
                    case VirtualKeys.VK_LBUTTON:
                        createMouseEvent(MouseEvents.MOUSEEVENTF_LEFTDOWN, MouseEvents.MOUSEEVENTF_LEFTUP);
                        break;
                    case VirtualKeys.VK_RBUTTON:
                        createMouseEvent(MouseEvents.MOUSEEVENTF_RIGHTDOWN, MouseEvents.MOUSEEVENTF_RIGHTUP);
                        break;
                    case VirtualKeys.VK_MBUTTON:
                        createMouseEvent(MouseEvents.MOUSEEVENTF_MIDDLEDOWN, MouseEvents.MOUSEEVENTF_MIDDLEUP);
                        break;
                    case VirtualKeys.VK_XBUTTON1:
                        createMouseEvent(MouseEvents.MOUSEEVENTF_XDOWN, MouseEvents.MOUSEEVENTF_XUP, 1);
                        break;
                    case VirtualKeys.VK_XBUTTON2:
                        createMouseEvent(MouseEvents.MOUSEEVENTF_XDOWN, MouseEvents.MOUSEEVENTF_XUP, 2);
                        break;
                    default:
                        keyboardKeyPress((byte)KeyToSend, 1);
                        break;
                }
            }
        }

        [STAThread]
        static void Main()
        {
            ExtractEmbeddedDLL();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            appForm = new AppForm();

            clickingThread.IsBackground = true;
            clickingThread.SetApartmentState(ApartmentState.STA);
            clickingThread.Start();

            Application.Run(appForm);

            clickingThread.Abort();
        }

        static void ExtractEmbeddedDLL()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            Stream stream = assembly.GetManifestResourceStream(dllName);
            BinaryReader br = new BinaryReader(stream);

            FileStream fs = new FileStream(tempFolder + dllName, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(br.ReadBytes((int)stream.Length));
            bw.Close();
            fs.Close();
        }
    }
}