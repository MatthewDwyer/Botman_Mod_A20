using System.IO;

namespace Botman.Source.BotModEvents
{
  internal class GameAwake
  {
    public static void Handler()
    {
      if (!Directory.Exists(API.BotmanPath))
      {
        Directory.CreateDirectory(API.BotmanPath);
      }
    }
  }
}
