using System;
using System.Collections.Generic;
using System.IO;

namespace Botman.Commands
{
  public class BMPExport : BMCmdAbstract
  {
    private static readonly Dictionary<int, Vector3i> StoredLocations = new Dictionary<int, Vector3i>();

    public override string GetDescription() => "~Botman~ Exports as Prefab some space";

    public override string[] GetCommands() => new[] { "bm-pexport" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-pexport <x1> <x2> <y1> <y2> <z1> <z2> <prefab_file_name>\n" +
      "2. bm-pexport \n" +
      "3. bm-pexport <prefab_file_name>\n" +
      "   1. Export the defined area to a prefabFile on folder .../UserData/LocalPrefabs/ by default\n" +
      "   2. Store the player position to be used together on method 3.\n" +
      "   3. Use stored position on method 2. with current position to export the area to prefabFile on folder .../UserData/LocalPrefabs/ by default\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 7 && _params.Count != 0 && _params.Count != 1)
      {
        SdtdConsole.Instance.Output("Wrong number of arguments, expected 0 or 1 or 7, found " + _params.Count + ".");
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      Vector3i startPos = default;
      Vector3i endPos = default;
      var fileName = "";

      switch (_params.Count)
      {
        case 0:
          {
            var cInfo = _senderInfo.RemoteClientInfo;

            if (cInfo == null)
            {
              SdtdConsole.Instance.Output("Unable to get your position");

              return;
            }

            var entityPlayer = GameManager.Instance.World.Players.dict[cInfo.entityId];

            if (entityPlayer == null)
            {
              SdtdConsole.Instance.Output("Unable to get your position");

              return;
            }

            if (StoredLocations.ContainsKey(cInfo.entityId))
            {
              StoredLocations.Remove(cInfo.entityId);
            }

            StoredLocations.Add(cInfo.entityId, entityPlayer.GetBlockPosition());

            SdtdConsole.Instance.Output($"Stored position: {entityPlayer.GetBlockPosition()}");

            return;
          }

        case 1:
          {
            var cInfo = _senderInfo.RemoteClientInfo;
            if (cInfo == null)
            {
              SdtdConsole.Instance.Output("Unable to get your position");

              return;
            }

            var entityPlayer = GameManager.Instance.World.Players.dict[cInfo.entityId];
            if (entityPlayer == null)
            {
              SdtdConsole.Instance.Output("Unable to get your position");

              return;
            }

            if (!StoredLocations.TryGetValue(cInfo.entityId, out var storedPos))
            {
              SdtdConsole.Instance.Output("There isn't any stored location. Use method 2. to store a position.");
              SdtdConsole.Instance.Output(GetHelp());

              return;
            }

            StoredLocations.Remove(cInfo.entityId);

            startPos = storedPos;
            endPos = entityPlayer.GetBlockPosition();

            fileName = _params[0];
            break;
          }

        case 7:
          if (!int.TryParse(_params[0], out startPos.x))
          {
            SdtdConsole.Instance.Output($"<x1> was not a number: {_params[0]}");

            return;
          }

          if (!int.TryParse(_params[1], out endPos.x))
          {
            SdtdConsole.Instance.Output($"<x2> was not a number: {_params[1]}");

            return;
          }

          if (!int.TryParse(_params[2], out startPos.y))
          {
            SdtdConsole.Instance.Output($"<y1> was not a number: {_params[2]}");

            return;
          }

          if (!int.TryParse(_params[3], out endPos.y))
          {
            SdtdConsole.Instance.Output($"<y2> was not a number: {_params[3]}");

            return;
          }

          if (!int.TryParse(_params[4], out startPos.z))
          {
            SdtdConsole.Instance.Output($"<z1> was not a number: {_params[4]}");

            return;
          }

          if (!int.TryParse(_params[5], out endPos.z))
          {
            SdtdConsole.Instance.Output($"<z2> was not a number: {_params[5]}");

            return;
          }

          fileName = _params[6];
          break;
      }

      if (Math.Abs(startPos.x - endPos.x) > 2048 ||
          Math.Abs(startPos.y - endPos.y) > 256 ||
          Math.Abs(startPos.z - endPos.z) > 2048)
      {
        SdtdConsole.Instance.Output("Prefab dimensions are too large. Maximum of 2048 x 256 x 2048.");

        return;
      }

      //todo: overwrite warning with option to override

      //todo: test using Mods/Botman/Prefabs as the folder location for import/export
      var loc = new PathAbstractions.AbstractedLocation(PathAbstractions.EAbstractedLocationType.UserDataPath,
        fileName, Path.Combine(GamePrefs.GetString(EnumGamePrefs.UserDataFolder), "LocalPrefabs", $"{fileName}.tts"));

      SdtdConsole.Instance.Output($"Attempting to save prefab to {loc.FullPath}");

      var prefab = new Prefab { location = loc };

      prefab.copyFromWorld(GameManager.Instance.World, startPos, endPos);

      if (!prefab.Save(prefab.location))
      {
        SdtdConsole.Instance.Output("Prefab could not be saved");

        return;
      }

      SdtdConsole.Instance.Output("Prefab exported.");
      SdtdConsole.Instance.Output($"Area: {startPos} to {endPos}");
    }
  }
}
