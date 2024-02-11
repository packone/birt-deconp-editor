using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

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
        private static readonly byte[] passphrase = [0xEA, 0xF1, 0x57, 0xFB, 0xFD, 0xF2, 0x6E, 0x0E, 0x3B, 0x9D, 0xC8, 0x7F, 0x16, 0x0B, 0x91, 0x25, 0xEA, 0xF1, 0x57, 0xFB, 0xFD, 0xF2, 0x6E, 0x0E];
        #region alternativ
        // byte[] passphrase = StringToByteArray("EAF157FBFDF26E0E3B9DC87F160B9125EAF157FBFDF26E0E");
        #endregion

        private static readonly string appName = AppDomain.CurrentDomain.FriendlyName;
        private readonly TripleDES TDESAlgorithm = TripleDES.Create();

        public Program()
        {
            SetupTDESAlg();
        }

        /// <summary>
        /// Setup TDES Algorithm
        /// </summary>
        private void SetupTDESAlg()
        {
            TDESAlgorithm.Key = ShortenKey(passphrase);
            TDESAlgorithm.Mode = CipherMode.ECB; // CipherMode.CBC is C# default;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
        }

        public static void Main(string[] args)
        {
            var program = new Program();
            if (args.Length == 0)
            {
                MessageBoxUi($@"Information
This Application edits BIRT Connection Profile Files (DataSource).
Drag and Drop a File onto '{appName}.exe' oder supply an absolute Path to the File as the first Parameter.");
                return;
            }

            string srcFilePath = args[0];
            string tmpFolderPath = Path.GetTempPath();
            string tmpFolderFilePath = Path.Combine(tmpFolderPath, Path.GetFileName(srcFilePath));

            try
            {
                // read file
                byte[] inEncrByte = File.ReadAllBytes(srcFilePath);
                // decrypt
                byte[] outDecrString = program.DecryptByte(inEncrByte);
                // write temp file
                File.WriteAllBytes(tmpFolderFilePath, outDecrString);
                // start editor with temp file
                Process process = Process.Start("notepad.exe", tmpFolderFilePath);
                process.WaitForExit();
                // read temp file
                byte[] tempFileContent = File.ReadAllBytes(tmpFolderFilePath);
                // encrypt
                byte[] outEncString = program.EncryptByte(tempFileContent);
                // write file
                File.WriteAllBytes(srcFilePath, outEncString);
            }
            finally
            {
                // delete temp file
                File.Delete(tmpFolderFilePath);
                program.TDESAlgorithm.Clear();
            }
        }

        public byte[] DecryptByte(byte[] DataToDecrypt)
        {
            using (ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor())
            {
                return Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
        }

        public byte[] EncryptByte(byte[] DataToEncrypt)
        {
            using (ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor())
            {
                return Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
        }

        private static byte[] ShortenKey(byte[] TDESKey)
        {
            if (TDESKey.Length <= 24)
                return TDESKey;

            Console.WriteLine("Warning: The Key is larger than 24 Byte/192 Bit and got shortend!");
            Array.Resize(ref TDESKey, 192 / 8);
            return TDESKey;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private static void MessageBoxUi(string message)
        {
            int result = MessageBox(0, message, appName, 0);
            if (result != 1)
                throw new Exception("Message Box failed");
        }
    }
}
