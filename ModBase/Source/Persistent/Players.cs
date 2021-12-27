using System;
using System.Collections.Generic;

namespace Botman
{
  [Serializable]
  public class Players
  {
    public Dictionary<string, Player> players = new Dictionary<string, Player>();
    public Dictionary<string, List<string>> clans = new Dictionary<string, List<string>>();

    public List<string> IDs => new List<string>(players.Keys);

    public Player this[string Id, bool create]
    {
      get
      {
        if (string.IsNullOrEmpty(Id))
        {
          return null;
        }

        if (players.ContainsKey(Id))
        {
          return players[Id];
        }

        if (create)
        {
          var player = new Player(Id);
          players.Add(Id, player);
          return player;
        }

        return null;
      }
    }

    public string GetId(string _nameORId)
    {
      var _cInfo = ConsoleHelper.ParseParamIdOrName(_nameORId, true, true);
      if (_cInfo != null)
      {
        return _cInfo.CrossplatformId.CombinedString;
      }

      foreach (var id in PersistentContainer.Instance.Players.IDs)
      {
        var name = PersistentContainer.Instance.Players[id, true].PlayerName;
        {
          if (name != null && name.StartsWith(_nameORId, StringComparison.InvariantCultureIgnoreCase))
          {
            return PersistentContainer.Instance.Players[id, true].ID;
          }
        }
      }

      return null;
    }
  }
}
