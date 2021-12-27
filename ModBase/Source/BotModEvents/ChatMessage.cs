
using Botman.Commands;
using System.Collections.Generic;

namespace Botman.Source.BotModEvents
{
    internal class ChatMessage
  {
    public static bool Hide = false;
    public static string CommandPrefix = "/";
    public static string PublicTextColor = "ffffff";
    public static string PrivateTextColor = "EA3257";

    public static bool Handler(ClientInfo _clientInfo, EChatType _chatType, int _senderId, string _message, string _mainName, bool _localizeMain, List<int> _recipientEntityIds)
    {
      if (string.IsNullOrEmpty(_message) || _clientInfo == null) { return true; }

      if (_message.StartsWith(CommandPrefix))
      {
        if (Hide)
        {
          ReturnCommandPM(_clientInfo, _message);
        }
        var p = PersistentContainer.Instance.Players[_clientInfo.CrossplatformId, false];
        Log.Out("Player Command: '" + p.PlayerName + "' " + _message);

        //if (BMClans.Enabled)
        //{
        //  if (_message.ToLower().StartsWith($"{CommandPrefix}clan create "))
        //  {
        //    BMClans.AddClan(_clientInfo, _message.ToLower());
        //    return false;
        //  }
        //  if (_message.ToLower().StartsWith($"{CommandPrefix}clan color "))
        //  {
        //    BMClans.ChangeClanTagColorFromChat(_clientInfo, _message.ToLower());
        //    return false;
        //  }
        //  if (_message.ToLower().Equals($"{CommandPrefix}clan public"))
        //  {
        //    BMClans.ToggleClanPublic(_clientInfo);
        //    return false;
        //  }
        //  if (_message.ToLower().Equals($"{CommandPrefix}clan private"))
        //  {
        //    BMClans.ToggleClanPrivate(_clientInfo);
        //    return false;
        //  }
        //  if (_message.ToLower().Equals($"{CommandPrefix}clan list"))
        //  {
        //    BMClans.ListClans(_clientInfo);
        //    return false;
        //  }
        //  if (_message.ToLower().Equals($"{CommandPrefix}clan delete"))
        //  {
        //    BMClans.RemoveClan(_clientInfo);
        //    return false;
        //  }
        //  if (_message.ToLower().Equals($"{CommandPrefix}clan commands"))
        //  {
        //    BMClans.ListClanCommands(_clientInfo);
        //    return false;
        //  }
        //  if (_message.ToLower().StartsWith($"{CommandPrefix}clan promote "))
        //  {
        //    BMClans.PromoteMember(_clientInfo, _message.ToLower());
        //    return false;
        //  }
        //  if (_message.ToLower().StartsWith($"{CommandPrefix}clan demote "))
        //  {
        //    BMClans.DemoteMember(_clientInfo, _message.ToLower());
        //    return false;
        //  }
        //  if (_message.ToLower().StartsWith($"{CommandPrefix}clan invite "))
        //  {
        //    BMClans.InviteToClan(_clientInfo, _message.ToLower());
        //    return false;
        //  }
        //  if (_message.ToLower().StartsWith($"{CommandPrefix}clan resign to "))
        //  {
        //    BMClans.ResignClanToMember(_clientInfo, _message.ToLower());
        //    return false;
        //  }
        //  if (_message.ToLower().StartsWith($"{CommandPrefix}clan kick "))
        //  {
        //    BMClans.KickFromClan(_clientInfo, _message.ToLower());
        //    return false;
        //  }
        //  if (_message.ToLower().Equals($"{CommandPrefix}clan leave"))
        //  {
        //    BMClans.LeaveClan(_clientInfo);
        //    return false;
        //  }
        //  if (_message.ToLower().Equals($"{CommandPrefix}clan accept invite"))
        //  {
        //    BMClans.AcceptInvite(_clientInfo);
        //    return false;
        //  }
        //  if (_message.ToLower().Equals($"{CommandPrefix}clan decline invite"))
        //  {
        //    BMClans.DeclineInvite(_clientInfo);
        //    return false;
        //  }
        //  if (_message.ToLower().StartsWith($"{CommandPrefix}clan join "))
        //  {
        //    BMClans.InviteSelfToClan(_clientInfo, _message.ToLower());
        //    return false;
        //  }
        //  if (_message.ToLower().StartsWith($"{CommandPrefix}clan name "))
        //  {
        //    BMClans.ChangeClanName(_clientInfo, _message.ToLower());
        //    return false;
        //  }
        //  if (_message.ToLower().Equals($"{CommandPrefix}clan members"))
        //  {
        //    BMClans.ListClanMembers(_clientInfo);
        //    return false;
        //  }
        //}

        if (Hide)
        {
          return false;
        }

      }

      switch (_chatType)
      {
        case EChatType.Global:
          {
            var p = PersistentContainer.Instance.Players[_clientInfo.CrossplatformId, false];
            if (p != null)
            {
              EntityPlayer _player = GameManager.Instance.World.Players.dict[_clientInfo.entityId];
              if (_message.Length > p.MaxChatLength)
              {
                SendMessage.Private(_clientInfo, "Your _message was too long. So we blocked it!");
                return false;
              }
              if (p.IsChatMuted)
              {
                SendMessage.Private(_clientInfo, "Your chat is muted!");
                return false;
              }
              var clantag = "";
              var clantagcolor = "";
              var playername = "";
              var playerlvl = "";
              var namecolor = "";
              var coloredchat = false;
              var textcolor = "";
              var altered = false;
              playername = p.PlayerName;
              //if (BMClans.Enabled)
              //{
              //  if (p.ChatColor != null && p.ChatColor.ToLower() != "ffffff")
              //  {
              //    namecolor = p.ChatColor;
              //    coloredchat = true;
              //    altered = true;
              //  }
              //  if (!p.ChatName)
              //  {
              //    textcolor = p.ChatColor;
              //    altered = true;

              //  }
              //  if (p.ClanName != null)
              //  {
              //    altered = true;
              //    clantag = p.ClanName;
              //    if (p.ClanTagColor != null)
              //    {
              //      clantagcolor = "[" + p.ClanTagColor + "]";
              //    }
              //  }
              //}
              if (p.ChatColor != null && p.ChatColor.ToLower() != "ffffff")
              {
                namecolor = p.ChatColor;
                altered = true;
                if (p.ChatName)
                {
                  coloredchat = true;
                }
              }
              if (LevelSystem.ShowLevelEnabled)
              {
                playerlvl = LevelSystem.PlayerLevel(_player);
                altered = true;
              }
              if (altered == false)
              {
                return true;
              }
              SendPlayersMessage(_clientInfo.entityId, textcolor + _message + "[-]", playerlvl + "[-]" + clantagcolor + clantag + "[-] " + namecolor + playername + "[-]");
              if (coloredchat)
              {
                Log.Out("Chatting colored: '" + p.PlayerName + "' " + _message);
              }
              return false;
            }

            return true;
          }

        case EChatType.Friends:
          {
            var p = PersistentContainer.Instance.Players[_clientInfo.CrossplatformId, false];
            if (p != null)
            {
              Log.Out("Friends Chat: '" + p.PlayerName + "' " + _message);
            }

            break;
          }

        case EChatType.Party:
          {
            var p = PersistentContainer.Instance.Players[_clientInfo.CrossplatformId, false];
            if (p != null)
            {
              Log.Out("Party Chat: '" + p.PlayerName + "' " + _message);
            }

            break;
          }

        case EChatType.Whisper:
          break;

        default:
          break;
      }

      return true;
    }

    private static void SendPlayersMessage(int senderEntityId, string message, string senderName)
    {
      foreach (var clientInfo in SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.List)
      {
        clientInfo?.SendPackage(NetPackageManager.GetPackage<NetPackageChat>()
          .Setup(EChatType.Global, senderEntityId, message, senderName, _localizeMain: false, _recipientEntityIds: null));
      }
    }

    private static void ReturnCommandPM(ClientInfo clientInfo, string message)
    {
      //todo: not used?
      var botName = $"(PM) {Config.BotName}[FFFFFF]";

      var persistentPlayer = PersistentContainer.Instance.Players[clientInfo.CrossplatformId, false];

      clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>()
        .Setup(EChatType.Whisper, _senderEntityId: -1, $"[{PrivateTextColor}]{message}[-]", persistentPlayer?.PlayerName + "[-]", _localizeMain: false, _recipientEntityIds: null));
    }
  }
}
