namespace Botman
{
  class GameMessage
  {
    public static bool Enabled = false;

    public static string LogInMessage = "[name] has joined the game.";
    public static string LogInNameColor = "[00FF00]";
    public static string LoginMessageColor = "[FFFFFF]";

    public static string LogOutMessage = "[name] has logged out.";
    public static string LogOutNameColor = "[00FF00]";
    public static string LogOutMessageColor = "[FFFFFF]";

    public static string PlayerKilled = "[killer] has killed [victim].";
    public static string KillerColor = "[FF0000]";
    public static string VictimColor = "[0000FF]";
    public static string PlayerKilledMessageColor = "[FFFFFF]";

    public static string PlayerDied = "[name] has died.";
    public static string PlayerDiedNameColor = "[00FF00]";
    public static string PlayerDiedMessageColor = "[FFFFFF]";

    public static bool Handler(ClientInfo _clientInfo, EnumGameMessages _messageType, string _message, string _targetName, bool _localizeTargetPlayerName, string _killerName, bool _localizeKillerName)
    {
      if (!Enabled) { return true; }
      switch (_messageType)
      {
        case EnumGameMessages.JoinedGame:
          HandlePlayerJoined(_targetName);
          break;

        case EnumGameMessages.LeftGame:
          HandlePlayerLogout(_targetName);
          break;

        case EnumGameMessages.EntityWasKilled:
          if (!string.IsNullOrEmpty(_killerName) && !string.IsNullOrEmpty(_targetName))
          {
            HandlePlayerKills(_killerName, _targetName);
          }
          else if (!string.IsNullOrEmpty(_targetName))
          {
            HandlePlayerDied(_targetName);
          }

          break;

        case EnumGameMessages.PlainTextLocal:
          break;

        case EnumGameMessages.ChangedTeam:
          break;

        case EnumGameMessages.Chat:
          break;

        default:
          break;
      }

      return false;
    }

    public static void HandlePlayerJoined(string playerName)
    {
      var message = LogInMessage;
      
      if (message.ToLower().Contains("[name]"))
      {
        message = message.Replace("[name]", $"{LogInNameColor}{playerName}[-]");
      }

      SendMessage.Public($"{LoginMessageColor}{message}[-]");
    }

    public static void HandlePlayerLogout(string playerName)
    {
      var message = LogOutMessage;
      if (message.Contains("[name]"))
      {
        message = message.Replace("[name]", $"{LogOutNameColor}{playerName}[-]");
      }

      SendMessage.Public($"{LogOutMessageColor}{message}[-]");
    }

    public static void HandlePlayerDied(string playerName)
    {
      var message = PlayerDied;
      if (message.Contains("[name]"))
      {
        message = message.Replace("[name]", $"{PlayerDiedNameColor}{playerName}[-]");
      }

      SendMessage.Public(PlayerDiedMessageColor + message + "[-]");
    }

    public static void HandlePlayerKills(string killerName, string victimName)
    {
      var message = PlayerKilled;
      if (message.Contains("[victim]"))
      {
        message = message.Replace("[victim]", $"{VictimColor}{victimName}[-]");
      }

      if (message.Contains("[killer]"))
      {
        message = message.Replace("[killer]", $"{KillerColor}{killerName}[-]");
      }
      
      SendMessage.Public($"{PlayerKilledMessageColor}{message}[-]");
    }
  }
}
