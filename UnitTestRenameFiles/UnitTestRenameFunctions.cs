using Microsoft.VisualStudio.TestTools.UnitTesting;
using RenameFunc = RenameFiles.Program;

namespace UnitTestRenameFiles
{
  [TestClass]
  public class UnitTestRenameFunctions
  {
    [TestMethod]
    public void TestMethod_ChangeFileExtension()
    {
      const string source1 = "toto.bat";
      const string source2 = "txt";
      const string expected = "toto.txt";
      string result = RenameFunc.ChangeFileExtension(source1, source2);
      Assert.AreEqual(result, expected);
    }
  }
}