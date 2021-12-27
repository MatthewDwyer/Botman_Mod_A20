using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  class BMWebMapTraceResetRegions : BMCmdAbstract
  {
    public static bool Enabled = false;
    public static string Color = "FF4500";
    public override string GetDescription() => "~Botman~ Highlights Reset Areas on webmap.";

    public override string[] GetCommands() => new[] { "bm-webmaptraceresetregions" };

    public override string GetHelp()
    {
      return "Controls highlights of reset regions on webmap \n" +
          "Usage: \n" +
          "1. bm-webmaptraceresetregions true/false\n" +
          "2. bm-webmaptraceresetregions color [color]\n";
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
        SdtdConsole.Instance.Output($"~Botman~ Reset Regions Color has been saved as: {Color}");

        return;
      }

      if (_params[0].Equals("true", StringComparison.InvariantCultureIgnoreCase))
      {
        if (!BMResetRegions.Enabled)
        {
          SdtdConsole.Instance.Output("You must have reset regions enabled for reset regions to show on map.");
          SdtdConsole.Instance.Output("Use console commands \"bm-resetregions enable\" to activate reset regions.");

          return;
        }

        if (Enabled)
        {
          SdtdConsole.Instance.Output("~Botman~ Show Regions is already enabled");

          return;
        }

        Enabled = true;
        Config.MapUpdateRequired = true;
        SdtdConsole.Instance.Output("~Botman~ Web Map Regions enabled.");
        Config.UpdateXml();

        return;
      }

      if (_params[0].Equals("false", StringComparison.InvariantCultureIgnoreCase))
      {
        Enabled = false;
        Config.MapUpdateRequired = true;
        SdtdConsole.Instance.Output("~Botman~ Web Map Regions will no longer show on the map");
        Config.UpdateXml();
      }
    }

    public static string Alter(string fileData)
    {
      var insertLines = "";
      for (var i = 0; i < BMResetRegions.ManualResetRegions.Count; i++)
      {
        insertLines += TraceResetRegionInstance(i, BMResetRegions.ManualResetRegions[i]);
      }

      if (fileData.Contains(insertLines))
      {
        return fileData;
      }

      var index = fileData.LastIndexOf(BMMapEditor.IndexString, StringComparison.InvariantCultureIgnoreCase);
      fileData = fileData.Insert(index, insertLines);

      return fileData;
    }


    public static string TraceResetRegionInstance(int index, string region)
    {
      region = region.Replace("r.", "");
      var _params = region.Split('.');
      if (!int.TryParse(_params[0], out var regionX) || !int.TryParse(_params[1], out var regionZ))
      {
        return "";
      }

      regionX *= 512;
      regionZ *= 512;

      return $"var BotmanResetRegion{index} = L.polygon([[{regionX}, {regionZ}], [{regionX + 512}, {regionZ}], [{regionX + 512}, {regionZ + 512}], [{regionX}, {regionZ + 512}]]).addTo(map);\n" +
             $"BotmanResetRegion{index}.bindPopup(\"Reset Zone\");\n" +
             $"BotmanResetRegion{index}.setStyle({{color: '#{Color}'}});\n" +
             $"BotmanResetRegion{index}.setStyle({{ weight: 1}});\n";
    }
  }
}
