using System;
using System.Collections.Generic;
using Botman.Source.BotModEvents;

namespace Botman.Commands
{
  public class BMClans : BMCmdAbstract
  {
    public static bool Enabled = false;
    public static int MaxClansAllowed = 10;
    public static int PlayerLimit = 5;
    public static int MaxCharacters = 8;
    public static int LevelToCreate = 25;
    public static Dictionary<string, List<string>> Clans = new Dictionary<string, List<string>>();
    public static Dictionary<string, bool> ClanIsInviteOnly = new Dictionary<string, bool>();
    public static string White = "[ffffff]";
    public static string Green = "[00ff00]";

    public override string GetDescription() => "~Botman~ Clans";

    public override string[] GetCommands() => new[] { "bm-clan" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-clan list \n" +
      "2. bm-clan add clan [clanname] [leadername] \n" +
      "3. bm-clan add member [clanname] [playername] \n" +
      "4. bm-clan remove clan [clanname] \n" +
      "5. bm-clan remove member [playername] \n" +
      "6. bm-clan replace [leadername] [newleadername] \n" +
      "7. bm-clan max clans [number] \n" +
      "8. bm-clan max player [number] \n" +
      "9. bm-clan min_level [number] \n" +
      "10. bm-clan enable \n" +
      "11. bm-clan disable \n" +
      "12. bm-clan toggle [clanname] \n" +
      "13. bm-clan promote [clanname] \n" +
      "14. bm-clan demote [clanname] \n" +
      "15. bm-clan color [clanname] [colorcode] \n" +
      "   1. returns list of clans and members \n" +
      "   2. adds a new clan and places an admin \n" +
      "   3. adds a member to an existing clan \n" +
      "   4. removes a clan and all of its members \n" +
      "   5. removes a player from any clan they are in \n" +
      "   6. replaces leadership (must be in same clan) \n" +
      "   7. sets maximum amount of allowable clans \n" +
      "   8. sets maxmimum amount of allowable players in a clan \n" +
      "   9. sets minimum level a player must be to create a clan \n" +
      "   10. enables the clan system \n" +
      "   11. disables the clan system \n" +
      "   12. toggles the clan to invite only or public for all to join. \n" +
      "   13. promotes a member to officer in their clan. \n" +
      "   14. demotes an officer to member. \n" +
      "   15. change clan tag color of clan. \n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      var json = false;
      if (_params.Count > 0 && _params.Contains("/json"))
      {
        json = true;
        _params.Remove("/json");
      }

      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      switch (_params[0].ToLower())
      {
        case "list":
          {
            if (json)
            {
              ClansJSON.SendClanInfo();

              return;
            }

            if (Clans.Count == 0)
            {
              SdtdConsole.Instance.Output("There are currently no clans.");
              SdtdConsole.Instance.Output("{ \"clancount\" : \"0\" }");
              return;
            }

            SdtdConsole.Instance.Output($"{{ \"clancount\" : \"{ClanIsInviteOnly.Count}\" }}");

            var x = 1;
            foreach (var clan in ClanIsInviteOnly)
            {
              var clanTagColor = PersistentContainer.Instance.Players[Clans[clan.Key].RandomObject(), true].ClanTagColor;
              SdtdConsole.Instance.Output($" {x}. Clan: {clan.Key} Invite Only: {clan.Value} Clan Tag Color: {clanTagColor}");

              var y = 1;
              foreach (var member in Clans[clan.Key])
              {
                var player = PersistentContainer.Instance.Players[member, true];
                SdtdConsole.Instance.Output($"    {y}. {player.PlayerName} Leader: {player.ClanLeader} Officer: {player.ClanPromoter}");
                y++;
              }

              x++;
            }
            return;
          }

        case "enable" when _params.Count > 1:
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case "enable" when Enabled:
          {
            if (json)
            {
              ClansJSON.Declined("already enabled");

              return;
            }

            SdtdConsole.Instance.Output("Clan system is already enabled");

            return;
          }

        case "enable":
          {
            if (json)
            {
              ClansJSON.Accepted();
            }
            else
            {
              SdtdConsole.Instance.Output("Clan system has been enabled");
            }

            Enabled = true;
            Config.UpdateXml();

            return;
          }

        case "disable" when _params.Count > 1:
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case "disable" when !Enabled:
          {
            if (json)
            {
              ClansJSON.Declined("already disabled");

              return;
            }

            SdtdConsole.Instance.Output("Clan system is already disabled");

            return;
          }

        case "disable":
          {
            if (json)
            {
              ClansJSON.Accepted();
            }
            else
            {
              SdtdConsole.Instance.Output("Clan system has been disabled");
            }

            Enabled = false;
            Config.UpdateXml();

            return;
          }

        case "min_level" when _params.Count != 2:
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case "min_level":
          {
            if (!int.TryParse(_params[1], out var level))
            {
              if (json)
              {
                ClansJSON.Declined("Level must be an integer.");

                return;
              }

              SdtdConsole.Instance.Output("Level must be an integer.");

              return;
            }

            if (json)
            {
              ClansJSON.Accepted();
            }
            else
            {
              SdtdConsole.Instance.Output($"Level to create a clan has been set to {LevelToCreate}");
            }

            LevelToCreate = level;
            Config.UpdateXml();

            return;
          }

        case "max" when _params.Count > 3:
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case "max" when _params[1].Equals("players", StringComparison.InvariantCultureIgnoreCase) && _params.Count == 3:
          {
            if (!int.TryParse(_params[2], out var count))
            {
              if (json)
              {
                ClansJSON.Declined("Number must be an integer.");

                return;
              }

              SdtdConsole.Instance.Output("Number must be an integer.");

              return;
            }

            if (json)
            {
              ClansJSON.Accepted();
            }
            else
            {
              SdtdConsole.Instance.Output($"Max players per clan has been set to {count}");
            }

            PlayerLimit = count;
            Config.UpdateXml();

            return;
          }

        case "max" when _params[1].Equals("clans", StringComparison.InvariantCultureIgnoreCase) && _params.Count == 3:
          {
            if (!int.TryParse(_params[2], out var count))
            {
              if (json)
              {
                ClansJSON.Declined("number must be an integer");

                return;
              }

              SdtdConsole.Instance.Output("Number must be an integer.");

              return;
            }

            if (json)
            {
              ClansJSON.Accepted();
            }
            else
            {
              SdtdConsole.Instance.Output($"Max clans has been set to {count}");
            }

            MaxClansAllowed = count;
            Config.UpdateXml();

            return;
          }

        case "max" when json:
          ClansJSON.Declined("parameters not recognized");
          return;

        case "max":
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case "add" when _params[1].Equals("clan", StringComparison.InvariantCultureIgnoreCase) && _params.Count == 4:
          {
            var clanName = _params[2];
            if (NameIsTaken(clanName))
            {
              if (json)
              {
                ClansJSON.Declined("clanname taken");

                return;
              }

              SdtdConsole.Instance.Output("Clanname is already taken. Please try again.");

              return;
            }

            var player = PersistentContainer.Instance.Players[PersistentContainer.Instance.Players.GetId(_params[3]), true];
            if (player == null)
            {
              if (json)
              {
                ClansJSON.Declined("could not find player");

                return;
              }

              SdtdConsole.Instance.Output("Could not find player");

              return;
            }

            if (player.ClanName != null)
            {
              if (json)
              {
                ClansJSON.Declined("player already in a clan");

                return;
              }

              SdtdConsole.Instance.Output($"{player.PlayerName} is already in a clan. Please remove them before trying again.");

              return;
            }

            player.ClanName = clanName;
            player.ClanPromoter = true;
            player.ClanIsInviteOnly = true;
            player.ClanInvite = null;
            player.ClanLeader = true;
            PersistentContainer.Instance.Save();

            if (json)
            {
              ClansJSON.Accepted();
            }
            else
            {
              SdtdConsole.Instance.Output($"Created clan {clanName} and {player.PlayerName} as the leader");
            }

            var member = new List<string> { player.ID };
            Clans.Add(clanName, member);
            ClanIsInviteOnly.Add(clanName, true);

            return;
          }

        case "add" when _params[1].Equals("member", StringComparison.InvariantCultureIgnoreCase) && _params.Count == 4:
          {
            if (!Clans.ContainsKey(_params[2]))
            {
              if (json)
              {
                ClansJSON.Declined("clan not found");

                return;
              }

              SdtdConsole.Instance.Output("Could not find clan.");

              return;
            }

            var ID = PersistentContainer.Instance.Players.GetId(_params[3]);
            if (ID == null)
            {
              if (json)
              {
                ClansJSON.Declined("player not found");
                return;
              }
              SdtdConsole.Instance.Output("Could not find member");
              return;
            }

            var player = PersistentContainer.Instance.Players[ID, true];
            if (player.ClanName != null)
            {
              if (json)
              {
                ClansJSON.Declined("player already in a clan");

                return;
              }

              SdtdConsole.Instance.Output($"{player.PlayerName} is already in a clan.");

              return;
            }

            if (!Clans.TryGetValue(_params[2], out var members))
            {
              ClansJSON.Declined($"clan not found {_params[2]}");

              return;
            }

            var random = members.RandomObject();
            var player2 = PersistentContainer.Instance.Players[random, true];
            player.ClanName = player2.ClanName;
            player.ClanLeader = false;
            player.ClanInvite = null;
            player.ClanPromoter = false;
            player.ClanTagColor = player2.ClanTagColor;
            player.ClanIsInviteOnly = player2.ClanIsInviteOnly;

            PersistentContainer.Instance.Save();
            Clans[player2.ClanName].Add(ID);

            if (json)
            {
              ClansJSON.Accepted();

              return;
            }

            SdtdConsole.Instance.Output($"{player.PlayerName} has been added to {player.ClanName}");

            return;
          }

        case "add" when json:
          ClansJSON.Declined("parameters not recognized");
          return;

        case "add":
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case "remove" when _params[1].Equals("clan", StringComparison.InvariantCultureIgnoreCase) && _params.Count == 3:
          {
            if (!Clans.ContainsKey(_params[2]))
            {
              if (json)
              {
                ClansJSON.Declined("clan not found");

                return;
              }

              SdtdConsole.Instance.Output("Clan does not exist");

              return;
            }

            var members = Clans[_params[2]];
            foreach (var member in members)
            {
              var player = PersistentContainer.Instance.Players[member, true];
              player.ClanName = null;
              player.ClanLeader = false;
              player.ClanPromoter = false;
              player.ClanTagColor = null;
              player.ClanInvite = null;

              PersistentContainer.Instance.Save();

              if (json)
              {
                ClansJSON.Accepted();
              }
              else
              {
                SdtdConsole.Instance.Output("Clan has been deleted.");
              }

              var cInfo = ConsoleHelper.ParseParamIdOrName(member, true, true);
              if (cInfo != null)
              {
                SendMessage.Private(cInfo, "Your clan has been deleted.");
              }
            }

            Clans.Remove(_params[2]);
            ClanIsInviteOnly.Remove(_params[2]);

            return;
          }

        case "remove" when _params[1].ToLower().Equals("member"):
          {
            var ID = PersistentContainer.Instance.Players.GetId(_params[2]);
            if (ID == null)
            {
              if (json)
              {
                ClansJSON.Declined("player not found");

                return;
              }

              SdtdConsole.Instance.Output("Could not find member");

              return;
            }

            var player = PersistentContainer.Instance.Players[ID, true];
            if (player.ClanName == null)
            {
              if (json)
              {
                ClansJSON.Declined("player not in a clan");

                return;
              }

              SdtdConsole.Instance.Output($"{player.PlayerName} is not in a clan.");

              return;
            }

            if (player.ClanLeader)
            {
              if (json)
              {
                ClansJSON.Declined("player already an admin");

                return;
              }

              SdtdConsole.Instance.Output($"{player.PlayerName} is an admin. Please replace them first using \"bm-clan replace [leadername] [newleadername]\"");

              return;
            }

            var members = Clans[player.ClanName];
            members.Remove(player.ID);
            player.ClanLeader = false;
            player.ClanPromoter = false;
            player.ClanTagColor = null;
            player.ClanInvite = null;

            if (json)
            {
              ClansJSON.Accepted();
            }
            else
            {
              SdtdConsole.Instance.Output($"{player.PlayerName} has been removed from {player.ClanName}");
            }

            var cInfo = ConsoleHelper.ParseParamIdOrName(player.ID);
            if (cInfo != null)
            {
              SendMessage.Private(cInfo, $"You have been removed from {player.ClanName}");
            }

            player.ClanName = null;
            PersistentContainer.Instance.Save();

            return;
          }

        case "remove" when json:
          ClansJSON.Declined("parameters not recognized");
          return;

        case "remove":
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case "replace" when _params.Count != 3:
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case "replace":
          {
            var oldId = PersistentContainer.Instance.Players.GetId(_params[1]);
            if (oldId == null)
            {
              if (json)
              {
                ClansJSON.Declined($"{_params[1]} not found");

                return;
              }

              SdtdConsole.Instance.Output($"Could not find player: {_params[1]}");

              return;
            }

            var newId = PersistentContainer.Instance.Players.GetId(_params[2]);
            if (newId == null)
            {
              if (json)
              {
                ClansJSON.Declined($"{_params[2]} not found");

                return;
              }

              SdtdConsole.Instance.Output($"Could not find player: {_params[2]}");

              return;
            }

            var oldPlayer = PersistentContainer.Instance.Players[oldId, true];
            if (oldPlayer.ClanName == null)
            {
              if (json)
              {
                ClansJSON.Declined("Players must be in the same clan to replace leadership.");

                return;
              }

              SdtdConsole.Instance.Output("Players must be in the same clan to replace leadership.");

              return;
            }

            var newPlayer = PersistentContainer.Instance.Players[newId, true];
            if (oldPlayer.ClanName != newPlayer.ClanName)
            {
              if (json)
              {
                ClansJSON.Declined("Players must be in the same clan to replace leadership.");

                return;
              }

              SdtdConsole.Instance.Output("Players must be in the same clan to replace leadership.");

              return;
            }

            if (!oldPlayer.ClanLeader)
            {
              if (json)
              {
                ClansJSON.Declined($"{oldPlayer.PlayerName} is not the leader of their clan");

                return;
              }

              SdtdConsole.Instance.Output($"{oldPlayer.PlayerName} is not the leader of their clan");

              return;
            }

            if (newPlayer.ClanLeader)
            {
              if (json)
              {
                ClansJSON.Declined($"{newPlayer.PlayerName} is already the leader of their clan");

                return;
              }

              SdtdConsole.Instance.Output($"{newPlayer.PlayerName} is already the leader of their clan");

              return;
            }

            oldPlayer.ClanLeader = false;
            oldPlayer.ClanPromoter = false;
            newPlayer.ClanLeader = true;
            newPlayer.ClanPromoter = true;

            if (json)
            {
              ClansJSON.Accepted();
            }
            else
            {
              SdtdConsole.Instance.Output($"{newPlayer.PlayerName} is now the leader of their clan.");
            }

            PersistentContainer.Instance.Save();

            return;
          }

        case "toggle" when _params.Count != 1:
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case "toggle":
          {
            var clanName = _params[1];
            if (!Clans.ContainsKey(clanName))
            {
              if (json)
              {
                ClansJSON.Declined("clan does not exist");

                return;
              }

              SdtdConsole.Instance.Output("Clan does not exist.");

              return;
            }

            var openState = "";
            if (ClanIsInviteOnly[clanName])
            {
              foreach (var steamId in Clans[clanName])
              {
                var player = PersistentContainer.Instance.Players[steamId, true];
                if (player.ClanName != clanName) { continue; }

                player.ClanIsInviteOnly = false;

                openState = "Open to the public";
              }

              ClanIsInviteOnly[clanName] = false;
            }
            else
            {
              foreach (var steamId in Clans[clanName])
              {
                var player = PersistentContainer.Instance.Players[steamId, true];

                if (player.ClanName != clanName) { continue; }

                player.ClanIsInviteOnly = true;
                openState = "Invite Only";
              }

              ClanIsInviteOnly[clanName] = true;
            }

            if (json)
            {
              ClansJSON.Accepted();

              return;
            }

            SdtdConsole.Instance.Output($"Clan has been set to {openState}");
            PersistentContainer.Instance.Save();

            return;
          }

        case "promote" when _params.Count != 2:
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case "promote":
          {
            var ID = PersistentContainer.Instance.Players.GetId(_params[1]);
            if (ID == null)
            {
              SdtdConsole.Instance.Output($"Could not locate {_params[1]}");

              return;
            }

            var player = PersistentContainer.Instance.Players[ID, true];
            if (player.ClanName == null)
            {
              SdtdConsole.Instance.Output("Player is not in a clan.");

              return;
            }

            if (!player.ClanPromoter)
            {
              player.ClanPromoter = true;
              PersistentContainer.Instance.Save();
              SdtdConsole.Instance.Output($"{player.PlayerName} promoted to officer.");

              return;
            }

            SdtdConsole.Instance.Output($"{player.PlayerName} already an officer.");

            return;
          }

        case "demote" when _params.Count != 2:
          SdtdConsole.Instance.Output(GetHelp());
          return;

        case "demote":
          {
            var ID = PersistentContainer.Instance.Players.GetId(_params[1]);
            if (ID == null)
            {
              SdtdConsole.Instance.Output("Could not locate " + _params[1]);

              return;
            }

            var player = PersistentContainer.Instance.Players[ID, true];
            if (player.ClanName == null)
            {
              SdtdConsole.Instance.Output("Player is not in a clan.");

              return;
            }

            if (player.ClanLeader)
            {
              SdtdConsole.Instance.Output("Cannot demote clan leaders. Instead use \"bm-clan replace\"");

              return;
            }

            if (player.ClanPromoter)
            {
              player.ClanPromoter = false;
              PersistentContainer.Instance.Save();
              SdtdConsole.Instance.Output($"{player.PlayerName} demoted to member.");

              return;
            }

            SdtdConsole.Instance.Output($"{player.PlayerName} already holds no position.");

            return;
          }

        case "color" when _params.Count != 3:
            SdtdConsole.Instance.Output(GetHelp());
            return;

         case "color":
          {
            var clanName = _params[1];
            if (!Clans.ContainsKey(clanName)) { return; }

            ChangeClanTagColor(clanName, _params[2]);
            SdtdConsole.Instance.Output("Tag color changed to " + _params[2]);
          }
          return;

        default:
          {
            if (json)
            {
              ClansJSON.Declined("parameters not recognized.");

              return;
            }

            SdtdConsole.Instance.Output(GetHelp());

            return;
          }
      }
    }

    public static void ListClanMembers(ClientInfo cInfo)
    {
      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];

      if (player.ClanName == null) { return; }

      SendMessage.Private(cInfo, $"Members of {player.ClanName}");
      var x = 1;
      foreach (var ID in PersistentContainer.Instance.Players.IDs)
      {
        var member = PersistentContainer.Instance.Players[ID, true];

        if (player.ClanName != member.ClanName) { continue; }

        SendMessage.Private(cInfo, $"  {x}. {member.PlayerName}");
        x++;
      }
    }

    public static void AddClan(ClientInfo cInfo, string clanName)
    {
      if (cInfo == null) { return; }

      if (Clans.Count >= MaxClansAllowed)
      {
        SendMessage.Private(cInfo, $"The admin has set max clans to {MaxClansAllowed}.");
        SendMessage.Private(cInfo, "You must join an already existing clan");

        return;
      }

      clanName = clanName.Replace($"{ChatMessage.CommandPrefix}clan create ", "").Trim();

      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];
      if (player == null) { return; }

      var entityPlayer = GameManager.Instance.World.Players.dict[cInfo.entityId];
      if (entityPlayer == null) { return; }

      if (XUiM_Player.GetLevel(entityPlayer) < LevelToCreate)
      {
        SendMessage.Private(cInfo, $"You must be at least level {LevelToCreate} to create a clan.");

        return;
      }

      if (player.ClanName != null)
      {
        SendMessage.Private(cInfo, "You are already in a clan. Please leave it before creating your own.");

        return;
      }

      if (clanName.Length > MaxCharacters || clanName.Length == 0)
      {
        SendMessage.Private(cInfo, $"Clan names must be between 1 and {MaxCharacters} characters.");

        return;
      }

      if (NameIsTaken(clanName))
      {
        SendMessage.Private(cInfo, "Name matches an already existing clan. Try again.");

        return;
      }

      player.ClanName = clanName;
      player.ClanLeader = true;
      player.ClanPromoter = true;
      player.ClanIsInviteOnly = true;
      player.ClanTagColor = null;

      var clanMembers = new List<string> { cInfo.CrossplatformId.CombinedString };

      Clans.Add(clanName, clanMembers);
      ClanIsInviteOnly.Add(clanName, true);
      PersistentContainer.Instance.Save();
      SendMessage.Private(cInfo, $"You have created the clan [u][FFFFFF]{clanName}[/u][-]");
      SendMessage.Private(cInfo, "Type \"/clan commands\" to view your options.");
    }

    public static void ChangeClanName(ClientInfo cInfo, string newName)
    {
      newName = newName.Replace($"{ChatMessage.CommandPrefix}clan name ", "").Trim();

      if (newName.Length > MaxCharacters || newName.Length == 0)
      {
        SendMessage.Private(cInfo, $"Clan names must be between 1 and {MaxCharacters} characters.");

        return;
      }

      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];
      var oldName = "";

      if (player.ClanLeader)
      {
        oldName = player.ClanName;
        var members = Clans[player.ClanName];

        foreach (var ID in members)
        {
          var member = PersistentContainer.Instance.Players[ID, true];
          member.ClanName = newName;
          var memberClientInfo = ConsoleHelper.ParseParamIdOrName(ID, true, false);

          if (memberClientInfo != null)
          {
            SendMessage.Private(memberClientInfo, $"Your clan name has been changed to [u][FFFFFF]{newName}[/u][-][-]");
          }
        }

        Clans.Remove(oldName);
        ClanIsInviteOnly.Remove(oldName);
        Clans.Add(newName, members);
        ClanIsInviteOnly.Add(newName, player.ClanIsInviteOnly);
        PersistentContainer.Instance.Save();
      }

      if (oldName == "") { return; }

      foreach (var ID in PersistentContainer.Instance.Players.IDs)
      {
        var player2 = PersistentContainer.Instance.Players[ID, true];

        if (player2.ClanInvite == oldName)
        {
          player2.ClanInvite = newName;
        }
      }

      PersistentContainer.Instance.Save();
    }

    public static void ChangeClanTagColor(string clanName, string color)
    {
      foreach (var ID in PersistentContainer.Instance.Players.IDs)
      {
        var player = PersistentContainer.Instance.Players[ID, true];

        if (player.ClanName == clanName)
        {
          player.ClanTagColor = color;
        }
      }

      PersistentContainer.Instance.Save();
    }

    public static void ChangeClanTagColorFromChat(ClientInfo cInfo, string color)
    {
      color = color.Replace($"{ChatMessage.CommandPrefix}clan color ", "").Trim();

      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];
      if (!player.ClanLeader) { return; }

      foreach (var ID in PersistentContainer.Instance.Players.IDs)
      {
        var member = PersistentContainer.Instance.Players[ID, true];

        if (member.ClanName == player.ClanName)
        {
          member.ClanTagColor = color;
        }
      }

      PersistentContainer.Instance.Save();
    }

    public static void ToggleClanOpen(ClientInfo cInfo, string toggle)
    {
      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];
      if (!player.ClanLeader)
      {
        SendMessage.Private(cInfo, "You do not have permission to use this command.");

        return;
      }

      foreach (var ID in PersistentContainer.Instance.Players.IDs)
      {
        var member = PersistentContainer.Instance.Players[ID, true];
        if (member.ClanName == player.ClanName)
        {
          if (member.ClanIsInviteOnly)
          {
            member.ClanIsInviteOnly = false;

            if (ID == cInfo.CrossplatformId.CombinedString)
            {
              SendMessage.Private(cInfo, "You have set the clan to OPEN for anyone to join.");

              continue;
            }

            var member2 = ConsoleHelper.ParseParamIdOrName(ID, true, false);
            if (member2 != null)
            {
              SendMessage.Private(member2, $"Your clan has been set to Open for anyone to join by {cInfo.playerName}");
            }

            continue;
          }

          member.ClanIsInviteOnly = true;

          if (ID == cInfo.CrossplatformId.CombinedString)
          {
            SendMessage.Private(cInfo, "You have set the clan to Invite only.");

            continue;
          }

          var member3 = ConsoleHelper.ParseParamIdOrName(ID, true, false);
          if (member3 != null)
          {
            SendMessage.Private(member3, $"Your clan has been set to Invite only by {cInfo.playerName}");
          }
        }

        ClanIsInviteOnly[player.ClanName] = player.ClanIsInviteOnly;
      }

      PersistentContainer.Instance.Save();
    }

    public static void ToggleClanPublic(ClientInfo _cInfo)
    {
      var player = PersistentContainer.Instance.Players[_cInfo.CrossplatformId.CombinedString, true];
      if (!player.ClanLeader)
      {
        SendMessage.Private(_cInfo, "You do not have permission to use this command.");

        return;
      }

      if (player.ClanIsInviteOnly)
      {
        SendMessage.Private(_cInfo, "Your clan is already set to public");

        return;
      }

      foreach (var ID in PersistentContainer.Instance.Players.IDs)
      {
        var member = PersistentContainer.Instance.Players[ID, true];
        if (member.ClanName == player.ClanName)
        {
          if (member.ClanIsInviteOnly == false)
          {
            if (ID == _cInfo.CrossplatformId.CombinedString)
            {
              member.ClanIsInviteOnly = true;
              SendMessage.Private(_cInfo, "You have set the clan to Public");

              continue;
            }

            member.ClanIsInviteOnly = true;
            var cInfo2 = ConsoleHelper.ParseParamIdOrName(ID, true, false);
            if (cInfo2 != null)
            {
              SendMessage.Private(cInfo2, $"Your clan has been set to Public by {_cInfo.playerName}");
            }

            continue;
          }
        }

        PersistentContainer.Instance.Save();
        ClanIsInviteOnly[player.ClanName] = player.ClanIsInviteOnly;
      }
    }

    public static void ToggleClanPrivate(ClientInfo _cInfo)
    {
      var player = PersistentContainer.Instance.Players[_cInfo.CrossplatformId.CombinedString, true];
      if (!player.ClanLeader)
      {
        SendMessage.Private(_cInfo, "You do not have permission to use this command.");

        return;
      }

      if (!player.ClanIsInviteOnly)
      {
        SendMessage.Private(_cInfo, "Your clan is already set to Private");

        return;
      }

      foreach (var ID in PersistentContainer.Instance.Players.IDs)
      {
        var member = PersistentContainer.Instance.Players[ID, true];
        if (member.ClanName == player.ClanName)
        {
          if (member.ClanIsInviteOnly)
          {
            if (ID == _cInfo.CrossplatformId.CombinedString)
            {
              member.ClanIsInviteOnly = false;
              SendMessage.Private(_cInfo, "You have set the clan to Private.");

              continue;
            }

            member.ClanIsInviteOnly = false;
            var cInfo2 = ConsoleHelper.ParseParamIdOrName(ID, true, false);
            if (cInfo2 != null)
            {
              SendMessage.Private(cInfo2, $"Your clan has been set to Private by {_cInfo.playerName}");
            }

            continue;
          }
        }

        PersistentContainer.Instance.Save();
        ClanIsInviteOnly[player.ClanName] = player.ClanIsInviteOnly;
      }
    }

    public static void LoadClans()
    {
      foreach (var id1 in PersistentContainer.Instance.Players.IDs)
      {
        var member = PersistentContainer.Instance.Players[id1, true];
        if (!member.ClanLeader) { continue; }

        var members = new List<string>();
        foreach (var id2 in PersistentContainer.Instance.Players.IDs)
        {
          var member2 = PersistentContainer.Instance.Players[id2, true];
          if (member == member2) { continue; }

          if (member.ClanName == member2.ClanName)
          {
            members.Add(id2);
          }
        }

        members.Add(id1);
        Clans.Add(member.ClanName, members);
        ClanIsInviteOnly.Add(member.ClanName, member.ClanIsInviteOnly);
      }
    }

    public static void ListClans(ClientInfo _cInfo)
    {
      if (Clans.Count <= 0)
      {
        SendMessage.Private(_cInfo, "There are currently no clans.");

        return;
      }

      var i = 1;
      foreach (var name in Clans.Keys)
      {
        SendMessage.Private(_cInfo, $"{i}. {name} ({(ClanIsInviteOnly[name] ? "Private" : "Public")})");
        i++;
      }
    }

    public static void RemoveClan(ClientInfo _cInfo)
    {
      var player = PersistentContainer.Instance.Players[_cInfo.CrossplatformId.CombinedString, true];
      if (!player.ClanLeader) { return; }

      var oldClanName = player.ClanName;
      Clans.Remove(player.ClanName);
      ClanIsInviteOnly.Remove(player.ClanName);

      foreach (var ID in PersistentContainer.Instance.Players.IDs)
      {
        var member = PersistentContainer.Instance.Players[ID, true];
        if (member.ClanName != oldClanName) { continue; }

        member.ClanName = null;
        member.ClanTagColor = null;
        member.ClanIsInviteOnly = true;
        member.ClanLeader = false;
        member.ClanPromoter = false;

        if (member.ID == _cInfo.CrossplatformId.CombinedString) { continue; }

        var cInfo2 = ConsoleHelper.ParseParamIdOrName(ID, true, false);
        if (cInfo2 != null)
        {
          SendMessage.Private(cInfo2, $"Your clan has been deleted by {_cInfo.playerName}");
        }
      }

      SendMessage.Private(_cInfo, "You have deleted your clan.");
      PersistentContainer.Instance.Save();
    }

    public static void ListClanCommands(ClientInfo cInfo)
    {
      SendMessage.Private(cInfo, $"{Green}::[-]{White}Leader[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan public[-]");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}Leader[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan private[-]");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}Leader[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan promote [name][-]\"");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}Leader[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan demote [name][-]\"");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}Leader[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan color [hex value][-]");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}Leader[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan name [name][-]");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}Leader[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan delete[-]\"");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}Leader[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan resign to [name][-]\"");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}Officer[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan invite [name][-]\"");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}Officer[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan kick [name][-]\"");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}All members[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan leave[-]\"");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}All members[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan members[-]\"");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}No clan[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan create [name][-]\"");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}No clan[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan accept invite[-]\"");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}No clan[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan decline invite[-]\"");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}No clan[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan join [name][-]\"");
      SendMessage.Private(cInfo, $"{Green}::[-]{White}ALL[-]{Green}:: [-]\"{ChatMessage.CommandPrefix}clan list[-]\"");
    }

    public static void InviteToClan(ClientInfo cInfo, string playerName)
    {
      playerName = playerName.Replace($"{ChatMessage.CommandPrefix}clan invite  ", "");

      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];
      if (!player.ClanPromoter)
      {
        SendMessage.Private(cInfo, "You must be a clan officer to use this command.");

        return;
      }

      var members = Clans[player.ClanName];
      if (members.Count >= PlayerLimit)
      {
        SendMessage.Private(cInfo, $"The max players per clan is set to {PlayerLimit}");
        SendMessage.Private(cInfo, "Make space in the clan if you want this member.");

        return;
      }

      var ID = PersistentContainer.Instance.Players.GetId(playerName);
      if (ID == null)
      {
        SendMessage.Private(cInfo, "Could not find player");

        return;
      }

      var invitePlayer = PersistentContainer.Instance.Players[ID, true];
      if (invitePlayer.ClanName != null)
      {
        SendMessage.Private(cInfo, $"{invitePlayer.PlayerName} is already in a clan.");

        return;
      }

      if (invitePlayer.ClanInvite != null)
      {
        SendMessage.Private(cInfo, $"{invitePlayer.PlayerName} already has a clan invite pending. Ask them to deny it and invite again.");

        return;
      }

      invitePlayer.ClanInvite = player.ClanName;
      invitePlayer.InviteDate = DateTime.Now;
      PersistentContainer.Instance.Save();
      SendMessage.Private(cInfo, $"You have invited {invitePlayer.PlayerName} to your clan.");

      var cInfo2 = ConsoleHelper.ParseParamIdOrName(ID, true, false);
      if (cInfo2 == null) { return; }

      SendMessage.Private(cInfo2, $"{player.PlayerName} has invited you to join [u][{player.ClanTagColor}]{player.ClanName}[/u]");
      SendMessage.Private(cInfo2, $"Type \"{ChatMessage.CommandPrefix}clan accept invite\" to accept or \"{ChatMessage.CommandPrefix}clan decline invite\" to decline.");

      foreach (var steamId2 in Clans[player.ClanName])
      {
        var cInfo3 = ConsoleHelper.ParseParamIdOrName(steamId2, true, false);
        if (cInfo3 == null) { continue; }

        if (cInfo3 == cInfo) { continue; }

        SendMessage.Private(cInfo3, $"{cInfo2.playerName} has been invited to the clan.");
      }
    }

    public static void KickFromClan(ClientInfo cInfo, string playerName)
    {
      playerName = playerName.Replace($"{ChatMessage.CommandPrefix}clan kick ", "");

      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];
      if (!player.ClanPromoter)
      {
        SendMessage.Private(cInfo, "You do not have permission to do this.");

        return;
      }

      var ID = PersistentContainer.Instance.Players.GetId(playerName);
      if (ID == null)
      {
        SendMessage.Private(cInfo, "Could not find player");

        return;
      }

      var kickPlayer = PersistentContainer.Instance.Players[ID, true];
      if (kickPlayer.ClanPromoter)
      {
        if (!player.ClanLeader)
        {
          SendMessage.Private(cInfo, "You can not kick any position players.");

          return;
        }

        if (kickPlayer == player)
        {
          SendMessage.Private(cInfo, "You cannot kick yourself!");

          return;
        }
      }

      if (kickPlayer.ClanName != player.ClanName)
      {
        SendMessage.Private(cInfo, $"{kickPlayer.PlayerName} is not in your clan.");

        return;
      }

      Clans[kickPlayer.ClanName].Remove(cInfo.CrossplatformId.CombinedString);

      kickPlayer.ClanName = null;
      kickPlayer.ClanPromoter = false;
      kickPlayer.ClanIsInviteOnly = false;
      kickPlayer.ClanLeader = false;
      kickPlayer.ClanInvite = null;
      kickPlayer.ClanTagColor = null;

      PersistentContainer.Instance.Save();
      SendMessage.Private(cInfo, $"{kickPlayer.PlayerName} has been kicked from the clan.");

      var cInfo2 = ConsoleHelper.ParseParamIdOrName(ID, true, false);
      if (cInfo2 != null)
      {
        SendMessage.Private(cInfo2, $"You have been kicked from your clan by {cInfo.playerName}");
      }
    }

    public static void LeaveClan(ClientInfo cInfo)
    {
      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];
      if (player.ClanLeader)
      {
        SendMessage.Private(cInfo, "You cannot leave your clan without first handing it over or deleting it.");
        SendMessage.Private(cInfo, "To hand over type \"/resign clan to [name]\"");
        SendMessage.Private(cInfo, "To delete type \"/delete clan\"");

        return;
      }

      if (player.ClanName == null)
      {
        SendMessage.Private(cInfo, "You are not in a clan.");

        return;
      }

      var oldClan = player.ClanName;

      player.ClanName = null;
      player.ClanPromoter = false;
      player.ClanIsInviteOnly = false;
      player.ClanLeader = false;
      player.ClanInvite = null;
      player.ClanTagColor = null;

      PersistentContainer.Instance.Save();

      Clans[oldClan].Remove(player.ID);

      foreach (var steamId in Clans[oldClan])
      {
        var cInfo2 = ConsoleHelper.ParseParamIdOrName(steamId, true, false);
        if (cInfo2 != null)
        {
          SendMessage.Private(cInfo2, $"{cInfo.playerName} has left the clan.");
        }
      }

      SendMessage.Private(cInfo, "You have left the clan.");
    }

    public static void AcceptInvite(ClientInfo cInfo)
    {
      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];
      if (player.ClanName != null)
      {
        SendMessage.Private(cInfo, "You are already in a clan.");

        return;
      }

      if (player.ClanInvite == null) { return; }

      player.ClanName = player.ClanInvite;
      player.ClanPromoter = false;
      player.ClanLeader = false;
      player.ClanInvite = null;

      var inviteOnly = true;
      var tagColor = "";

      foreach (var ID in PersistentContainer.Instance.Players.IDs)
      {
        var member = PersistentContainer.Instance.Players[ID, true];
        if (member.ClanName != player.ClanName) { continue; }

        if (!member.ClanLeader) { continue; }

        tagColor = member.ClanTagColor;
        inviteOnly = member.ClanIsInviteOnly;
      }

      player.ClanIsInviteOnly = inviteOnly;
      player.ClanTagColor = tagColor;

      PersistentContainer.Instance.Save();
      SendMessage.Private(cInfo, $"You have joined {player.ClanName}");

      foreach (var steamId in Clans[player.ClanName])
      {
        var cInfo2 = ConsoleHelper.ParseParamIdOrName(steamId, true, false);
        if (cInfo2 != null)
        {
          SendMessage.Private(cInfo2, $"{cInfo.playerName} has joined the clan.");
        }
      }

      Clans[player.ClanName].Add(cInfo.CrossplatformId.CombinedString);
    }

    public static void DeclineInvite(ClientInfo cInfo)
    {
      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];
      if (player.ClanName != null)
      {
        SendMessage.Private(cInfo, "You are already in a clan.");

        return;
      }

      if (player.ClanInvite == null)
      {
        SendMessage.Private(cInfo, "You have no clan invites to decline. Try making some friends.");

        return;
      }

      SendMessage.Private(cInfo, "You have declined your clan invite.");

      foreach (var steamId in Clans[player.ClanInvite])
      {
        var member = ConsoleHelper.ParseParamIdOrName(steamId, true, false);
        if (member != null)
        {
          SendMessage.Private(member, $"{cInfo.playerName} has declined the clan invite.");
        }
      }

      player.ClanInvite = null;
      PersistentContainer.Instance.Save();
    }

    public static void InviteSelfToClan(ClientInfo cInfo, string clanName)
    {
      clanName = clanName.Replace($"{ChatMessage.CommandPrefix}clan join ", "");

      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];
      if (player.ClanName != null)
      {
        SendMessage.Private(cInfo, "You are already in a clan.");

        return;
      }

      if (!Clans.ContainsKey(clanName))
      {
        SendMessage.Private(cInfo, "Clan does not exist");

        return;
      }

      var clanMembers = Clans[clanName];
      var steamId = clanMembers.RandomObject();
      var member = PersistentContainer.Instance.Players[steamId, true];
      if (member.ClanIsInviteOnly)
      {
        SendMessage.Private(cInfo, $"{member.ClanName} is invite only. Please ask to be invited.");

        return;
      }

      player.ClanName = member.ClanName;
      player.ClanPromoter = false;
      player.ClanLeader = false;
      player.ClanInvite = null;
      player.ClanTagColor = member.ClanTagColor;

      PersistentContainer.Instance.Save();
      SendMessage.Private(cInfo, $"You have joined {player.ClanName}");

      foreach (var steamId2 in clanMembers)
      {
        if (steamId2 == cInfo.CrossplatformId.CombinedString) { continue; }

        var cInfo2 = ConsoleHelper.ParseParamIdOrName(steamId2, true, false);
        if (cInfo2 != null)
        {
          SendMessage.Private(cInfo2, $"{cInfo.playerName} has joined the clan.");
        }
      }

      Clans[player.ClanName].Add(cInfo.CrossplatformId.CombinedString);
    }

    public static void ResignClanToMember(ClientInfo cInfo, string playerName)
    {
      playerName = playerName.Replace($"{ChatMessage.CommandPrefix}clan resign to ", "");

      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];
      if (player.ClanName == null)
      {
        SendMessage.Private(cInfo, "You are not in a clan.");

        return;
      }

      if (!player.ClanLeader)
      {
        SendMessage.Private(cInfo, "You must be the clan leader to use this command.");

        return;
      }

      var ID = PersistentContainer.Instance.Players.GetId(playerName);
      if (ID == null)
      {
        SendMessage.Private(cInfo, "Could not find player.");

        return;
      }

      var member = PersistentContainer.Instance.Players[ID, true];
      if (player.ClanName != member.ClanName)
      {
        SendMessage.Private(cInfo, "You can only hand over the clan to a current member.");

        return;
      }

      player.ClanLeader = false;
      player.ClanPromoter = true;
      member.ClanLeader = true;
      member.ClanPromoter = true;

      PersistentContainer.Instance.Save();
      SendMessage.Private(cInfo, $"You have handed over the clan to {member.PlayerName}");

      foreach (var ID2 in Clans[player.ClanName])
      {
        var cInfo2 = ConsoleHelper.ParseParamIdOrName(ID2, true, false);
        if (cInfo2 == null) { continue; }

        SendMessage.Private(cInfo2, cInfo2.CrossplatformId.CombinedString == ID
            ? $"{cInfo.playerName} has handed over the clan to you."
            : $"{cInfo.playerName} has handed the clan over to {member.PlayerName}");
      }
    }

    public static void PromoteMember(ClientInfo cInfo, string playerName)
    {
      playerName = playerName.Replace($"{ChatMessage.CommandPrefix}clan promote ", "");

      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];
      if (!player.ClanLeader)
      {
        SendMessage.Private(cInfo, "You must be a clan leader to use this command.");

        return;
      }

      var ID = PersistentContainer.Instance.Players.GetId(playerName);
      if (ID == null)
      {
        SendMessage.Private(cInfo, "Could not find player.");

        return;
      }

      var promotePlayer = PersistentContainer.Instance.Players[ID, true];
      if (promotePlayer.ClanPromoter)
      {
        SendMessage.Private(cInfo, $"{promotePlayer.PlayerName} is already an officer.");

        return;
      }

      promotePlayer.ClanPromoter = true;
      SendMessage.Private(cInfo, $"{promotePlayer.PlayerName} can now add/remove players from the clan.");

      var cInfo2 = ConsoleHelper.ParseParamIdOrName(ID, true, false);
      if (cInfo2 != null)
      {
        SendMessage.Private(cInfo2, $"You have been given the position of officer by {cInfo.playerName}");
      }

      PersistentContainer.Instance.Save();
    }

    public static void DemoteMember(ClientInfo cInfo, string playerName)
    {
      playerName = playerName.Replace($"{ChatMessage.CommandPrefix}clan demote ", "");

      var player = PersistentContainer.Instance.Players[cInfo.CrossplatformId.CombinedString, true];
      if (!player.ClanLeader)
      {
        SendMessage.Private(cInfo, "You must be a clan leader to use this command.");

        return;
      }

      var ID = PersistentContainer.Instance.Players.GetId(playerName);
      if (ID == null)
      {
        SendMessage.Private(cInfo, "Could not find player.");

        return;
      }

      var demotePlayer = PersistentContainer.Instance.Players[ID, true];
      if (!demotePlayer.ClanPromoter)
      {
        SendMessage.Private(cInfo, $"{demotePlayer.PlayerName} is not a clan officer");

        return;
      }

      demotePlayer.ClanPromoter = false;

      SendMessage.Private(cInfo, $"{demotePlayer.PlayerName} has been removed from the officer position");

      var cInfo2 = ConsoleHelper.ParseParamIdOrName(ID, true, false);
      if (cInfo2 != null)
      {
        SendMessage.Private(cInfo2, $"You have been removed from officer by {cInfo.playerName}");
      }

      PersistentContainer.Instance.Save();
    }

    public static bool NameIsTaken(string requestedClan)
    {
      foreach (var clanName in Clans.Keys)
      {
        if (clanName.Equals(requestedClan, StringComparison.InvariantCultureIgnoreCase))
        {
          return true;
        }
      }

      return false;
    }

    public static string GetClanLeader(Player player)
    {
      var output = "";
      foreach (var ID in Clans[player.ClanName])
      {
        var member = PersistentContainer.Instance.Players[ID, true];

        if (!member.ClanLeader) { continue; }

        output = member.PlayerName;
      }

      return output;
    }
  }
}
