using Botman.Commands;
using System;

namespace Botman.Source.BotModEvents
{
  internal class GameShutdown
  {
    public static void Handler()
    {
      try
      {
        Log.Out("~Botman~ Server Shutting Down, Saving Persistent Container!");
        PersistentContainer.Instance.Save();

        GameUpdate.IsAlive = false;
        BMUptime.UpTime.Stop();
      }
      catch (Exception arg)
      {
        Log.Error("~Botman~ Error in StateManager.Shutdown: " + arg);
      }
    }
  }
}
