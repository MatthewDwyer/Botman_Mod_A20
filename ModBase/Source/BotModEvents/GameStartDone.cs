using System;
using Botman.Commands;

namespace Botman.Source.BotModEvents
{
  internal class GameStartDone
  {

    public static void Handler()
    {
      //todo: if world is null the delegate will never be assigned
      var world = GameManager.Instance.World;
      if (world != null)
      {
        GameManager.Instance.World.EntityLoadedDelegates += EntityLoadedDelegate.OnSpawn;
      }

      Config.Load();

      if (!Config.VersionMatches)
      {
        Config.ReloadConfigs();
      }

      //MainTimer.TimerStart();
      BMUptime.UpTime.Start();
      GameUpdate.IsAlive = true;

      try
      {
        Load();
      }
      catch (Exception e)
      {
        Log.Out("~Botman~ Error with GameStartDone: " + e);
      }

      //BMDropMiner.AddDelegateIfEnabled();
    }

    //todo: tidy up
    public static void Load()
    {
      try
      {
        Log.Out("~Botman~ Persistent Container loading.");
        PersistentContainer.Load();
        Log.Out("~Botman~ Loading Persistent Container at: " + PersistentContainer.Filepath);
      }
      catch (Exception arg)
      {
        Log.Out("~Botman~ Error Loading Persistent Container \n" +
                "Reason: \n" +
                arg);
      }

      BMClans.LoadClans();
      BMResetRegions.CheckForReset();
      BMResetAllPrefabs.CheckForReset();
      BMAreaReset.CheckForReset();
      try
      {
        BMHarmonyMain.ApplyPatches();
        Log.Out("~Botman~ Harmony Loaded");
      }
      catch (Exception e)
      {
        Log.Error("~Botman Notice~ Error loading harmony: " + e);
      }
    }
  }
}
