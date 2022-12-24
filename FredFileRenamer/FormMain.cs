using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FredFileRenamer.Properties;

namespace FredFileRenamer
{
  public partial class FormMain : Form
  {
    public FormMain()
    {
      InitializeComponent();
    }

    private void FormMain_Load(object sender, EventArgs e)
    {
      DisplayTitle();
      GetWindowValue();
    }

    private void QuitterToolStripMenuItem_Click(object sender, EventArgs e)
    {
      SaveWindowValue();
      Application.Exit();
    }

    private void DisplayTitle()
    {
      Text += $" - {GetApplicationVersion()}";
      GetWindowValue();
    }

    private string GetApplicationVersion()
    {
      Assembly assembly = Assembly.GetExecutingAssembly();
      FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      return string.Format("V{0}.{1}.{2}.{3}", fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
    }

    private void SaveWindowValue()
    {
      Settings.Default.WindowHeight = Height;
      Settings.Default.WindowWidth = Width;
      Settings.Default.WindowLeft = Left;
      Settings.Default.WindowTop = Top;
      Settings.Default.Save();
    }

    private void GetWindowValue()
    {
      Width = Settings.Default.WindowWidth;
      Height = Settings.Default.WindowHeight;
      Top = Settings.Default.WindowTop < 0 ? 0 : Settings.Default.WindowTop;
      Left = Settings.Default.WindowLeft < 0 ? 0 : Settings.Default.WindowLeft;
    }

    private void TextBoxPath_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == 13)
      {
        MessageBox.Show("return has been pressed");
      }
    }

    public static List<string> GetSubfoldersAndFiles(DirectoryInfo directoryInfo, int depth)
    {
      List<string> result = new List<string>();
      foreach (FileInfo fileInfo in directoryInfo.GetFiles())
      {
        result.Add(fileInfo.FullName);
      }

      if (depth > 0)
      {
        foreach (DirectoryInfo subDi in directoryInfo.GetDirectories())
        {
          result.AddRange(GetSubfoldersAndFiles(subDi, depth - 1).ToArray());
        }
      }

      return result;
    }

    public static FileInfo GetFileInfo(string fileName)
    {
      return new FileInfo(fileName);
    }

    private void ButtonPreview_Click(object sender, EventArgs e)
    {
      var backupDataGridViewItems = dataGridViewPreview.Rows;
      dataGridViewPreview.Rows.Clear();
      // adding new extension

    }

    private void ButtonApply_Click(object sender, EventArgs e)
    {
      // apply the preview

    }

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      AboutBoxApp aboutBox = new AboutBoxApp();
      aboutBox.ShowDialog();
    }
  }
}
