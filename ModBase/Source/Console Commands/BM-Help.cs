using System.Collections.Generic;

namespace Botman.Commands
{
  class BMHelp : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Lists Console Commands for Botman";

    public override string[] GetCommands() => new[] { "bm-help" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-help \n" +
      "   1. Returns All Console Commands for Botman\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      SdtdConsole.Instance.Output(
          "bm-additemloot \n" +
          "bm-addresetarea \n" +
          "bm-anticheat \n" +
          "bm-buffs \n" +
          "bm-change \n" +
          "bm-chatcommands \n" +
          "bm-chatplayercolor\n" +
          "bm-checkloot\n" +
          "bm-chunkreset\n" +
          "bm-clan\n" +
          "bm-clearinventory\n" +
          "bm-dronefiledel \n" +
          "bm-exemptionlist \n" +
          "bm-fallingblocksremoval \n" +
          "bm-getowner \n" +
          "bm-getskills \n" +
          "bm-give \n" +
          "bm-giveat \n" +
          "bm-givexp \n" +
          "bm-givequest \n" +
          "bm-help \n" +
          "bm-lcbprefabrule \n" +
          "bm-levelachievement \n" +
          "bm-levelprefix \n" +
          "bm-listentities \n" +
          "bm-listplayerbed \n" +
          "bm-listplayerfriends \n" +
          "bm-milestones \n" +
          "bm-mutePlayer \n" +
          "bm-overridechatname \n" +
          "bm-pblock \n" +
          "bm-pdup \n" +
          "bm-playerchatmaxlength \n" +
          "bm-playergrounddistance\n" +
          "bm-playerunderground \n" +
          "bm-prender \n" +
          "bm-pundo \n" +
          "bm-readconfig \n" +
          "bm-remove (entity removal) \n" +
          "bm-removeitem \n" +
          "bm-removequest \n" +
          "bm-removeresetarea \n" +
          "bm-repblock \n" +
          "bm-resetallprefabs \n" +
          "bm-resetplayer \n" +
          "bm-resetprefab \n" +
          "bm-resetregions \n" +
          "bm-safe \n" +
          "bm-say \n" +
          "bm-sayprivate \n" +
          "bm-setchatprefix \n" +
          "bm-setcustommsg \n" +
          "bm-setowner \n" +
          "bm-setpundosize \n" +
          "bm-spawnhorde \n" +
          "bm-teleportplayerhome \n" +
          "bm-unlockall \n" +
          "bm-uptime \n" +
          "bm-webmaptraceprefabs \n" +
          "bm-webmaptraceresetareas \n" +
          "bm-webmaptraceresetregions \n" +
          "bm-webmaptracetraders \n" +
          "bm-zombieannouncer \n" +
          "bm-zones \n" +
          "bm-zombiefreetime \n" +
          "bm-vehiclefiledel \n"
          );
    }
  }
}
