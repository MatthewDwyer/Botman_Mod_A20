using System.Text.RegularExpressions;
using UnityEngine;

namespace Botman.Commands
{
  public static class CmdHelpers
  {
    public static Regex HexColorRegex = new Regex("^[a-fA-F0-9]{6}$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    public static bool GetColor(string _color, out string color)
    {
      if (!HexColorRegex.IsMatch(_color))
      {
        SdtdConsole.Instance.Output("Color must be a 6 digit hex rgb color code");

        color = "";

        return false;
      }

      color = _color.ToUpper();

      return true;
    }

    public static bool GetVector3i(string _x, string _y, string _z, out Vector3i vector)
    {
      if (!GetXyz(_x, _y, _z, out var x, out var y, out var z))
      {
        vector = default;

        return false;
      }

      vector = new Vector3i(x, y, z);

      return true;
    }

    public static bool GetVector3(string _x, string _y, string _z, out Vector3 vector)
    {
      if (!GetXyz(_x, _y, _z, out var x, out var y, out var z))
      {
        vector = default;

        return false;
      }

      vector = new Vector3(x, y, z);

      return true;
    }

    public static bool GetXyz(string _x, string _y, string _z, out int x, out int y, out int z)
    {
      if (!int.TryParse(_x, out x))
      {
        SdtdConsole.Instance.Output($"The value of x is not a valid number: {_x}");
        y = default;
        z = default;

        return false;
      }

      if (!int.TryParse(_y, out y))
      {
        SdtdConsole.Instance.Output($"The value of y is not a valid number: {_y}");
        z = default;

        return false;
      }

      if (!int.TryParse(_z, out z))
      {
        SdtdConsole.Instance.Output($"The value of z is not a valid number: {_z}");

        return false;
      }

      return true;
    }

    public static bool GetClientInfo(string nameOrId, out ClientInfo clientInfo)
    {
      clientInfo = ConsoleHelper.ParseParamIdOrName(nameOrId, true, false);
      if (clientInfo == null)
      {
        SdtdConsole.Instance.Output("Player name, entity id or steam id not found.");

        return false;
      }

      return true;
    }

    public static bool GetEntityPlayer(string nameOrId, out EntityPlayer entityPlayer)
    {

      if (!GetClientInfo(nameOrId, out var clientInfo))
      {
        entityPlayer = null;

        return false;
      }

      if (!GameManager.Instance.World.Players.dict.TryGetValue(clientInfo.entityId, out entityPlayer))
      {
        SdtdConsole.Instance.Output("Player not found.");

        return false;
      }

      return true;
    }

    public static bool GetClientAndEntity(string nameOrId, out ClientInfo clientInfo, out EntityPlayer entityPlayer)
    {
      if (!GetClientInfo(nameOrId, out clientInfo))
      {
        entityPlayer = null;

        return false;
      }

      if (!GameManager.Instance.World.Players.dict.TryGetValue(clientInfo.entityId, out entityPlayer))
      {
        SdtdConsole.Instance.Output($"Player not found for {nameOrId}.");

        return false;
      }

      return true;
    }

    public static bool GetClientAndEntity(ClientInfo remoteClientInfo, out EntityPlayer entityPlayer)
    {
      if (remoteClientInfo == null)
      {
        entityPlayer = null;

        return false;
      }

      if (!GameManager.Instance.World.Players.dict.TryGetValue(remoteClientInfo.entityId, out entityPlayer))
      {
        SdtdConsole.Instance.Output($"Player not found.");

        return false;
      }

      return true;
    }
  }
}
