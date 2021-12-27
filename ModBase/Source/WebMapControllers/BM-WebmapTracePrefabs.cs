using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  class BMWebMapTracePrefabs : BMCmdAbstract
  {

    public static bool Enabled = false;
    public static string Color = "00ff00";
    public override string GetDescription() => "~Botman~ Highlights Prefabs on webmap.";

    public override string[] GetCommands() => new[] { "bm-webmaptraceprefabs" };

    public override string GetHelp()
    {
      return "Controls highlights of prefabs on webmap \n" +
          "Usage: \n" +
          "1. bm-webmaptraceprefabs true/false" +
          "2. bm-webmaptraceprefabs color [color]";
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
        SdtdConsole.Instance.Output($"~Botman~ Prefab Color has been saved as: {Color}");

        return;
      }

      if (_params[0].Equals("true", StringComparison.InvariantCultureIgnoreCase))
      {
        if (Enabled)
        {
          SdtdConsole.Instance.Output("~Botman~ Show Prefabs is already enabled");

          return;
        }

        Enabled = true;
        SdtdConsole.Instance.Output("~Botman~ Web Map Prefabs enabled.");
        Config.MapUpdateRequired = true;
        Config.UpdateXml();

        return;
      }

      if (_params[0].Equals("false", StringComparison.InvariantCultureIgnoreCase))
      {
        Enabled = false;
        SdtdConsole.Instance.Output("~Botman~ Web Map Prefabs will no longer show on the map");
        Config.MapUpdateRequired = true;
        Config.UpdateXml();
      }
    }

    public static string Alter(string fileData)
    {
      var insertLines = "";
      var x = 0;

      foreach (var prefab in GameManager.Instance.GetDynamicPrefabDecorator().GetDynamicPrefabs())
      {
        if (prefab.name.ToLower().Contains("trader") && BMWebMapTraceTraders.Enabled)
        {
          continue;
        }

        x++;
        insertLines += TracePrefabInstance(x, prefab);

      }

      if (fileData.Contains(insertLines))
      {
        return fileData;
      }

      var index = fileData.LastIndexOf(BMMapEditor.IndexString, StringComparison.InvariantCultureIgnoreCase);
      fileData = fileData.Insert(index, insertLines);

      return fileData;
    }

    public static string TracePrefabInstance(int index, PrefabInstance prefab)
    {
      var x1 = prefab.boundingBoxPosition.x;
      var z1 = prefab.boundingBoxPosition.z;
      var x2 = prefab.boundingBoxPosition.x + prefab.boundingBoxSize.x;
      var z2 = prefab.boundingBoxPosition.z + prefab.boundingBoxSize.z;

      return $"var BotmanPrefab{index} = L.polygon([[{x1}, {z1}], [{x2}, {z1}], [{x2}, {z2}], [{x1}, {z2}]]).addTo(map);\n" +
             $"BotmanPrefab{index}.bindPopup(\"{prefab.name}\");\n" +
             $"BotmanPrefab{index}.setStyle({{color: '#{Color}'}});\n" +
             $"BotmanPrefab{index}.setStyle({{ weight: 1}});\n";
    }
  }
}
