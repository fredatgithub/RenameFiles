using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RenameFiles
{
  public static class Program
  {
    private static void Main(string[] arguments)
    {
      Action<string> display = Console.WriteLine;
      var argumentDictionary = new Dictionary<string, string>
      {
        // Initialization of the dictionary with default values
        {"directory", "."},
        {"includesubdirectories", "false"},
        {"oldextension", "bat" },
        {"newextension", "txt" },
        {"log", "false"}
      };
      // the variable numberOfInitialDictionaryItems is used for the log to list all non-standard arguments passed in.
      int numberOfInitialDictionaryItems = argumentDictionary.Count;
      int numberOfFilesRenamed = 0;
      bool hasExtraArguments = false;
      string datedLogFileName = string.Empty;

      Stopwatch chrono = new Stopwatch();
      if (arguments.Length == 0 || arguments[0].ToLower().Contains("help") || arguments[0].Contains("?"))
      {
        Usage();
        return;
      }

      chrono.Start();
      // we split arguments into the dictionary
      foreach (string argument in arguments)
      {
        string argumentKey = string.Empty;
        string argumentValue = string.Empty;
        if (argument.IndexOf('=') != -1)
        {
          argumentKey = argument.Substring(1, argument.IndexOf('=') - 1).ToLower();
          argumentValue = argument.Substring(argument.IndexOf('=') + 1,
            argument.Length - (argument.IndexOf('=') + 1));
        }
        else
        {
          // If we have an argument without the equal sign (=) then we add it to the dictionary
          argumentKey = argument;
          argumentValue = "The argument passed in does not have any value. The equal sign (=) is missing.";
        }

        if (argumentDictionary.ContainsKey(argumentKey))
        {
          // set the value of the argument
          argumentDictionary[argumentKey] = argumentValue;
        }
        else
        {
          // we add any other or new argument into the dictionary to look at them in the log
          argumentDictionary.Add(argumentKey, argumentValue);
          hasExtraArguments = true;
        }
      }

      // Add version of the program at the beginning of the log
      Log(datedLogFileName, argumentDictionary["log"], $"{Assembly.GetExecutingAssembly().GetName().Name} is in version {GetAssemblyVersion()}");

      // We log all arguments passed in.
      foreach (KeyValuePair<string, string> keyValuePair in argumentDictionary)
      {
        if (argumentDictionary["log"] == "true")
        {
          Log(datedLogFileName, argumentDictionary["log"], $"Argument requested: {keyValuePair.Key}");
          Log(datedLogFileName, argumentDictionary["log"], $"Value of the argument: {keyValuePair.Value}");
        }
      }

      //we log extra arguments
      if (hasExtraArguments && argumentDictionary["log"] == "true")
      {
        Log(datedLogFileName, argumentDictionary["log"], "Here are a list of argument passed in but not understood and thus not used (for debug purpose only).");
        for (int i = numberOfInitialDictionaryItems; i <= argumentDictionary.Count - 1; i++)
        {
          Log(datedLogFileName, argumentDictionary["log"], $"Extra argument requested: {argumentDictionary.Keys.ElementAt(i)}");
          Log(datedLogFileName, argumentDictionary["log"], $"Value of the extra argument: {argumentDictionary.Values.ElementAt(i)}");
        }
      }

      // renaming files start here
      foreach (string filename in Directory.GetFiles(argumentDictionary["directory"], $"*.{argumentDictionary["oldextension"]}"))
      {
        try
        {
          if (filename.EndsWith(argumentDictionary["oldextension"]))
          {
            File.Move(filename, ChangeFileExtension(filename, argumentDictionary["newextension"]));
            numberOfFilesRenamed++;
          }
        }
        catch (Exception exception)
        {
          display("error while trying to rename files");
          display($"The exception is {exception}");
        }
      }
    }

    public static string ChangeFileExtension(string filename, string newExtension)
    {
      string result = filename;
      result = $"{Path.GetFileNameWithoutExtension(filename)}.{newExtension}";
      return result;
    }

    public static string[] GetDirectoryFileNameAndExtension(string filePath)
    {
      string directory = Path.GetDirectoryName(filePath);
      string fileName = Path.GetFileNameWithoutExtension(filePath);
      string extension = Path.GetExtension(filePath);

      return new[] { directory, fileName, extension };
    }

    /// <summary>
    /// Get assembly version.
    /// </summary>
    /// <returns>A string with all assembly versions like major, minor, build.</returns>
    private static string GetAssemblyVersion()
    {
      Assembly assembly = Assembly.GetExecutingAssembly();
      FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      return $"{fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}.{fvi.FilePrivatePart}";
    }

    /// <summary>
    /// The log file to record all activities.
    /// </summary>
    /// <param name="filename">The name of the file.</param>
    /// <param name="logging">Do we log or not?</param>
    /// <param name="message">The message to be logged.</param>
    private static void Log(string filename, string logging, string message)
    {
      if (logging.ToLower() != "true") return;
      if (filename.Trim() == string.Empty) return;
      try
      {
        StreamWriter sw = new StreamWriter(filename, true);
        sw.WriteLine($"{DateTime.Now} - {message}");
        sw.Close();
      }
      catch (Exception exception)
      {
        Console.WriteLine($"There was an error while writing the file: {filename}. The exception is:{exception}");
      }
    }

    /// <summary>
    /// Convert a Time span to days hours minutes seconds milliseconds.
    /// </summary>
    /// <param name="ts">The time span.</param>
    /// <param name="removeZeroArgument">Do you want zero argument not send back, true by default.</param>
    /// <returns>Returns a string with the number of days, hours, minutes, seconds and milliseconds.</returns>
    public static string ConvertToTimeString(TimeSpan ts, bool removeZeroArgument = true)
    {
      string result = string.Empty;
      if (!removeZeroArgument || ts.Days != 0)
      {
        result = $"{ts.Days} jour{Plural(ts.Days)} ";
      }

      if (!removeZeroArgument || ts.Hours != 0)
      {
        result += $"{ts.Hours} heure{Plural(ts.Hours)} ";
      }

      if (!removeZeroArgument || ts.Minutes != 0)
      {
        result += $"{ts.Minutes} minute{Plural(ts.Minutes)} ";
      }

      if (!removeZeroArgument || ts.Seconds != 0)
      {
        result += $"{ts.Seconds} seconde{Plural(ts.Seconds)} ";
      }

      if (!removeZeroArgument || ts.Milliseconds != 0)
      {
        result += $"{ts.Milliseconds} milliseconde{Plural(ts.Milliseconds)}";
      }

      return result.TrimEnd();
    }

    /// <summary>
    /// Remove all Windows forbidden characters for a Windows path.
    /// </summary>
    /// <param name="filename">The initial string to be processed.</param>
    /// <returns>A string without Windows forbidden characters.</returns>
    private static string RemoveWindowsForbiddenCharacters(string filename)
    {
      string result = filename;
      // We remove all characters which are forbidden for a Windows path
      string[] forbiddenWindowsFilenameCharacters = { "\\", "/", "*", "?", "\"", "<", ">", "|" };
      foreach (var item in forbiddenWindowsFilenameCharacters)
      {
        result = result.Replace(item, string.Empty);
      }

      return result;
    }

    /// <summary>
    /// Add an 's' if the number is greater than 1.
    /// </summary>
    /// <param name="number"></param>
    /// <returns>Returns an 's' if number if greater than one ortherwise returns an empty string.</returns>
    public static string Plural(int number)
    {
      return number > 1 ? "s" : string.Empty;
    }

    /// <summary>
    /// Add date to the file name.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>A string with the date at the end of the file name.</returns>
    private static string AddDateToFileName(string fileName)
    {
      string result = string.Empty;
      // We strip the fileName and add a datetime before the extension of the filename.
      string tmpFileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
      string tmpFileNameExtension = Path.GetExtension(fileName);
      string tmpDateTime = DateTime.Now.ToShortDateString();
      tmpDateTime = tmpDateTime.Replace('/', '-');
      result = $"{tmpFileNameWithoutExtension}_{tmpDateTime}{tmpFileNameExtension}";

      return result;
    }

    /// <summary>
    /// If the user requests help or gives no argument, then we display the help section.
    /// </summary>
    private static void Usage()
    {
      Action<string> display = Console.WriteLine;
      display(string.Empty);
      display("RenameFiles is a console application written by Freddy Juhel.");
      display($"RenameFiles.exe is in version {GetAssemblyVersion()}");
      display("RenameFiles needs Microsoft .NET framework 3.5 to run, if you don't have it, download it from microsoft.com.");
      display("Copyrighted (c) MIT 2017 by Freddy Juhel.");
      display(string.Empty);
      display("Usage of this program:");
      display(string.Empty);
      display("List of arguments:");
      display(string.Empty);
      display("/help (this help)");
      display("/? (this help)");
      display(string.Empty);
      display(
        "You can write argument name (not its value) in uppercase or lowercase or a mixed of them (case insensitive)");
      display("/oldextension is the same as /OldExtension or /oldExtension or /OLDEXTENSION");
      display(string.Empty);
      display("/directory=<name of the directory where files to be renamed are> default is where RenameFiles.exe is");
      display("/includesubdirectories=<true or false> false by default");
      display("/oldextension=<name of the old extension you want to rename from>");
      display("/newextension=<name of the new extension you want to rename to>");
      display("/log=<true or false> false by default");
      display(string.Empty);
      display("Examples:");
      display(string.Empty);
      display(@"RenameFiles /directory=c:\sampleDir\ /oldextension=bat /oldextension=txt /log=true");
      display(string.Empty);
      display("RenameFiles /help (this help)");
      display("RenameFiles /? (this help)");
      display(string.Empty);
    }
  }
}