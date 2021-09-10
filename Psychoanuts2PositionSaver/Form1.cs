using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Psychonauts2PositionSaver
{
    public partial class Form1 : Form
    {
        private static VAMemory Vam;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static bool steamVersion = false;
        private static bool steamVersion2 = false;
        private static bool gogVersion = false;
        private static bool gamepassVersion = false;
        private static long offsetsAddedX = 0;
        private static long offsetsAddedY = 0;
        private static long offsetsAddedZ = 0;
        private static float x;
        private static float y;
        private static float z;
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public Form1()
        {
            InitializeComponent();
            List<Process> processes = Process.GetProcesses().ToList();

            if (processes.Any(x => x.ProcessName == "Psychonauts2-Win64-Shipping"))
            {
                Vam = new VAMemory("Psychonauts2-Win64-Shipping");
                int whichVersion = processes.Where(x => x.ProcessName == "Psychonauts2-Win64-Shipping").FirstOrDefault().Modules[0].ModuleMemorySize;
                if (whichVersion == 95236096)
                {
                    // steam patch 2
                    steamVersion = true;

                }
                else if (whichVersion == 95240192)
                {
                    // steam patch 3
                    steamVersion2 = true;
                }
                else if (whichVersion == 92188672)
                {
                    // GOG version
                    gogVersion = true;
                }
            }
            else if (processes.Any(x => x.ProcessName == "Psychonauts2-WinGDK-Shipping"))
            {
                Vam = new VAMemory("Psychonauts2-WinGDK-Shipping");
                gamepassVersion = true;
            }

            if (!Vam.CheckProcess())
            {
                Environment.Exit(0);
            }

            _hookID = SetHook(_proc);
        }


        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (!Vam.CheckProcess())
            {
                UnhookWindowsHookEx(_hookID);
                Environment.Exit(0);
            }

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == (int)KeyDefinitions.F1 || vkCode == (int)KeyDefinitions.F2 || vkCode == (int)KeyDefinitions.F3
                    || vkCode == (int)KeyDefinitions.F4)
                {
                    
                    if (steamVersion)
                    {
                        long baseAddressX = Vam.ReadInt64((IntPtr)(Vam.getBaseAddress + 0x055418E0));
                        offsetsAddedX = Vam.ReadInt64((IntPtr)baseAddressX + 0x30);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x138);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x8);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x70);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x2A0);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x2C0);
                        x = Vam.ReadFloat((IntPtr)offsetsAddedX + 0x1F0);

                        long baseAddressY = Vam.ReadInt64((IntPtr)(Vam.getBaseAddress + 0x05534E70));
                        offsetsAddedY = Vam.ReadInt64((IntPtr)baseAddressY + 0x0);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x20);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x130);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0xE8);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x8);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0xD8);
                        y = Vam.ReadFloat((IntPtr)offsetsAddedY + 0x1F4);

                        long baseAddressZ = Vam.ReadInt64((IntPtr)(Vam.getBaseAddress + 0x0553DFD0));
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)baseAddressZ + 0x8);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x8);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x240);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x80);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x130);
                        z = Vam.ReadFloat((IntPtr)offsetsAddedZ + 0x1F8);
                    }
                    else if (steamVersion2)
                    {
                        long baseAddressX = Vam.ReadInt64((IntPtr)(Vam.getBaseAddress + 0x0553EFC0));
                        offsetsAddedX = Vam.ReadInt64((IntPtr)baseAddressX + 0x8);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x8);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x180);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x2A0);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0xB0);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x130);
                        x = Vam.ReadFloat((IntPtr)offsetsAddedX + 0x1F0);

                        long baseAddressY = Vam.ReadInt64((IntPtr)(Vam.getBaseAddress + 0x0553EFC0));
                        offsetsAddedY = Vam.ReadInt64((IntPtr)baseAddressY + 0x8);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x8);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x180);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x328);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0xD8);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0xD8);
                        y = Vam.ReadFloat((IntPtr)offsetsAddedY + 0x1F4);

                        long baseAddressZ = Vam.ReadInt64((IntPtr)(Vam.getBaseAddress + 0x0553EFC0));
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)baseAddressZ + 0x8);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x8);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x190);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x98);
                        z = Vam.ReadFloat((IntPtr)offsetsAddedZ + 0x1F8);
                    }
                    else if (gogVersion)
                    {
                        long baseAddressX = Vam.ReadInt64((IntPtr)(Vam.getBaseAddress + 0x051E9198));
                        offsetsAddedX = Vam.ReadInt64((IntPtr)baseAddressX + 0x8);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x120);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x1D0);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x2C0);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x58);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0xD8);
                        x = Vam.ReadFloat((IntPtr)offsetsAddedX + 0x1F0);

                        long baseAddressY = Vam.ReadInt64((IntPtr)(Vam.getBaseAddress + 0x051E9198));
                        offsetsAddedY = Vam.ReadInt64((IntPtr)baseAddressY + 0x0);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x120);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x230);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x10);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x248);
                        y = Vam.ReadFloat((IntPtr)offsetsAddedY + 0x1F4);

                        long baseAddressZ = Vam.ReadInt64((IntPtr)(Vam.getBaseAddress + 0x051E9198));
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)baseAddressZ + 0x28);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x120);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x2B8);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x20);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x5A0);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0xD8);
                        z = Vam.ReadFloat((IntPtr)offsetsAddedZ + 0x1F8);
                    }
                    else if (gamepassVersion)
                    {
                        long baseAddressX = Vam.ReadInt64((IntPtr)(Vam.getBaseAddress + 0x0513A0C0));
                        offsetsAddedX = Vam.ReadInt64((IntPtr)baseAddressX + 0x30);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x280);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x1D0);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0x250);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0xD8);
                        offsetsAddedX = Vam.ReadInt64((IntPtr)offsetsAddedX + 0xD8);
                        x = Vam.ReadFloat((IntPtr)offsetsAddedX + 0x1F0);

                        long baseAddressY = Vam.ReadInt64((IntPtr)(Vam.getBaseAddress + 0x0514D8E0));
                        offsetsAddedY = Vam.ReadInt64((IntPtr)baseAddressY + 0x8);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x8);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x180);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x350);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x20);
                        offsetsAddedY = Vam.ReadInt64((IntPtr)offsetsAddedY + 0x2C0);
                        y = Vam.ReadFloat((IntPtr)offsetsAddedY + 0x1F4);

                        long baseAddressZ = Vam.ReadInt64((IntPtr)(Vam.getBaseAddress + 0x051511E8));
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)baseAddressZ + 0x30);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x138);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x8);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x190);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x140);
                        offsetsAddedZ = Vam.ReadInt64((IntPtr)offsetsAddedZ + 0x2C0);
                        z = Vam.ReadFloat((IntPtr)offsetsAddedZ + 0x1F8);
                    }


                    Form1 form = (Form1)Application.OpenForms["Form1"];

                    if (vkCode == (int)KeyDefinitions.F1 || vkCode == (int)KeyDefinitions.F3)
                    {
                        if (vkCode == (int)KeyDefinitions.F1)
                        {
                            form.label10.Text = x.ToString();
                            form.label11.Text = y.ToString();
                            form.label12.Text = z.ToString();
                        }
                        else if (vkCode == (int)KeyDefinitions.F3)
                        {
                            form.label13.Text = x.ToString();
                            form.label14.Text = y.ToString();
                            form.label15.Text = z.ToString();
                        }
                    }
                    else if (vkCode == (int)KeyDefinitions.F2 || vkCode == (int)KeyDefinitions.F4)
                    {
                        if (vkCode == (int)KeyDefinitions.F2)
                        {
                            if (steamVersion || steamVersion2 || gogVersion || gamepassVersion)
                            {
                                Vam.WriteFloat((IntPtr)offsetsAddedX + 0x1F0, float.Parse(form.label10.Text));
                                Vam.WriteFloat((IntPtr)offsetsAddedY + 0x1F4, float.Parse(form.label11.Text));
                                Vam.WriteFloat((IntPtr)offsetsAddedZ + 0x1F8, float.Parse(form.label12.Text));
                            }
                        }
                        else
                        {
                            if (steamVersion || steamVersion2 || gogVersion || gamepassVersion)
                            {
                                Vam.WriteFloat((IntPtr)offsetsAddedX + 0x1F0, float.Parse(form.label13.Text));
                                Vam.WriteFloat((IntPtr)offsetsAddedY + 0x1F4, float.Parse(form.label14.Text));
                                Vam.WriteFloat((IntPtr)offsetsAddedZ + 0x1F8, float.Parse(form.label15.Text));
                            }
                        }
                    }
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnhookWindowsHookEx(_hookID);
        }

        public enum KeyDefinitions
        {
            F1 = 112,
            F2 = 113,
            F3 = 114,
            F4 = 115,
            F5 = 116,
            F6 = 117,
            F7 = 118,
            F8 = 119,
            F9 = 120,
            F10 = 121,
            F11 = 122,
            F12 = 123,
            a = 65,
            b = 66,
            c = 67,
            d = 68,
            e = 69,
            f = 70,
            g = 71,
            h = 72,
            i = 73,
            j = 74,
            k = 75,
            l = 76,
            m = 77,
            n = 78,
            o = 79,
            p = 80,
            q = 81,
            r = 82,
            s = 83,
            t = 84,
            u = 85,
            v = 86,
            w = 87,
            x = 88,
            y = 89,
            z = 90,
            D1 = 49,
            D2 = 50,
            D3 = 51,
            D4 = 52,
            D5 = 53,
            D6 = 54,
            D7 = 55,
            D8 = 56,
            D9 = 57,
            D0 = 48
        }
    }
}
