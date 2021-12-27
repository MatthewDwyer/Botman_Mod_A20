using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMZombieFreeTime : BMCmdAbstract
  {
    public static bool Enabled = false;
    public static int Start = 6;
    public static int End = 20;
    public static Queue<int> DespawnQueue = new Queue<int>();

    public override string GetDescription() => "~Botman~ If Enabled, removes zombies during designated hours.";

    public override string[] GetCommands() => new[] { "bm-zombiefreetime" };

    public override string GetHelp() =>
      "If Enabled, removes zombies during designated hours.\n" +
      "Usage:\n" +
      "1. bm-zombiefreetime [enable/disable] \n" +
      "2. bm-zombiefreetime set [startTime] [endTime] \n" +
      "  1. Enables/Disables zombie remover. \n" +
      "  2. Sets window for when zombies will be removed.\n" +
      "     ex: bm-zombiefreetime set 7 22\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      switch (_params[0].ToLower())
      {
        case "enable":
          {
            Enabled = true;
            SdtdConsole.Instance.Output($"~Botman~ Zombie Free Time ENABLED. Zombies Will be allowed between {End} and {Start}.");
            return;
          }

        case "disable":
          {
            Enabled = false;
            GameStats.Set(EnumGameStats.IsSpawnEnemies, true);
            SdtdConsole.Instance.Output("~Botman~ Zombie Free Time DISABLED. Zombies will no respawn as usual.");
            return;
          }

        case "set":
          {
            if (_params.Count != 3)
            {
              SdtdConsole.Instance.Output(GetHelp());
              SdtdConsole.Instance.Output("~Botman~ Invalid parameter count.");

              return;
            }

            if (!int.TryParse(_params[1], out Start))
            {
              SdtdConsole.Instance.Output("~Botman~ start time must be an integer.");

              return;
            }
            if (Start < 0 || Start > 23)
            {
              SdtdConsole.Instance.Output($"~Botman~ invalid start time: {Start}. Start time must be an integer between 0 and 23.");

              return;
            }

            if (!int.TryParse(_params[2], out End))
            {
              SdtdConsole.Instance.Output($"~Botman~ start time must be an integer.");

              return;
            }

            if (Start < 0 || Start > 23)
            {
              SdtdConsole.Instance.Output($"~Botman~ invalid start time: {Start}. Start time must be an integer between 0 and 23.");

              return;
            }

            Config.UpdateXml();
            SdtdConsole.Instance.Output($"~Botman~ Zombie freetime has been updated. Start time: {Start}  End time: {End}. Tool is currently {(Enabled ? "ENABLED" : "DISABLED")}");

            return;
          }
      }

      SdtdConsole.Instance.Output(GetHelp());
    }

    public static void Update()
    {
      if (!Enabled) { return; }

      if (NoZombieTime())
      {
        if (!GameStats.GetBool(EnumGameStats.IsSpawnEnemies)) { return; }

        Log.Warning("~Botman~ Zombie Free time!");
        GameStats.Set(EnumGameStats.IsSpawnEnemies, false);
        ClearRemainingZombies();
        Log.Warning("~Botman~ All Zombies have been removed.");
      }
      else
      {
        if (GameStats.GetBool(EnumGameStats.IsSpawnEnemies)) { return; }

        Log.Warning("~Botman~ Zombies Free time has ended!~");
        GameStats.Set(EnumGameStats.IsSpawnEnemies, true);
      }
    }

    public static bool NoZombieTime()
    {
      if (!Enabled) { return false; }

      var worldHours = (int)(GameManager.Instance.World.worldTime / 1000UL) % 24;

      return Start < End && worldHours >= Start && worldHours < End ||
             Start > End && (worldHours >= Start || worldHours < End);
    }

    public static void ClearRemainingZombies()
    {
      foreach (var entity in GameManager.Instance.World.Entities.dict)
      {
        if (!(entity.Value is EntityZombie)){continue;}

        DespawnQueue.Enqueue(entity.Key);
      }
    }
  }
}
