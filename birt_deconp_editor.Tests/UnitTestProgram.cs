namespace birt_deconp_editor.Tests;

public class UnitTestProgram
{
    private Program program = new Program();

    [Fact]
    public void TestDecrypt()
    {
        const string sourceFileEncrypted = "./testfiles/DataSourceBIRT";

        byte[] inEncrByte = System.IO.File.ReadAllBytes(sourceFileEncrypted);
        byte[] outDecrString = program.DecryptByte(inEncrByte);
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
        byte[] outEncString = program.EncryptByte(tempFileContent);

        Assert.Equal(resultFile, outEncString);
    }

    [Fact]
    public void TestEncrypt2()
    {
        const string sourceFileEncrypted = "./testfiles/BasicXMLDataSource";
        byte[] resultFile = System.IO.File.ReadAllBytes(sourceFileEncrypted);

        const string sourceFileDecrypted = "./testfiles/BasicXML.txt";
        byte[] tempFileContent = System.IO.File.ReadAllBytes(sourceFileDecrypted);
        byte[] outEncString = program.EncryptByte(tempFileContent);

        Assert.Equal(resultFile, outEncString);
    }

    [Fact]
    public void TestCompleteBothNoFiles()
    {
        program = new Program();

        const string inString = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>";
        byte[] tempFileContent = System.Text.Encoding.Default.GetBytes(inString);
        byte[] outEncString = program.EncryptByte(tempFileContent);

        byte[] outDecrString = program.DecryptByte(outEncString);
        string outString = System.Text.Encoding.Default.GetString(outDecrString);

        Assert.Equal(inString, outString);
    }
}