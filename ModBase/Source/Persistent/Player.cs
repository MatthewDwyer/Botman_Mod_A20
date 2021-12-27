using System;
using System.Runtime.Serialization;

namespace Botman
{
  [Serializable]
  public class Player
  {
    private readonly PlatformUserIdentifierAbs platformId;
    [OptionalField]
    private string chatColor;
    private bool chatName;
    private bool isChatMuted;
    private bool isOnline;
    private string playerName;
    private string lastPosition;
    private string markedForReturn;
    private int entityId;
    private int maxChatLength;

    //Clans//
    private bool clanleader;
    private bool clanpromoter;
    private bool claninviteonly;
    //private bool clanisfull;
    private int clanmembercount;
    private string clantag;
    private string clantagcolor;
    private string claninvite;

    private DateTime invitedate;

    public Player(PlatformUserIdentifierAbs platformId)
    {
      this.platformId = platformId;

    }

    public PlatformUserIdentifierAbs PlatformID => platformId;

    public string ChatColor
    {
      get => chatColor;
      set => chatColor = value;
    }

    public bool ChatName
    {
      get => chatName;
      set => chatName = value;
    }

    public string PlayerName
    {
      get => playerName;
      set => playerName = value;
    }

    public int MaxChatLength
    {
      get
      {
        if (maxChatLength == 0)
        {
          maxChatLength = 255;
        }
        return maxChatLength;
      }
      set => maxChatLength = value;
    }

    public bool IsChatMuted
    {
      get => isChatMuted;
      set => isChatMuted = value;
    }

    public string LastPosition
    {
      get => lastPosition;
      set => lastPosition = value;
    }

    public string MarkedForReturn
    {
      get => markedForReturn;
      set => markedForReturn = value;
    }
    
    public int EntityId
    {
      get => entityId;
      set => entityId = value;
    }

    public bool IsOnline
    {
      get => isOnline;
      set => isOnline = value;
    }

    //Clans

    public bool ClanLeader
    {
      get => clanleader;
      set => clanleader = value;
    }
    public bool ClanPromoter
    {
      get => clanpromoter;
      set => clanpromoter = value;
    }
    public bool ClanIsInviteOnly
    {
      get => claninviteonly;
      set => claninviteonly = value;
    }

    public int ClanMembersCount
    {
      get => clanmembercount;
      set => clanmembercount = value;
    }

    public string ClanName
    {
      get => clantag;
      set => clantag = value;
    }
    public string ClanTagColor
    {
      get => clantagcolor;
      set => clantagcolor = value;
    }
    public string ClanInvite
    {
      get => claninvite;
      set => claninvite = value;
    }

    public DateTime InviteDate
    {
      get => invitedate;
      set => invitedate = value;
    }
  }
}
