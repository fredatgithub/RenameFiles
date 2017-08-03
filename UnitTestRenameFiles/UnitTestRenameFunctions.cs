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

    [TestMethod]
    public void TestMethod_Pluralize_zero()
    {
      const int source = 0;
      string expected = string.Empty;
      string result = RenameFunc.Pluralize(source);
      Assert.AreEqual(result, expected);
    }

    [TestMethod]
    public void TestMethod_Pluralize_one()
    {
      const int source = 1;
      string expected = string.Empty;
      string result = RenameFunc.Pluralize(source);
      Assert.AreEqual(result, expected);
    }

    [TestMethod]
    public void TestMethod_Pluralize_two()
    {
      const int source = 2;
      const string expected = "s";
      string result = RenameFunc.Pluralize(source);
      Assert.AreEqual(result, expected);
    }
  }
}