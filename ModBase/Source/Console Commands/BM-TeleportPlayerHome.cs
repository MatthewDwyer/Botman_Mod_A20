using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMTeleportPlayerHome : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Teleport a player to his home (on bedroll)";

    public override string[] GetCommands() => new[] { "bm-teleportplayerhome" };

    public override string GetHelp() =>
      "Usage:\n " +
      "1. bm-teleportplayerhome <steam id / player name / entity id>\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 1)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (!CmdHelpers.GetClientAndEntity(_params[0], out var clientInfo, out var entityPlayer) || !entityPlayer.IsSpawned()) { return; }

      var spawnPoints = entityPlayer.SpawnPoints;

      if (spawnPoints.Count == 0)
      {
        SdtdConsole.Instance.Output("The player does not have a defined HOME bed!");

        return;
      }

      entityPlayer.position.x = spawnPoints[0].x;
      entityPlayer.position.y = spawnPoints[0].y + 1f;
      entityPlayer.position.z = spawnPoints[0].z;

      clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityTeleport>().Setup(entityPlayer));

      SdtdConsole.Instance.Output($"Player teleported to his home at {entityPlayer.GetBlockPosition()}");
    }
  }
}
