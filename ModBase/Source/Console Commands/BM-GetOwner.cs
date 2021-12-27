using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMGetOwner : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Discover owner of SecureLoot/SecureDoor at location.";

    public override string[] GetCommands() => new[] { "bm-getowner" };

    public override string GetHelp() =>
      "Discover owner of SecureLoot/SecureDoor at location\n" +
      "Usage: \n" +
      "1. bm-getowner <x> <y> <z> \n" +
      "2. bm-getowner <entity_id>\n" +
      "3. bm-getowner \n" +
      "   1. x,y,z defines the location of SecureLoot/SecureDoor/Signs you would like to check \n" +
      "   2. The location of secure SecureLoot/SecureDoor/Signs below specified player\n" +
      "   3. The location of secure SecureLoot/SecureDoor/Signs below player running command\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 0 && _params.Count != 1 && _params.Count != 2 && _params.Count != 3 && _params.Count != 4)
      {
        SdtdConsole.Instance.Output("Wrong number of arguments, expected 0 to 4, found " + _params.Count + ".");
        SdtdConsole.Instance.Output(this.GetHelp());
        return;
      }
      var num = int.MinValue;
      var num2 = int.MinValue;
      var num3 = int.MinValue;
      ClientInfo clientInfo = null;
      if (_params.Count == 3 || _params.Count == 4)
      {
        int.TryParse(_params[0], out num);
        int.TryParse(_params[1], out num2);
        int.TryParse(_params[2], out num3);
      }
      else if (_params.Count == 2)
      {
        var clientInfo2 = ConsoleHelper.ParseParamIdOrName(_params[0], true, false);
        if (clientInfo2 == null)
        {
          SdtdConsole.Instance.Output("Playername or entity id not found.");
          return;
        }
        var entityPlayer = GameManager.Instance.World.Players.dict[clientInfo2.entityId];
        if (entityPlayer == null)
        {
          SdtdConsole.Instance.Output("Unable to get player`s position");
          return;
        }
        num = entityPlayer.GetBlockPosition().x;
        num2 = entityPlayer.GetBlockPosition().y;
        num3 = entityPlayer.GetBlockPosition().z;
      }
      else if (_params.Count == 0 || _params.Count == 1)
      {
        if (_senderInfo.RemoteClientInfo == null || _senderInfo.RemoteClientInfo.CrossplatformId.CombinedString == null || _senderInfo.RemoteClientInfo.CrossplatformId.CombinedString.Equals(""))
        {
          SdtdConsole.Instance.Output("Unable to apply this command. You need be a player to execute it.");
          return;
        }
        try
        {
          var clientInfo3 = ConsoleHelper.ParseParamIdOrName(_senderInfo.RemoteClientInfo.playerName, true, false);
          if (clientInfo3 == null)
          {
            SdtdConsole.Instance.Output("Playername or entity id not found.");
            return;
          }
          var entityPlayer2 = GameManager.Instance.World.Players.dict[clientInfo3.entityId];
          if (entityPlayer2 == null)
          {
            SdtdConsole.Instance.Output("Unable to get player`s position");
            return;
          }
          num = entityPlayer2.GetBlockPosition().x;
          num2 = entityPlayer2.GetBlockPosition().y;
          num3 = entityPlayer2.GetBlockPosition().z;

        }
        catch (Exception arg)
        {
          Log.Out("`Botman Notice~ Error in bm-getowner.execute: " + arg);
        }
      }
      if (_params.Count == 4 || _params.Count == 1)
      {
        clientInfo = ConsoleHelper.ParseParamIdOrName(_params[0], true, false);
      }
      else if (_params.Count == 3 || _params.Count == 0)
      {
        clientInfo = ConsoleHelper.ParseParamIdOrName(_senderInfo.RemoteClientInfo.playerName, true, false);
      }
      else if (_params.Count == 2)
      {
        clientInfo = ConsoleHelper.ParseParamIdOrName(_params[1], true, false);
      }
      if (clientInfo == null)
      {
        SdtdConsole.Instance.Output("Playername or entity id not found.");
        return;
      }
      if (num == -2147483648 || num2 == -2147483648 || num3 == -2147483648)
      {
        SdtdConsole.Instance.Output("At least one of the given coordinates is not a valid integer");
        return;
      }
      var flag = false;
      var chunkClusters = GameManager.Instance.World.ChunkClusters;
      new Dictionary<long, Chunk>();
      var owner = "";
      for (var i = 0; i < chunkClusters.Count; i++)
      {
        foreach (var chunk in chunkClusters[i].GetChunkArray())
        {
          foreach (var tileEntity in chunk.GetTileEntities().dict.Values)
          {
            var tileEntityType = tileEntity.GetTileEntityType();
            try
            {
              if (tileEntityType.ToString().Equals("Sign"))
              {
                var tileEntitySign = (TileEntitySign)tileEntity;
                var vector3i = tileEntitySign.ToWorldPos();
                if (vector3i.x == num && vector3i.z == num3 && vector3i.y == num2)
                {
                  var playerId = clientInfo.CrossplatformId.CombinedString;
                  flag = true;
                  owner = tileEntitySign.GetOwner().CombinedString;
                  break;
                }
              }
              if (tileEntityType.ToString().Equals("SecureDoor"))
              {
                var tileEntitySecureDoor = (TileEntitySecureDoor)tileEntity;
                var vector3i2 = tileEntitySecureDoor.ToWorldPos();
                if (vector3i2.x == num && vector3i2.z == num3 && vector3i2.y == num2)
                {
                  var playerId2 = clientInfo.CrossplatformId.CombinedString;
                  flag = true;
                  owner = tileEntitySecureDoor.GetOwner().CombinedString;
                  break;
                }
              }
              else if (tileEntityType.ToString().Equals("SecureLoot"))
              {
                var tileEntitySecureLootContainer = (TileEntitySecureLootContainer)tileEntity;
                var vector3i3 = tileEntitySecureLootContainer.ToWorldPos();
                if (vector3i3.x == num && vector3i3.z == num3 && vector3i3.y == num2)
                {
                  var playerId3 = clientInfo.CrossplatformId.CombinedString;
                  flag = true;
                  owner = tileEntitySecureLootContainer.GetOwner().CombinedString;
                  break;
                }
              }
            }
            catch (Exception arg2)
            {
              Log.Out("Error in RemoveFromLooter.Run: " + arg2);
            }
          }
        }
      }
      if (!flag)
      {
        SdtdConsole.Instance.Output(string.Concat(new object[]
        {
                    "No SecureLoot/SecureDoor at ",
                    num,
                    ", ",
                    num2 - 1,
                    ", ",
                    num3,
                    " found."
        }));
        return;
      }
      SdtdConsole.Instance.Output(string.Concat(new object[]
      {
                "The SecureLoot/SecureDoor/Signs at ",
                num,
                ", ",
                num2 - 1,
                ", ",
                num3,
                " belongs to: ",
                owner,
      }));
    }
  }
}
