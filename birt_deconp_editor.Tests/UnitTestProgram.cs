namespace birt_deconp_editor.Tests;

public class UnitTestProgram
{
    [Fact]
    public void TestDecrypt()
    {
        const string sourceFileEncrypted = "./testfiles/DataSourceBIRT";

        byte[] inEncrByte = System.IO.File.ReadAllBytes(sourceFileEncrypted);
        byte[] outDecrString = Program.DecryptByte(inEncrByte, Program.passphrase);
        string outString = System.Text.Encoding.Default.GetString(outDecrString);

        Assert.StartsWith("<?xml version=", outString);
    }

    [Fact]
    public void TestEncrypt()
    {
        const string sourceFileEncrypted = "./testfiles/BasicXMLDataSource";
        byte[] resultFile = System.IO.File.ReadAllBytes(sourceFileEncrypted);

        const string sourceFileDecrypted = "./testfiles/BasicXML.txt";
        byte[] tempFileContent = System.IO.File.ReadAllBytes(sourceFileDecrypted);
        byte[] outEncString = Program.EncryptByte(tempFileContent, Program.passphrase);

        Assert.Equal(resultFile, outEncString);
    }
}