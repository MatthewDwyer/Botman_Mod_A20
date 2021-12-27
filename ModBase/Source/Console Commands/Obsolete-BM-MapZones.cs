//using System.IO;
//using System.Collections.Generic;

//namespace Botman.Commands
//{
//  class BMMapZones : BMCmdAbstract
//  {
//    public override string GetDescription() => "~Botman~ Changes settings for webmap edits.";

//    public override string[] GetCommands() => new[] { "bm-webmapzones" };

//    public override string GetHelp()
//    {
//      return "Changes settings for webmap edits \n" +
//          "Usage: \n" +
//          "1. bm-webmapzones enable/disable" +
//          "2. bm-webmapzones color [color]" +
//          "3. bm-webmapzones path [path]" +
//          "4. bm-webmapzones reset";
//    }

//    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
//    {
//      if (_params.Count == 0)
//      {
//        SdtdConsole.Instance.Output("Webmap Settings");
//        SdtdConsole.Instance.Output("Enabled: " + BMMapEditor.enabled.ToString());
//        SdtdConsole.Instance.Output("Color: " + Vectors.ResetRegionColor);
//        SdtdConsole.Instance.Output("Path: " + BMMapEditor.filePath);
//        //C:/Program Files (x86)/Steam/steamapps/common/7 Days to Die Dedicated Server/Mods/Allocs_WebAndMapRendering/webserver/js/map.js
//        return;
//      }
//      if (_params[0].ToLower().Contains("path"))
//      {
//        _params.Remove("bm-webmapzones");
//        _params.Remove("path");
//        SdtdConsole.Instance.Output("~Botman~ File Path has been saved as: " + _params[0].ToString());
//        BMMapEditor.filePath = _params[0].ToString();
//        Config.UpdateXml();
//        return;
//      }
//      if (_params[0].ToLower().Contains("color"))
//      {
//        _params.Remove("color");

//        if (_params.Count > 1 || _params[0].Length > 7)
//        {
//          SdtdConsole.Instance.Output("~Botman~ Too many characters. Please try again.");
//          return;
//        }
//        Vectors.ResetRegionColor = _params[0];
//        Config.UpdateXml();
//        SdtdConsole.Instance.Output("~Botman~ Color has been saved as: " + _params[0].ToString());
//        return;
//      }
//      if (_params[0].ToLower().Contains("enable"))
//      {
//        if (BMMapEditor.enabled)
//        {
//          SdtdConsole.Instance.Output("~Botman~ Web map zones already enabled");
//          return;
//        }
//        BMMapEditor.enabled = true;
//        SdtdConsole.Instance.Output("~Botman~ Web map zones have been enabled");
//        Config.UpdateXml();
//        return;
//      }
//      if (_params[0].ToLower().Contains("disable"))
//      {
//        BMMapEditor.enabled = false;
//        if (File.Exists(BMMapEditor.filePath))
//        {
//          File.Delete(BMMapEditor.filePath);
//        }
//        if (!File.Exists(BMMapEditor.filePath))
//        {
//          using (var sw = new StreamWriter(BMMapEditor.filePath))
//          {
//            sw.WriteLine(Default.MapFile);
//          }
//        }
//        SdtdConsole.Instance.Output("~Botman~ Web map zones have been disabled, and map has been reset to default.");
//        Config.UpdateXml();
//        return;
//      }
//      if (_params[0].ToLower().Equals("reset"))
//      {
//        if (!File.Exists(BMMapEditor.filePath))
//        {
//          SdtdConsole.Instance.Output("~Botman Error~ Could not locate Allocs map.js. Please check your path and try again.");
//          return;
//        }
//        if (File.Exists(BMMapEditor.filePath))
//        {
//          File.Delete(BMMapEditor.filePath);
//        }
//        if (!File.Exists(BMMapEditor.filePath))
//        {
//          using (var sw = new StreamWriter(BMMapEditor.filePath))
//          {
//            sw.WriteLine(Default.MapFile);
//          }
//        }
//        SdtdConsole.Instance.Output("~Botman~ Web Map Reset to default.");
//      }
//    }
//  }
//}