using Botman.Commands;
using System;
using System.IO;

namespace Botman
{
  class BMMapEditor
  {
    public static string FilePath = API.MapJsPath;
    public static string IndexString = "var playersMappingList =";

    public static void Update()
    {
      try
      {
        if (!GetFile(FilePath, out var file))
        {
          SdtdConsole.Instance.Output("~Botman Error~ Could not locate Allocs map.js. Please check your path and try again.");
        }
        
        file = CleanMap(file);

        if (BMWebMapTraceTraders.Enabled)
        {
          file = BMWebMapTraceTraders.Alter(file);
        }

        if (BMWebMapTracePrefabs.Enabled)
        {
          file = BMWebMapTracePrefabs.Alter(file);
        }

        if (BMWebMapTraceResetArea.Enabled && BMAreaReset.Enabled)
        {
          file = BMWebMapTraceResetArea.Alter(file);
        }

        if (BMWebMapTraceResetRegions.Enabled && BMResetRegions.Enabled)
        {
          file = BMWebMapTraceResetRegions.Alter(file);
        }

        using (var writer = new StreamWriter(FilePath, false))
        {
          writer.Write(file);
          writer.Flush();
          writer.Close();
        }
      }
      catch (IOException e)
      {
        Log.Error("~=Botman Notice=~ Error Editing Map:\n" + e);
      }
    }

    public static bool GetFile(string fileName, out string fileData)
    {
      fileData = null;

      if (!File.Exists(fileName))
      {
        return false;
      }

      var reader = new StreamReader(FilePath);
      fileData = reader.ReadToEnd();
      reader.Close();

      return true;
    }

    public static string CleanMap(string fileData)
    {
      var lines = fileData.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      fileData = "";

      foreach (var line in lines)
      {
        if (!line.Contains("Botman"))
        {
          fileData += line + "\n";
        }
      }

      return fileData;
    }
  }
}
