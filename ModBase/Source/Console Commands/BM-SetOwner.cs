using System;
using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMSetOwner : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Define an owner to SecureLoot/SecureDoor for a player.";

    public override string[] GetCommands() => new[] { "bm-setowner" };

    public override string GetHelp() =>
      "Define an owner for SecureLoot/SecureDoor for a player. After using this command you may need to leave and reenter the game to take effect.\n" +
      "1. Usage: \n" +
      "1. bm-setowner <x> <y> <z> <entity_id> \n" +
      "2. bm-setowner <x> <y> <z> \n" +
      "3. bm-setowner <entity_id> <entity_id_new_owner>\n" +
      "4. bm-setowner <entity_id>\n" +
      "5. bm-setowner \n" +
      "   1. x,y,z defines the location of SecureLoot/SecureDoor/Signs and the entity_id is the new owner \n" +
      "   2. x,y,z defines the location of SecureLoot/SecureDoor/Signs and you are the new owner \n" +
      "   3. the location of secure SecureLoot/SecureDoor/Signs if bellow entity_id and the entity_id_new_owner is the new owner\n" +
      "   4. the location of secure SecureLoot/SecureDoor/Signs if bellow your player and the entity_id is the new owner\n" +
      "   5. the location of secure SecureLoot/SecureDoor/Signs if bellow your player and you are the new owner\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 0 && _params.Count != 1 && _params.Count != 2 && _params.Count != 3 && _params.Count != 4)
      {
        SdtdConsole.Instance.Output($"Wrong number of arguments, expected 0 to 4, found {_params.Count}.");
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      var x = int.MinValue;
      var y = int.MinValue;
      var z = int.MinValue;
      ClientInfo clientInfo = null;

      switch (_params.Count)
      {
        case 3:
        case 4:
          int.TryParse(_params[0], out x);
          int.TryParse(_params[1], out y);
          int.TryParse(_params[2], out z);
          break;
        case 2:
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

            x = entityPlayer.GetBlockPosition().x;
            y = entityPlayer.GetBlockPosition().y;
            z = entityPlayer.GetBlockPosition().z;
            break;
          }

        case 0:
        case 1:
          {
            if (string.IsNullOrEmpty(_senderInfo.RemoteClientInfo?.CrossplatformId.CombinedString))
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

              x = entityPlayer2.GetBlockPosition().x;
              y = entityPlayer2.GetBlockPosition().y;
              z = entityPlayer2.GetBlockPosition().z;

            }
            catch (Exception arg)
            {
              Log.Out("Error in MakeOwner.Run: " + arg);
            }

            break;
          }
      }

      switch (_params.Count)
      {
        case 4:
        case 1:
          clientInfo = ConsoleHelper.ParseParamIdOrName(_params[0], true, false);
          break;
        case 3:
        case 0:
          clientInfo = _senderInfo.RemoteClientInfo;
          break;
        case 2:
          clientInfo = ConsoleHelper.ParseParamIdOrName(_params[1], true, false);
          break;
      }

      if (clientInfo == null)
      {
        SdtdConsole.Instance.Output("Playername or entity id not found.");

        return;
      }

      if (x == int.MinValue || y == int.MinValue || z == int.MinValue)
      {
        SdtdConsole.Instance.Output("At least one of the given coordinates is not a valid integer");

        return;
      }

      var found = false;
      var chunkClusters = GameManager.Instance.World.ChunkClusters;
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
                var position = tileEntitySign.ToWorldPos();
                if (position.x != x || position.z != z || position.y != y)
                {
                  continue;
                }

                found = true;
                tileEntitySign.SetOwner(clientInfo.CrossplatformId);
                break;
              }

              if (tileEntityType.ToString().Equals("SecureDoor"))
              {
                var tileEntitySecureDoor = (TileEntitySecureDoor)tileEntity;
                var position = tileEntitySecureDoor.ToWorldPos();
                if (position.x != x || position.z != z || position.y != y) { continue; }

                found = true;
                tileEntitySecureDoor.SetOwner(clientInfo.CrossplatformId);
                break;
              }

              if (tileEntityType.ToString().Equals("SecureLoot"))
              {
                var tileEntitySecureLootContainer = (TileEntitySecureLootContainer)tileEntity;
                var position = tileEntitySecureLootContainer.ToWorldPos();
                if (position.x != x || position.z != z || position.y != y) { continue; }

                found = true;
                tileEntitySecureLootContainer.SetOwner(clientInfo.CrossplatformId);
                break;
              }
            }
            catch (Exception ex)
            {
              Log.Out($"~Botman Notice~ Error in bm-setowner.execute: {ex}");
            }
          }
        }
      }

      if (!found)
      {
        SdtdConsole.Instance.Output($"Was not possible to find a SecureLoot/SecureDoor at {x}, {y - 1}, {z}");

        return;
      }

      SdtdConsole.Instance.Output($"The SecureLoot/SecureDoor/Signs at {x}, {y - 1}, {z} has the Owner transferred to {clientInfo.playerName}");
    }
  }
}
