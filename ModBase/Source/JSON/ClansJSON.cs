using Botman.Commands;

namespace Botman
{
  class ClansJSON
  {
    public static void SendClanInfo()
    {
      if (BMClans.Clans.Count == 0)
      {
        SdtdConsole.Instance.Output("{ \"clancount\" : \"0\" }");

        return;
      }

      var clancount = 0;

      var output =
          "{ \"clancount\" : \"" + BMClans.Clans.Count + "\" , ";
      output += " \"clanlist\" : [ {";
      foreach (var clan in BMClans.Clans)
      {
        if (clancount != 0)
        {
          output += " , ";
        }

        var _randomPlayer = PersistentContainer.Instance.Players[clan.Value.RandomObject(), true];
        output += " \"" + clancount + "\" :  { ";
        //output += " \"clanname \" : \"" + clan.Key + "\" , ";
        output += " \"clanname\" : \"" + clan.Key + "\" , ";
        output += " \"membercount\" : \"" + clan.Value.Count + "\" , ";
        output += " \"isinviteonly\" : " + BMClans.ClanIsInviteOnly[clan.Key].ToString().ToLower() + " , ";
        output += " \"tagcolor\" : \"" + _randomPlayer.ClanTagColor + "\" , ";
        output += " \"clanleader\" : \"" + BMClans.GetClanLeader(_randomPlayer) + "\" , ";
        output += " \"officers\" : [ ";
        var officerfound = false;
        foreach (var member in clan.Value)
        {
          var P = PersistentContainer.Instance.Players[member, true];
          var name = P.PlayerName;
          if (P.ClanPromoter && !P.ClanLeader)
          {
            output += "\"" + name + "\" ,";
            officerfound = true;
          }
        }
        if (officerfound)
        {
          output = output.Substring(0, output.Length - 1) + " ] , ";
        }
        else
        {
          output = output + " ] , ";
        }
        output += " \"members\" : [ ";
        var membersfound = false;
        foreach (var member in clan.Value)
        {
          var P = PersistentContainer.Instance.Players[member, true];
          var name = P.PlayerName;

          if (!P.ClanLeader && !P.ClanPromoter)
          {
            output += "\"" + name + "\" ,";
            membersfound = true;
          }
        }
        if (membersfound)
        {
          output = output.Substring(0, output.Length - 1) + " ] ";
        }
        else
        {
          output = output + "] ";
        }

        output += " }";
        clancount++;
      }
      output += " } ] }";
      SdtdConsole.Instance.Output(output);
    }

    public static void Accepted()
    {
      var output = "{ \"accepted\" : null }";
      SdtdConsole.Instance.Output(output);
    }

    public static void Declined(string _reason)
    {
      var output = " { \"declined\" : \"" + _reason + "\" }";
      SdtdConsole.Instance.Output(output);
    }
  }
}
