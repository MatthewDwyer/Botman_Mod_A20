using System.Collections.Generic;
using UnityEngine;

namespace Botman
{
  class LevelSystem
  {
    public static bool ShowLevelEnabled = false;
    public static bool MilestonesEnabled = false;
    public static bool AwardDukesEnabled = false;
    //public static int AwardDukesMinLevel = 2;
    public static int AwardDukesMaxLevel = 10;
    public static int AwardDukesAmount = 1000;
    public static int Ticker = 0;
    public static int TickerLimit = 2;
    public static string ShowLevelColor = "ff0000";
    public static Dictionary<int, string> MilestoneDict = new Dictionary<int, string>();
    public static Dictionary<int, string> MilestoneRewardDict = new Dictionary<int, string>();

    public static Dictionary<string, int> Players = new Dictionary<string, int>();

    public static void Update()
    {
      Ticker++;

      if (Ticker < TickerLimit) { return; }

      foreach (var entity in GameManager.Instance.World.Entities.dict.Values)
      {
        if (!(entity is EntityPlayer player)) { continue; }

        if (ConsoleHelper.ParseParamEntityIdToClientInfo(player.entityId.ToString()) == null) { return; }

        if (!Players.ContainsKey(ConsoleHelper.ParseParamEntityIdToClientInfo(player.entityId.ToString()).playerName))
        {
          Players.Add(ConsoleHelper.ParseParamEntityIdToClientInfo(player.entityId.ToString()).playerName, XUiM_Player.GetLevel(player));

          return;
        }

        if (Players[ConsoleHelper.ParseParamEntityIdToClientInfo(player.entityId.ToString()).playerName] == XUiM_Player.GetLevel(player)) { continue; }

        AnnounceMilestone(ConsoleHelper.ParseParamEntityIdToClientInfo(player.entityId.ToString()), XUiM_Player.GetLevel(player));
        AwardDukes(ConsoleHelper.ParseParamEntityIdToClientInfo(player.entityId.ToString()), XUiM_Player.GetLevel(player));
        Players[ConsoleHelper.ParseParamEntityIdToClientInfo(player.entityId.ToString()).playerName] = XUiM_Player.GetLevel(player);
        //Players.Remove(ConsoleHelper.ParseParamEntityIdToClientInfo(_player.entityId.ToString()).playerName);
        return;
      }

      Ticker = 0;
    }

    public static string PlayerLevel(EntityPlayer _player) =>
      (!string.IsNullOrEmpty(ShowLevelColor) ? "[" + ShowLevelColor + "]" : "")
        + " (Lv." + XUiM_Player.GetLevel(_player) + ")[-]";

    public static void AnnounceMilestone(ClientInfo _cInfo, int LevelAchieved)
    {
      if (!MilestonesEnabled) { return; }

      if (MilestoneDict.Count <= 0) { return; }

      if (!MilestoneDict.ContainsKey(LevelAchieved)) { return; }

      var message = MilestoneDict[LevelAchieved];

      if (message.Contains("[playername]"))
      {
        message = message.Replace("[playername]", _cInfo.playerName);
      }

      if (message.Contains("[lvl]"))
      {
        message = message.Replace("[lvl]", LevelAchieved.ToString());
      }

      if (message != string.Empty)
      {
        SendMessage.Public(message);
      }

      if (!MilestoneRewardDict.ContainsKey(LevelAchieved)) { return; }

      var quantity = 0;
      var quality = 0;
      var reward = MilestoneRewardDict[LevelAchieved].Split(' ');

      if (reward.Length == 0) { return; }

      var itemName = reward[0];
      if (string.IsNullOrWhiteSpace(itemName)) { return; }

      if (reward.Length > 1)
      {
        int.TryParse(reward[1], out quantity);
      }

      if (reward.Length > 2)
      {
        int.TryParse(reward[2], out quality);
      }

      if (itemName.Contains(","))
      {
        var items = itemName.Split(',');
        foreach (var name in items)
        {
          GiveItem(_cInfo, name, quantity, quality);
        }

        return;
      }

      GiveItem(_cInfo, itemName, quantity, quality);
    }

    public static void AwardDukes(ClientInfo _cInfo, int LevelAchieved)
    {
      if (!AwardDukesEnabled) { return; }

      if (LevelAchieved > AwardDukesMaxLevel || AwardDukesAmount <= 0) { return; }

      GiveItem(_cInfo, "casinoCoin", AwardDukesAmount, 0);
      SendMessage.Private(_cInfo, $"You've been awarded {AwardDukesAmount} dukes for reaching level {LevelAchieved}");
    }

    private static readonly System.Random Rnd = new System.Random();

    public static void GiveItem(ClientInfo cInfo, string itemName, int amount, int quality)
    {
      if (GameManager.Instance.World == null) { return; }

      var itemValue = new ItemValue(ItemClass.GetItem(itemName, true).type, false);

      if (quality == 0 & itemValue.HasQuality)
      {
        itemValue.Quality = Rnd.Next(1, 6);
      }

      if (quality >= 1 && itemValue.HasQuality)
      {
        itemValue.Quality = quality;
      }

      if (itemValue.HasModSlots)
      {

      }

      var entityItem = (EntityItem)EntityFactory.CreateEntity(new EntityCreationData
      {
        entityClass = EntityClass.FromString("item"),
        id = EntityFactory.nextEntityID++,
        itemStack = new ItemStack(itemValue, amount),
        pos = GameManager.Instance.World.Players.dict[cInfo.entityId].position,
        rot = new Vector3(20f, 0f, 20f),
        lifetime = 60f,
        belongsPlayerId = cInfo.entityId
      });

      GameManager.Instance.World.SpawnEntityInWorld(entityItem);
      cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, cInfo.entityId));
      GameManager.Instance.World.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Killed);
    }
  }

  class MilestoneReward
  {
    public string ItemName { get; set; }
    public int Quality { get; set; }
    public int Quantity { get; set; }

    public MilestoneReward(string itemName, int quality, int quantity)
    {
      ItemName = itemName;
      Quality = quality;
      Quantity = quantity;
    }
  }
}
