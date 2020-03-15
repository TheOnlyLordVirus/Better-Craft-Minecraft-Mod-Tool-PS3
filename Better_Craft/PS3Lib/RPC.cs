using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PS3Lib;

namespace BlackOps3Osiris
{
    class RPC
    {
        public static PS3API PS3 = new PS3API();
        public static uint function_address = 0x3D0B28;


        public static int Init()
        {
            if (function_address == 0) return -1;
            Enable_RPC();
            return 0;
        }


        public static void Enable_RPC()
        {
            PS3.SetMemory(function_address, new byte[] { 0x4E, 0x80, 0x00, 0x20 });
            System.Threading.Thread.Sleep(20);
            byte[] func = new byte[] {
0x7c, 8, 2, 0xa6, 0xf8, 1, 0, 0x80, 60, 0x60, 0x10, 5, 0x81, 0x83, 0, 0x4c,
0x2c, 12, 0, 0, 0x41, 130, 0, 100, 0x80, 0x83, 0, 4, 0x80, 0xa3, 0, 8,
0x80, 0xc3, 0, 12, 0x80, 0xe3, 0, 0x10, 0x81, 3, 0, 20, 0x81, 0x23, 0, 0x18,
0x81, 0x43, 0, 0x1c, 0x81, 0x63, 0, 0x20, 0xc0, 0x23, 0, 0x24, 0xc0, 0x43, 0, 40,
0xc0, 0x63, 0, 0x2c, 0xc0, 0x83, 0, 0x30, 0xc0, 0xa3, 0, 0x34, 0xc0, 0xc3, 0, 0x38,
0xc0, 0xe3, 0, 60, 0xc1, 3, 0, 0x40, 0xc1, 0x23, 0, 0x48, 0x80, 0x63, 0, 0,
0x7d, 0x89, 3, 0xa6, 0x4e, 0x80, 4, 0x21, 60, 0x80, 0x10, 5, 0x38, 160, 0, 0,
0x90, 0xa4, 0, 0x4c, 0x90, 100, 0, 80, 0xe8, 1, 0, 0x80, 0x7c, 8, 3, 0xa6,
0x38, 0x21, 0, 0x70, 0x4e, 0x80, 0, 0x20
};
            PS3.SetMemory(function_address + 0x4, func);
            PS3.SetMemory(0x10050000, new byte[0x2854]);
            PS3.SetMemory(function_address, new byte[] { 0xF8, 0x21, 0xFF, 0x91 });
        }


        public static int Call(uint func_address, params object[] parameters)
        {
            int num_params = parameters.Length;
            uint num_floats = 0;
            for (uint i = 0; i < num_params; i++)
            {
                if (parameters[i] is int)
                {
                    byte[] val = BitConverter.GetBytes((int)parameters[i]);
                    Array.Reverse(val);
                    PS3.SetMemory(0x10050000 + (i + num_floats) * 4, val);
                }
                else if (parameters[i] is uint)
                {
                    byte[] val = BitConverter.GetBytes((uint)parameters[i]);
                    Array.Reverse(val);
                    PS3.SetMemory(0x10050000 + (i + num_floats) * 4, val);
                }
                else if (parameters[i] is string)
                {
                    byte[] str = Encoding.UTF8.GetBytes(Convert.ToString(parameters[i]) + "\0");
                    PS3.SetMemory(0x10050054 + i * 0x400, str);
                    uint addr = 0x10050054 + i * 0x400;
                    byte[] address = BitConverter.GetBytes(addr);
                    Array.Reverse(address);
                    PS3.SetMemory(0x10050000 + (i + num_floats) * 4, address);
                }
                else if (parameters[i] is float)
                {
                    num_floats++;
                    byte[] val = BitConverter.GetBytes((float)parameters[i]);
                    Array.Reverse(val);
                    PS3.SetMemory(0x10050024 + ((num_floats - 1) * 0x4), val);
                }
            }
            byte[] fadd = BitConverter.GetBytes(func_address);
            Array.Reverse(fadd);
            PS3.SetMemory(0x1005004C, fadd);
            System.Threading.Thread.Sleep(20);
            byte[] ret = PS3.Extension.ReadBytes(0x10050050, 4);
            Array.Reverse(ret);
            return BitConverter.ToInt32(ret, 0);
        }
    }
} 