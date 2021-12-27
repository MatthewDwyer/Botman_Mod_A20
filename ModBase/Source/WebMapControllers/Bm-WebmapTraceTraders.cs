using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  class BMWebMapTraceTraders : BMCmdAbstract
  {
    public static bool Enabled = false;
    public static string Color = "FF0000";

    public override string GetDescription() => "~Botman~ Highlights Traders on webmap.";

    public override string[] GetCommands() => new[] { "bm-webmaptracetraders" };

    public override string GetHelp()
    {
      return "Controls highlights of prefabs on webmap \n" +
          "Usage: \n" +
          "1. bm-webmaptracetraders true/false\n" +
          "2. bm-webmaptracetraders color [color]";
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
        if (!CmdHelpers.GetColor(_params[1], out var color)) { return; }

        Color = color;
        Config.MapUpdateRequired = true;
        Config.UpdateXml();
        SdtdConsole.Instance.Output($"~Botman~ Trader Prefab Color has been saved as: {Color}");

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
        Config.MapUpdateRequired = true;
        SdtdConsole.Instance.Output("~Botman~ Web Map Prefabs enabled.");
        Config.UpdateXml();

        return;
      }

      if (_params[0].Equals("false", StringComparison.InvariantCultureIgnoreCase))
      {
        Enabled = false;
        Config.MapUpdateRequired = true;
        SdtdConsole.Instance.Output("~Botman~ Web Map Prefabs will no longer show on the map");
        Config.UpdateXml();
      }

      SdtdConsole.Instance.Output($"~Botman~ unknown sub command {_params[0]}");
    }

    public static string Alter(string file)
    {
      var insertLines = "";
      var x = 1;

      foreach (var prefab in GameManager.Instance.GetDynamicPrefabDecorator().GetDynamicPrefabs())
      {
        if (prefab.name.ToLower().Contains("trader"))
        {
          insertLines += TraceTraderInstance(x, prefab);
        }
        x++;
      }

      if (file.Contains(insertLines))
      {
        return file;
      }

      var index = file.LastIndexOf(BMMapEditor.IndexString, StringComparison.InvariantCultureIgnoreCase);
      file = file.Insert(index, insertLines);

      return file;
    }

    public static string TraceTraderInstance(int index, PrefabInstance prefab)
    {
      var name = $"BotmanTrader{index}";
      var pos1 = prefab.boundingBoxPosition.x;
      var pos2 = prefab.boundingBoxPosition.z;
      var pos3 = prefab.boundingBoxPosition.x + prefab.boundingBoxSize.z;
      var pos4 = prefab.boundingBoxPosition.z + prefab.boundingBoxSize.z;

      return $"var {name} = L.polygon([[{pos1}, {pos2}], [{pos3}, {pos2}], [{pos3}, {pos4}], [{pos1}, {pos4}]]).addTo(map);\n" +
             name + ".bindPopup(\"{_prefab.name}\");\n" +
             name + ".setStyle({ color: '#" + Color + "' });\n" +
             name + ".setStyle({ weight: 1 });\n" +
             name + ".bindTooltip(\"Trader\", { opacity: .50, permanent: true }).openTooltip();\n";
    }
  }
}
