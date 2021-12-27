using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  class BMWebMapTraceResetArea : BMCmdAbstract
  {
    public static bool Enabled = false;
    public static string Color = "FF4500";

    public override string GetDescription() => "~Botman~ Highlights Reset Areas on webmap.";

    public override string[] GetCommands() => new[] { "bm-webmaptraceresetareas" };

    public override string GetHelp()
    {
      return "Controls highlights of reset areas on webmap \n" +
          "Usage: \n" +
          "1. bm-webmaptraceresetareas true/false\n" +
          "2. bm-webmaptraceresetareas color [color]\n";
    }

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params[0].Equals("color", StringComparison.InvariantCultureIgnoreCase) && _params.Count > 1)
      {
        if (!CmdHelpers.GetColor(_params[1], out var color))
        {
          return;
        }

        Color = color;
        Config.MapUpdateRequired = true;
        Config.UpdateXml();
        SdtdConsole.Instance.Output($"~Botman~ Reset Area Color has been saved as: {Color}");

        return;
      }

      if (_params[0].Equals("true", StringComparison.InvariantCultureIgnoreCase))
      {
        if (!BMAreaReset.Enabled)
        {
          Log.Out("Reset areas must be enabled for areas to show on map.");
          Log.Out("Use command \"bm-resetareas enabled\" to enable reset areas.");

          return;
        }

        if (Enabled)
        {
          SdtdConsole.Instance.Output("~Botman~ Show Reset Areas is already enabled");

          return;
        }

        Enabled = true;
        Config.MapUpdateRequired = true;
        SdtdConsole.Instance.Output("~Botman~ Web Map Reset Areas enabled.");
        Config.UpdateXml();

        return;
      }

      if (_params[0].Equals("false", StringComparison.InvariantCultureIgnoreCase))
      {
        Enabled = false;
        Config.MapUpdateRequired = true;
        SdtdConsole.Instance.Output("~Botman~ Web Map Reset Areas will no longer show on the map");
        Config.UpdateXml();
      }
    }

    public static string Alter(string fileData)
    {
      var insertLines = "";
      foreach (var kvp in BMAreaReset.ResetAreas)
      {
        var _params = kvp.Value.Split(' ');
        if (_params.Length < 4)
        {
          return fileData;
        }

        if (!int.TryParse(_params[0].Replace("x", ""), out var num) ||
            !int.TryParse(_params[1].Replace("z", ""), out var num2) ||
            !int.TryParse(_params[2].Replace("xx", ""), out var num3) ||
            !int.TryParse(_params[3].Replace("zz", ""), out var num4))
        {
          return fileData;
        }

        insertLines += TraceResetArea(kvp.Key, num, num2, num3, num4);
      }

      if (fileData.Contains(insertLines))
      {
        return fileData;
      }

      var index = fileData.LastIndexOf(BMMapEditor.IndexString, StringComparison.InvariantCultureIgnoreCase);
      fileData = fileData.Insert(index, insertLines);

      return fileData;
    }

    public static string TraceResetArea(string areaName, int pos1, int pos2, int pos3, int pos4) =>
      $"var BotmanResetArea{areaName} = L.polygon([[{pos1}, {pos2}], [{pos3}, {pos2}], [{pos3}, {pos4}], [{pos1}, {pos4}]]).addTo(map);\n" +
      $"BotmanResetArea{areaName}.bindPopup(\"{areaName}\");\n" +
      $"BotmanResetArea{areaName}.setStyle({{color: '#{Color}'}});\n" +
      $"BotmanResetArea{areaName}.setStyle({{ weight: 1}});\n";
  }
}
