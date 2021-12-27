using System.Text;

namespace Botman.Source.BotModEvents
{
  internal class PlayerLogin
  {
    public static bool Handler(ClientInfo _clientInfo, string _compatibilityVersion, StringBuilder _stringBuilder)
    {
      if (!APICommon.GetOnlinePlayer(_clientInfo, out var persistentPlayer, out _)) { return true; }

      persistentPlayer.IsOnline = true;
      PersistentContainer.Instance.Save();
      return true;
    }
  }
}
