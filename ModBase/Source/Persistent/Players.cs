using System;
using System.Collections.Generic;

namespace Botman
{
  [Serializable]
  public class Players
  {
    public Dictionary<PlatformUserIdentifierAbs, Player> players = new Dictionary<PlatformUserIdentifierAbs, Player>();
    public Dictionary<PlatformUserIdentifierAbs, List<PlatformUserIdentifierAbs>> clans = new Dictionary<PlatformUserIdentifierAbs, List<PlatformUserIdentifierAbs>>();

    public List<PlatformUserIdentifierAbs> PlatformIDs => new List<PlatformUserIdentifierAbs>(players.Keys);

    public Player this[PlatformUserIdentifierAbs PlatformId, bool create]
    {
      get
      {
        if (string.IsNullOrEmpty(PlatformId.CombinedString))
        {
          return null;
        }

        if (players.ContainsKey(PlatformId))
        {
          return players[PlatformId];
        }

        if (create)
        {
          var player = new Player(PlatformId);
          players.Add(PlatformId, player);
          return player;
        }

        return null;
      }
    }

    public PlatformUserIdentifierAbs GetSteamId(string _nameorid)
    {
      //// todo: this is broken. Also needs to confirm it is a 'long' to be a valid steam id
      //if (_nameorid.Length == 23)
      //{
      //  return _nameorid;
      //}

      var _cInfo = ConsoleHelper.ParseParamIdOrName(_nameorid, true, true);
      if (_cInfo != null)
      {
        return _cInfo.PlatformId;
      }

      foreach (var PlatformId in PersistentContainer.Instance.Players.PlatformIDs)
      {
        var name = PersistentContainer.Instance.Players[PlatformId, true].PlayerName;
        {
          if (name != null && name.StartsWith(_nameorid, StringComparison.InvariantCultureIgnoreCase))
          {
            return PersistentContainer.Instance.Players[PlatformId, true].PlatformID;
          }
        }
      }

      return null;
    }
  }
}
