using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace birt_deconp_editor
{
    public class Program
    {
        /*
         * Enable Popup output for Console Application
         */
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

        /*
         * symmetischer 24 Byte DESede Schlüssel von "org.eclipse.datatools" verwendet in BIRT bzw. Eclipse
         */
        public static byte[] passphrase = new byte[] { 0xEA, 0xF1, 0x57, 0xFB, 0xFD, 0xF2, 0x6E, 0x0E, 0x3B, 0x9D, 0xC8, 0x7F, 0x16, 0x0B, 0x91, 0x25, 0xEA, 0xF1, 0x57, 0xFB, 0xFD, 0xF2, 0x6E, 0x0E };
        #region alternativ
        // byte[] passphrase = StringToByteArray("EAF157FBFDF26E0E3B9DC87F160B9125EAF157FBFDF26E0E");
        #endregion

        public static void Main(string[] args)
        {
            if (!args.Any())
            {
                MessageBoxUi($@"Information
This Application edits BIRT Connection Profile Files (DataSource).
Drag and Drop a File onto '{System.AppDomain.CurrentDomain.FriendlyName}.exe' oder supply an absolute Path to the File as the first Parameter.");
                return;
            }

            string srcFilePath = args[0];
            string tmpFolderPath = Path.GetTempPath();
            string tmpFolderFilePath = Path.Combine(tmpFolderPath, Path.GetFileName(srcFilePath));

            try
            {
                // read file
                byte[] inEncrByte = System.IO.File.ReadAllBytes(srcFilePath);
                // decrypt
                byte[] outDecrString = DecryptByte(inEncrByte, passphrase);
                // write temp file
                System.IO.File.WriteAllBytes(tmpFolderFilePath, outDecrString);
                // start editor with temp file
                Process process = Process.Start("notepad.exe", tmpFolderFilePath);
                process.WaitForExit();
                // read temp file
                byte[] tempFileContent = System.IO.File.ReadAllBytes(tmpFolderFilePath);
                // encrypt
                byte[] outEncString = EncryptByte(tempFileContent, passphrase);
                // write file
                System.IO.File.WriteAllBytes(srcFilePath, outEncString);
            }
            finally
            {
                // delete temp file
                System.IO.File.Delete(tmpFolderFilePath);
            }
        }

        public static byte[] DecryptByte(byte[] DataToDecrypt, byte[] TDESKey)
        {
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider()
            {
                Key = shortenKey(TDESKey),
                Mode = CipherMode.ECB, // CipherMode.CBC is C# default
                Padding = PaddingMode.PKCS7,
            };

            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                return Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
            }
        }

        public static byte[] EncryptByte(byte[] DataToEncrypt, byte[] TDESKey)
        {
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider()
            {
                Key = shortenKey(TDESKey),
                Mode = CipherMode.ECB, // CipherMode.CBC is C# default
                Padding = PaddingMode.PKCS7,
            };

            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                return Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
            }
        }

        private static byte[] shortenKey(byte[] TDESKey)
        {
            if (TDESKey.Length <= 24)
                return TDESKey;

            Console.WriteLine("Warning: The Key is larger than 24 Byte/192 Bit and got shortend!");
            System.Array.Resize(ref TDESKey, 192 / 8);
            return TDESKey;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static void MessageBoxUi(string message)
        {
            MessageBox((IntPtr)0, message, System.AppDomain.CurrentDomain.FriendlyName, 0);
        }
    }
}
