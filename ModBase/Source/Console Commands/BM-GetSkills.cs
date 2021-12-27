using System.Collections.Generic;

namespace Botman.Commands
{
  class BMGetSkills : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Returns a players skills";

    public override string[] GetCommands() => new[] { "bm-getskills", "getskills" };

    public override string GetHelp() =>
      "Returns a players skills and levels \n" +
      "Usage: \n" +
      "1. bm-getskills <playername/steamid>\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 1)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (!CmdHelpers.GetEntityPlayer(_params[0], out var entityPlayer)) { return; }

      foreach (var progressionValue in entityPlayer.Progression.ProgressionValues.Dict)
      {
        SdtdConsole.Instance.Output($"Name: {progressionValue.Value.Name} Level: {progressionValue.Value.Level} Next Level Locked: {progressionValue.Value.IsLocked(entityPlayer)}");
      }

      SdtdConsole.Instance.Output($"Points to spend: {entityPlayer.Progression.SkillPoints}");
    }
  }
}
