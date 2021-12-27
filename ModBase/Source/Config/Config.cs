using Botman.Commands;
using Botman.Source.BotModEvents;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Botman
{
  internal class Config
  {
    public static string BotName = "[F63B3B]Botman";
    public static string Version = "2.3.17";
    public static bool VersionMatches;
    public static bool MapUpdateRequired;

    public static readonly string ConfigFilePath = Path.Combine(API.BotmanPath, ConfigFile);

    private const string ConfigFile = "Config.xml";
    private static bool updateConfigs;
    private static readonly FileSystemWatcher FileWatcher = new FileSystemWatcher(API.BotmanPath, ConfigFile);

    public static void Load()
    {
      LoadXml();
      InitFileWatcher();
    }

    private static void InitFileWatcher()
    {
      FileWatcher.Changed += OnFileChanged;
      FileWatcher.Created += OnFileChanged;
      FileWatcher.Deleted += OnFileChanged;
      FileWatcher.EnableRaisingEvents = true;
    }

    private static void OnFileChanged(object source, FileSystemEventArgs e)
    {
      if (!System.IO.File.Exists(ConfigFilePath))
      {
        UpdateXml();
      }
      LoadXml();

      if (VersionMatches) { return; }

      if (!File.Exists(ConfigFilePath)) { return; }

      SdtdConsole.Instance.Output("~Botman~ UPDATING CONFIG");
      File.Delete(ConfigFilePath);
    }

    public static void ReloadConfigs()
    {
      if (!File.Exists(ConfigFilePath)) { return; }

      VersionMatches = true;
      File.Delete(ConfigFilePath);
    }

    private static void LoadXml()
    {
      var ResetAreasCopy = BMAreaReset.ResetAreas;
      var ResetRegionsCopy = BMResetRegions.ManualResetRegions;
      if (!File.Exists(ConfigFilePath))
      {
        UpdateXml();
        return;
      }
      XmlDocument xmlDoc = new XmlDocument();
      try
      {
        xmlDoc.Load(ConfigFilePath);
      }
      catch (XmlException e)
      {
        Log.Error(string.Format("~Botman Notice~ Failed loading {0}: {1}", ConfigFilePath, e.Message));
        return;
      }
      XmlNode _XmlNode = xmlDoc.DocumentElement;
      foreach (XmlNode childNode in _XmlNode.ChildNodes)
      {
        if (childNode.Name == "CustomMessages")
        {
          foreach (XmlNode subChild in childNode.ChildNodes)
          {
            if (subChild.NodeType == XmlNodeType.Comment)
            {
              continue;
            }
            if (subChild.NodeType != XmlNodeType.Element)
            {
              Log.Warning(string.Format("~Botman Notice~ Unexpected XML node found in 'Config.xml' section: {0}", subChild.OuterXml));
              continue;
            }
            XmlElement _line = (XmlElement)subChild;
            if (!_line.HasAttribute("name"))
            {
              Log.Warning(string.Format("~Botman Notice~ Ignoring CustomMessages entry because of missing 'name' attribute: {0}", subChild.OuterXml));
              continue;
            }
            switch (_line.GetAttribute("name"))
            {
              case "login":
                {
                  if (!_line.HasAttribute("name_color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring login.name_color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("name_color"))
                  {
                    GameMessage.LogInNameColor = _line.GetAttribute("name_color");
                  }
                  if (!_line.HasAttribute("message_color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring login.message_color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("message_color"))
                  {
                    GameMessage.LoginMessageColor = _line.GetAttribute("message_color");
                  }
                  if (!_line.HasAttribute("message"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring login.message from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("message"))
                  {
                    GameMessage.LogInMessage = _line.GetAttribute("message");
                  }
                }
                break;
              case "logout":
                {
                  if (!_line.HasAttribute("name_color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring logout.name_color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("name_color"))
                  {
                    GameMessage.LogOutNameColor = _line.GetAttribute("name_color");
                  }
                  if (!_line.HasAttribute("message_color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring logout.message_color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("message_color"))
                  {
                    GameMessage.LogOutMessageColor = _line.GetAttribute("message_color");
                  }
                  if (!_line.HasAttribute("message"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring logout.message from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("message"))
                  {
                    GameMessage.LogOutMessage = _line.GetAttribute("message");
                  }
                }
                break;
              case "died":
                {
                  if (!_line.HasAttribute("name_color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring died.name_color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("name_color"))
                  {
                    GameMessage.PlayerDiedNameColor = _line.GetAttribute("name_color");
                  }
                  if (!_line.HasAttribute("message_color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring died.message_color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("message_color"))
                  {
                    GameMessage.PlayerDiedMessageColor = _line.GetAttribute("message_color");
                  }
                  if (!_line.HasAttribute("message"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring died.message from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("message"))
                  {
                    GameMessage.PlayerDied = _line.GetAttribute("message");
                  }
                }
                break;
              case "killed":
                {
                  if (!_line.HasAttribute("killer_name_color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring killed.killer_name_color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("killer_name_color"))
                  {
                    GameMessage.KillerColor = _line.GetAttribute("killer_name_color");
                  }
                  if (!_line.HasAttribute("victim_name_color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring killed.victim_name_color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("victim_name_color"))
                  {
                    GameMessage.VictimColor = _line.GetAttribute("victim_name_color");
                  }
                  if (!_line.HasAttribute("message_color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring killed.message_color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("message_color"))
                  {
                    GameMessage.PlayerKilledMessageColor = _line.GetAttribute("message_color");
                  }
                  if (!_line.HasAttribute("message"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring killed.message from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("message"))
                  {
                    GameMessage.PlayerKilled = _line.GetAttribute("message");
                  }
                }
                break;
            }
          }
        }
        if (childNode.Name == "Configs")
        {
          foreach (XmlNode subChild in childNode.ChildNodes)
          {
            if (subChild.NodeType == XmlNodeType.Comment)
            {
              continue;
            }
            if (subChild.NodeType != XmlNodeType.Element)
            {
              Log.Warning(string.Format("~Botman Notice~ Unexpected XML node found in 'Config.xml' section: {0}", subChild.OuterXml));
              continue;
            }
            XmlElement _line = (XmlElement)subChild;
            if (!_line.HasAttribute("name"))
            {
              Log.Warning(string.Format("~Botman Notice~ Ignoring Config entry because of missing 'name' attribute: {0}", subChild.OuterXml));
              continue;
            }
            switch (_line.GetAttribute("name"))
            {
              //Change 1.9//
              case "allocs_web_file":
                if (!_line.HasAttribute("location"))
                {
                  Log.Warning(string.Format("~Botman Notice~ Ignoring allocs_web_file.location from config.xml because of missing attribute: {0}", subChild.OuterXml));
                  continue;
                }
                if (_line.HasAttribute("location"))
                {
                  BMMapEditor.FilePath = _line.GetAttribute("location");
                }
                break;
              case "anticheat":
                if (!_line.HasAttribute("enabled"))
                {
                  Log.Warning(string.Format("~Botman Notice~ Ignoring anticheat.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                  continue;
                }
                if (!bool.TryParse(_line.GetAttribute("enabled"), out AntiCheat.Enabled))
                {
                  Log.Warning(string.Format("~Botman Notice~ Ignoring anticheat.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                  continue;
                }

                break;
              case "botname":
                {
                  if (!_line.HasAttribute("text"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring Bot.text from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("text"))
                  {
                    BotName = _line.GetAttribute("text");
                  }
                  if (!_line.HasAttribute("color-private"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring Bot.color-private from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("color-private"))
                  {
                    ChatMessage.PrivateTextColor = _line.GetAttribute("color-private");
                  }
                  if (!_line.HasAttribute("color-public"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring Bot.color-public from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("color-public"))
                  {
                    ChatMessage.PublicTextColor = _line.GetAttribute("color-public");
                  }
                }
                break;
              case "chatcommands":
                {
                  if (!_line.HasAttribute("prefix"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring chatcommands.prefix from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("prefix"))
                  {
                    ChatMessage.CommandPrefix = _line.GetAttribute("prefix");
                  }
                  if (!_line.HasAttribute("hide"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring chatcommands.hide from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("hide"), out ChatMessage.Hide))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring chatcommands.hide from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                }
                break;
                case "clans":
                    {
                        if (!_line.HasAttribute("enabled"))
                        {
                            Log.Warning(string.Format("~Botman Notice~ Ignoring clans.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!bool.TryParse(_line.GetAttribute("enabled"), out BMClans.Enabled))
                        {
                            Log.Warning(string.Format("~Botman Notice~ Ignoring clans.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!_line.HasAttribute("max_clans"))
                        {
                            Log.Warning(string.Format("~Botman Notice~ Ignoring clans.max_clans from config.xml because of missing attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!int.TryParse(_line.GetAttribute("max_clans"), out BMClans.MaxClansAllowed))
                        {
                            Log.Warning(string.Format("~Botman Notice~ Ignoring clans.max_clans from config.xml entry because of invalid (non-numeric) value: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!_line.HasAttribute("max_players"))
                        {
                            Log.Warning(string.Format("~Botman Notice~ Ignoring clans.max_players from config.xml because of missing attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!int.TryParse(_line.GetAttribute("max_players"), out BMClans.PlayerLimit))
                        {
                            Log.Warning(string.Format("~Botman Notice~ Ignoring clans.max_players from config.xml entry because of invalid (non-numeric) value: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!_line.HasAttribute("required_level_to_create"))
                        {
                            Log.Warning(string.Format("~Botman Notice~ Ignoring clans.required_level_to_create from config.xml because of missing attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!int.TryParse(_line.GetAttribute("required_level_to_create"), out BMClans.LevelToCreate))
                        {
                            Log.Warning(string.Format("~Botman Notice~ Ignoring clans.required_level_to_create from config.xml entry because of invalid (non-numeric) value: {0}", subChild.OuterXml));
                            continue;
                        }
                    }
                    break;
                case "custommessages":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring custommessages.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out GameMessage.Enabled))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring custommessages.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                }
                break;
              case "dropminer":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format(
                      "~Botman Notice~ Ignoring dropminer.enabled from config.xml because of missing attribute: {0}",
                      subChild.OuterXml));
                    continue;
                  }

                  if (!bool.TryParse(_line.GetAttribute("enabled"), out Botman.Patches.FallingBlocks.enabled))
                  {
                    Log.Warning(string.Format(
                      "~Botman Notice~ Ignoring BMDropMiner.enabled from config.xml because of missing attribute: {0}",
                      subChild.OuterXml));
                    continue;
                  }
                }
                break;
              case "chat_level_prefix":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring chat_level_prefix.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out LevelSystem.ShowLevelEnabled))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring chat_level_prefix.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }

                  if (!_line.HasAttribute("color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring chat_level_prefix.color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  LevelSystem.ShowLevelColor = _line.GetAttribute("color");
                }
                break;
              case "level_achievement_reward":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring level_achievement_reward.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out LevelSystem.AwardDukesEnabled))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring level_achievement_reward.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }

                  if (!_line.HasAttribute("dukes"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring level_achievement_reward.dukes from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!int.TryParse(_line.GetAttribute("dukes"), out LevelSystem.AwardDukesAmount))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring level_achievement_reward.dukes from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!_line.HasAttribute("max_level"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring level_achievement_reward.max_level from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!int.TryParse(_line.GetAttribute("max_level"), out LevelSystem.AwardDukesMaxLevel))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring level_achievement_reward.max_level from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                }
                break;
              case "milestones":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring milestones.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out LevelSystem.MilestonesEnabled))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring milestones.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                }
                break;
              case "resetareas":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetareas.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out bool option))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetareas.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (option != BMAreaReset.Enabled)
                  {
                    MapUpdateRequired = true;
                    BMAreaReset.Enabled = option;
                  }
                  if (!_line.HasAttribute("days_between_resets"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetareas.days_between_resets from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!int.TryParse(_line.GetAttribute("days_between_resets"), out BMAreaReset.DaysBetweenReset))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetareas.days_between_resets from config.xml entry because of invalid (non-numeric) value: {0}", subChild.OuterXml));
                    continue;
                  }
                }
                break;
              case "resetallprefabs":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetallprefabs.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out BMResetAllPrefabs.Enabled))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetallprefabs.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!_line.HasAttribute("days_between_resets"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetallprefabs.days_between_resets from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!int.TryParse(_line.GetAttribute("days_between_resets"), out BMResetAllPrefabs.DaysBetweenReset))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetallprefabs.days_between_resets from config.xml entry because of invalid (non-numeric) value: {0}", subChild.OuterXml));
                    continue;
                  }
                }
                break;
              case "resetregions":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetregions.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out BMResetRegions.Enabled))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetregions.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!_line.HasAttribute("prefabsonly"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetregions.prefabsonly from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("prefabsonly"), out BMResetRegions.PrefabsOnly))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetregions.prefabsonly from config.xml entry because of invalid (non-numeric) value: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!_line.HasAttribute("days_between_resets"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetregions.days_between_resets from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!int.TryParse(_line.GetAttribute("days_between_resets"), out BMResetRegions.DaysBetweenReset))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetregions.days_between_resets from config.xml entry because of invalid (non-numeric) value: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!_line.HasAttribute("remove_lcbs"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetregions.remove_lcbs from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("remove_lcbs"), out BMResetRegions.RemoveLCBs))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring resetregions.remove_lcbs from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                }
                break;
              case "webmaptraceresetregions":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring webmaptraceresetregions.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out bool option))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring webmaptraceresetregions.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (BMWebMapTraceResetRegions.Enabled != option)
                  {
                    MapUpdateRequired = true;
                    BMWebMapTraceResetRegions.Enabled = option;
                  }
                  string coloroption = "";
                  if (!_line.HasAttribute("color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ webmaptraceresetregions.color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("color"))
                  {
                    coloroption = _line.GetAttribute("color");
                  }
                  if (coloroption != BMWebMapTraceResetRegions.Color)
                  {
                    MapUpdateRequired = true;
                    BMWebMapTraceResetRegions.Color = coloroption;
                  }
                }
                break;
              case "webmaptraceprefabs":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring webmaptraceprefabs.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out bool option))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring webmaptraceprefabs.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (BMWebMapTracePrefabs.Enabled != option)
                  {
                    MapUpdateRequired = true;
                    BMWebMapTracePrefabs.Enabled = option;
                  }
                  if (!_line.HasAttribute("color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ webmaptraceprefabs.color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  string coloroption = "";
                  if (_line.HasAttribute("color"))
                  {
                    coloroption = _line.GetAttribute("color");
                  }
                  if (coloroption != BMWebMapTracePrefabs.Color)
                  {
                    MapUpdateRequired = true;
                    BMWebMapTracePrefabs.Color = coloroption;
                  }
                }
                break;
              case "webmaptracetraders":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring webmaptracetraders.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out bool option))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring webmaptracetraders.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (BMWebMapTraceTraders.Enabled != option)
                  {
                    MapUpdateRequired = true;
                    BMWebMapTraceTraders.Enabled = option;
                  }
                  if (!_line.HasAttribute("color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ webmaptracetraders.color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  string coloroption = "";
                  if (_line.HasAttribute("color"))
                  {
                    coloroption = _line.GetAttribute("color");
                  }
                  if (coloroption != BMWebMapTraceTraders.Color)
                  {
                    MapUpdateRequired = true;
                    BMWebMapTraceTraders.Color = coloroption;
                  }
                }
                break;
              case "webmaptraceresetareas":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring webmaptraceresetareas.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out bool option))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring webmaptraceresetareas.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (BMWebMapTraceResetArea.Enabled != option)
                  {
                    MapUpdateRequired = true;
                    BMWebMapTraceResetArea.Enabled = option;
                  }
                  string coloroption = "";
                  if (!_line.HasAttribute("color"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ webmaptraceresetareas.color from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (_line.HasAttribute("color"))
                  {
                    coloroption = _line.GetAttribute("color");
                  }
                  if (coloroption != BMWebMapTraceResetArea.Color)
                  {
                    MapUpdateRequired = true;
                    BMWebMapTraceResetArea.Color = coloroption;
                  }
                }
                break;
              case "lcbprefabrule":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring lcbprefabrule.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out LCBPlacement.PrefabRangeEnabled))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring lcbprefabrule.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!int.TryParse(_line.GetAttribute("distance"), out LCBPlacement.PrefabRangeSearch))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring lcbprefabrule.distance from config.xml entry because of invalid (non-numeric) value: {0}", subChild.OuterXml));
                    continue;
                  }
                }
                break;
              case "zombieannouncer":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring zombieannouncer.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out EntityWatch.Enabled))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring zombieannouncer.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                }
                break;
              case "zombiefreetime":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring zombiefreetime.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out BMZombieFreeTime.Enabled))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring zombiefreetime.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!_line.HasAttribute("start"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring zombiefreetime.start from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!int.TryParse(_line.GetAttribute("start"), out BMZombieFreeTime.Start))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring zombiefreetime.start from config.xml entry because of invalid (non-numeric) value: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!_line.HasAttribute("end"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring zombiefreetime.end from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!int.TryParse(_line.GetAttribute("end"), out BMZombieFreeTime.End))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring zombiefreetime.end from config.xml entry because of invalid (non-numeric) value: {0}", subChild.OuterXml));
                    continue;
                  }
                }
                break;
              case "zones":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring zones.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out BMSanctuaries.Enabled))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring zones.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                }
                break;
              case "vehiclefiledelete":
                {
                  if (!_line.HasAttribute("enabled"))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring vehiclefiledelete.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                  if (!bool.TryParse(_line.GetAttribute("enabled"), out BMVehicleFileDelete.Enabled))
                  {
                    Log.Warning(string.Format("~Botman Notice~ Ignoring vehiclefiledelete.enabled from config.xml because of missing attribute: {0}", subChild.OuterXml));
                    continue;
                  }
                }
                break;
              case "version":
                {
                  if (_line.HasAttribute("value"))
                  {
                    if (Version == _line.GetAttribute("value"))
                    {
                      VersionMatches = true;
                      continue;
                    }
                  }
                }
                break;
            }
          }
        }
        if (childNode.Name == "Milestones")
        {
          LevelSystem.MilestoneDict.Clear();
          LevelSystem.MilestoneRewardDict.Clear();
          foreach (XmlNode subChild in childNode.ChildNodes)
          {
            if (subChild.NodeType == XmlNodeType.Comment)
            {
              continue;
            }
            if (subChild.NodeType != XmlNodeType.Element)
            {
              Log.Warning(string.Format("~Botman Notice~ Unexpected XML node found in 'Milestones' section of Config.xml: {0}", subChild.OuterXml));
              continue;
            }
            XmlElement _line = (XmlElement)subChild;
            if (!_line.HasAttribute("lvl"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring milestone.lvl entry because of missing lvl attribute: {0}", _line.GetAttribute("lvl")));
              continue;
            }
            if (_line.GetAttribute("lvl").Equals("#"))
            {
              continue;
            }
            if (!int.TryParse(_line.GetAttribute("lvl"), out int lvl))
            {
              Log.Warning(string.Format("~Botman Notice~ Ignoring milestone.lvl from config.xml entry because of invalid (non-numeric) value: {0}", subChild.OuterXml));
              continue;
            }
            if (!_line.HasAttribute("message"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring milestone.message entry because of missing message attribute: {0}", subChild.OuterXml));
              continue;
            }

            string _message = _line.GetAttribute("message");
            if (!_line.HasAttribute("reward"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring milestone.reward entry because of missing message attribute: {0}", subChild.OuterXml));
              continue;
            }
            string reward = _line.GetAttribute("reward");
            if (!_line.HasAttribute("quantity"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring milestone.quantity entry because of missing message attribute: {0}", subChild.OuterXml));
              continue;
            }
            reward = reward + " " + _line.GetAttribute("quantity");
            if (!_line.HasAttribute("quality"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring milestone.quality entry because of missing message attribute: {0}", subChild.OuterXml));
              continue;
            }
            reward = reward + " " + _line.GetAttribute("quality");
            if (LevelSystem.MilestoneDict.ContainsKey(lvl))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring milestone.lvl {0} entry because of duplicate entry", lvl));
              continue;
            }
            LevelSystem.MilestoneDict.Add(lvl, _message);
            if (reward != null || reward != string.Empty)
            {
              LevelSystem.MilestoneRewardDict.Add(lvl, reward);
            }
          }
        }
        ///////////////////////////////
        if (childNode.Name == "ResetRegions")
        {
          BMResetRegions.PrefabsInRegion.Clear();
          BMResetRegions.ManuallyAddedPrefabsInRegion.Clear();
          BMResetRegions.ManualResetRegions.Clear();
          foreach (XmlNode subChild in childNode.ChildNodes)
          {
            if (subChild.NodeType == XmlNodeType.Comment)
            {
              continue;
            }
            if (subChild.NodeType != XmlNodeType.Element)
            {
              Log.Warning(string.Format("~Botman Notice~ Unexpected XML node found in 'ResetRegions' section of Config.xml: {0}", subChild.OuterXml));
              continue;
            }
            XmlElement _line = (XmlElement)subChild;
            if (!_line.HasAttribute("type"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring ResetRegions.type entry because of missing name attribute: {0}", subChild.OuterXml));
              continue;
            }
            string _type = "";
            string _region = "";
            if (!_line.HasAttribute("type"))
            {
              Log.Warning(string.Format("~Botman Notice~ Ignoring ResetRegions.type from config.xml because of missing attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (_line.HasAttribute("type"))
            {
              _type = _line.GetAttribute("type");
              _type = _type.ToLower();
            }

            if (!_line.HasAttribute("region"))
            {
              Log.Warning(string.Format("~Botman Notice~ Ignoring ResetRegions.region from config.xml because of missing attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (_line.HasAttribute("region"))
            {
              _region = _line.GetAttribute("region");
            }

            if (_type == "manual")
            {
              if (!BMResetRegions.ManualResetRegions.Contains(_region))
              {
                BMResetRegions.ManualResetRegions.Add(_region);
              }
              else
              {
                continue;
              }
            }
          }

          BMResetRegions.ReloadPrefabsList();
        }
        ///////////////////////////////////////////////////////////////////////
        if (childNode.Name == "ZombieAnnouncer")
        {
          EntityWatch.AlertMessage.Clear();
          foreach (XmlNode subChild in childNode.ChildNodes)
          {
            if (subChild.NodeType == XmlNodeType.Comment)
            {
              continue;
            }
            if (subChild.NodeType != XmlNodeType.Element)
            {
              Log.Warning(string.Format("~Botman Notice~ Unexpected XML node found in 'ZombieAnnouncer' section of Config.xml: {0}", subChild.OuterXml));
              continue;
            }
            XmlElement _line = (XmlElement)subChild;
            if (!_line.HasAttribute("name"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring ZombieAnnouncer.entity entry because of missing name attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (!_line.HasAttribute("message"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring ZombieAnnouncer.entity entry because of missing message attribute: {0}", subChild.OuterXml));
              continue;
            }

            string _name = "";
            string _message = "";

            if (!_line.HasAttribute("name"))
            {
              Log.Warning(string.Format("~Botman Notice~ Ignoring ZombieAnnouncer.entity.name from config.xml because of missing attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (_line.HasAttribute("name"))
            {
              _name = _line.GetAttribute("name");
              _name = _name.ToLower();
            }

            if (!_line.HasAttribute("message"))
            {
              Log.Warning(string.Format("~Botman Notice~ Ignoring ZombieAnnouncer.entity.message from config.xml because of missing attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (_line.HasAttribute("message"))
            {
              _message = _line.GetAttribute("message");
            }

            if (!EntityWatch.AlertMessage.ContainsKey(_name))
            {
              EntityWatch.AlertMessage.Add(_name, _message);
            }
            else
            {
              Log.Warning("~Botman Notice~ Ignoring ZombieAnnouncer.entity name: " + _name + " - entity already on the list.");
              continue;
            }
          }
        }
        if (childNode.Name == "ExemptPrefabs")
        {
          PrefabReset.ExemptList.Clear();
          foreach (XmlNode subChild in childNode.ChildNodes)
          {
            if (subChild.NodeType == XmlNodeType.Comment)
            {
              continue;
            }
            if (subChild.NodeType != XmlNodeType.Element)
            {
              Log.Warning(string.Format("~Botman Notice~ Unexpected XML node found in 'ExemptPrefabs' section of Config.xml: {0}", subChild.OuterXml));
              continue;
            }
            XmlElement _line = (XmlElement)subChild;
            if (!_line.HasAttribute("name"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring ExemptPrefabs.name entry because of missing name attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (!PrefabReset.ExemptList.Contains(_line.GetAttribute("name")))
            {
              if (_line.GetAttribute("name").Equals("Prefab_Name_Here_01")) { continue; }
              PrefabReset.ExemptList.Add(_line.GetAttribute("name"));
            }
          }
        }
        if (childNode.Name == "Zones")
        {
          foreach (XmlNode subChild in childNode.ChildNodes)
          {
            if (subChild.NodeType == XmlNodeType.Comment)
            {
              continue;
            }
            if (subChild.NodeType != XmlNodeType.Element)
            {
              Log.Warning(string.Format("~Botman Notice~ Unexpected XML node found in 'Zones' section of Config.xml: {0}", subChild.OuterXml));
              continue;
            }
            XmlElement _line = (XmlElement)subChild;
            if (!_line.HasAttribute("name"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring Zones.name entry because of missing name attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (!_line.HasAttribute("corner1"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring Zones.corner1 entry because of missing message attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (!_line.HasAttribute("corner2"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring Zones.corner2 entry because of missing message attribute: {0}", subChild.OuterXml));
              continue;
            }
            string _name = "";
            Vector3i _corner1 = Vector3i.zero;
            Vector3i _corner2 = Vector3i.zero;
            if (!_line.HasAttribute("name"))
            {
              Log.Warning(string.Format("~Botman Notice~ Ignoring Zones.name from config.xml because of missing attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (_line.HasAttribute("name"))
            {
              _name = _line.GetAttribute("name");
              _name = _name.ToLower();
            }

            if (!_line.HasAttribute("corner1"))
            {
              Log.Warning(string.Format("~Botman Notice~ Ignoring Zones.corner1 from config.xml because of missing attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (_line.HasAttribute("corner1"))
            {
              _corner1 = Vector3i.Parse(_line.GetAttribute("corner1"));
            }
            if (!_line.HasAttribute("corner2"))
            {
              Log.Warning(string.Format("~Botman Notice~ Ignoring Zones.corner2 from config.xml because of missing attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (_line.HasAttribute("corner2"))
            {
              _corner2 = Vector3i.Parse(_line.GetAttribute("corner2"));
            }
            if (!BMSanctuaries.Sanctuaries.ContainsKey(_name))
            {
              if (_corner1 != Vector3i.zero && _corner2 != Vector3i.zero)
              {

                SanctuaryData _zd = new SanctuaryData();
                _zd.Corner1 = _corner1;
                _zd.Corner2 = _corner2;

                BMSanctuaries.Sanctuaries.Add(_name, _zd);
              }
            }
          }
        }
        if (childNode.Name == "ResetAreas")
        {
          BMAreaReset.ResetAreas.Clear();
          foreach (XmlNode subChild in childNode.ChildNodes)
          {
            if (subChild.NodeType == XmlNodeType.Comment)
            {
              continue;
            }
            if (subChild.NodeType != XmlNodeType.Element)
            {
              Log.Warning(string.Format("~Botman Notice~ Unexpected XML node found in 'ResetAreas' section of Config.xml: {0}", subChild.OuterXml));
              continue;
            }
            XmlElement _line = (XmlElement)subChild;
            string _name = "";
            if (!_line.HasAttribute("name"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring ResetAreas.name entry because of missing name attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (_line.HasAttribute("name"))
            {
              _name = _line.GetAttribute("name");
              _name = _name.ToLower();
            }
            if (!_line.HasAttribute("x1"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring ResetAreas.x1 entry because of missing message attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (!int.TryParse(_line.GetAttribute("x1"), out int num))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring ResetAreas.x1. Entry is not an integer"));
              continue;
            }
            if (!_line.HasAttribute("z1"))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring ResetAreas.z1 entry because of missing message attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (!int.TryParse(_line.GetAttribute("z1"), out int num2))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring ResetAreas.z1. Entry is not an integer"));
              continue;
            }

            if (!_line.HasAttribute("x2"))
            {
              Log.Warning(string.Format("~Botman Notice~ Ignoring ResetAreas.x2 from config.xml because of missing attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (!int.TryParse(_line.GetAttribute("x2"), out int num3))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring ResetAreas.x2. Entry is not an integer"));
              continue;
            }
            if (!_line.HasAttribute("z2"))
            {
              Log.Warning(string.Format("~Botman Notice~ Ignoring ResetAreas.z2 from config.xml because of missing attribute: {0}", subChild.OuterXml));
              continue;
            }
            if (!int.TryParse(_line.GetAttribute("z2"), out int num4))
            {
              Log.Warning(string.Format("*~Botman Notice~ Ignoring ResetAreas.z2. Entry is not an integer"));
              continue;
            }
            if (!BMAreaReset.ResetAreas.ContainsKey(_name))
            {
              string area = $"x{num} z{num2} xx{num3} zz{num4}";
              BMAreaReset.ResetAreas.Add(_name, area);
            }
          }
        }
      }
      if (MapUpdateRequired || ResetRegionsCopy != BMResetRegions.ManualResetRegions || ResetAreasCopy != BMAreaReset.ResetAreas)
      {
        BMMapEditor.Update();
        MapUpdateRequired = false;
      }
      if (updateConfigs)
      {
        UpdateXml();
      }

      updateConfigs = false;

    }

    public static void UpdateXml()
    {
      FileWatcher.EnableRaisingEvents = false;
      using (StreamWriter sw = new StreamWriter(ConfigFilePath))
      {
        sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sw.WriteLine("<Botman>");
        sw.WriteLine(string.Empty);
        sw.WriteLine("    <Configs>");
        sw.WriteLine(string.Format("        <config name=\"allocs_web_file\" location=\"{0}\"/> <!-- Specific directory location to \"Mods/Allocs_WebAndMapRendering/webserver/js/map.js\" -->", BMMapEditor.FilePath));
        sw.WriteLine(string.Format("        <config name=\"anticheat\" enabled=\"{0}\" />", AntiCheat.Enabled));
        sw.WriteLine(string.Format("        <config name=\"botname\" text=\"{0}\" color-public=\"{1}\" color-private=\"{2}\" />", BotName, ChatMessage.PublicTextColor, ChatMessage.PrivateTextColor));
        sw.WriteLine(string.Format("        <config name=\"chat_level_prefix\" enabled=\"{0}\" color=\"{1}\"/>", LevelSystem.ShowLevelEnabled, LevelSystem.ShowLevelColor));
        sw.WriteLine(string.Format("        <config name=\"chatcommands\" prefix=\"{0}\" hide=\"{1}\" />", ChatMessage.CommandPrefix, ChatMessage.Hide));
//        sw.WriteLine(string.Format("        <config name=\"clans\" enabled=\"{0}\" max_clans=\"{1}\" max_players=\"{2}\" required_level_to_create=\"{3}\" />", BMClans.Enabled, BMClans.MaxClansAllowed, BMClans.PlayerLimit, BMClans.LevelToCreate));
        sw.WriteLine(string.Format("        <config name=\"dropminer\" enabled=\"{0}\" />", Patches.FallingBlocks.enabled));
        sw.WriteLine(string.Format("        <config name=\"custommessages\" enabled=\"{0}\" />", GameMessage.Enabled));
        sw.WriteLine(string.Format("        <config name=\"level_achievement_reward\" enabled=\"{0}\" dukes=\"{1}\" max_level=\"{2}\" />", LevelSystem.AwardDukesEnabled, LevelSystem.AwardDukesAmount, LevelSystem.AwardDukesMaxLevel));
        sw.WriteLine(string.Format("        <config name=\"lcbprefabrule\" enabled=\"{0}\" distance=\"{1}\" />", LCBPlacement.PrefabRangeEnabled, LCBPlacement.PrefabRangeSearch));
        sw.WriteLine(string.Format("        <config name=\"milestones\" enabled=\"{0}\" />", LevelSystem.ShowLevelEnabled, LevelSystem.MilestonesEnabled, LevelSystem.MilestonesEnabled));
        sw.WriteLine(string.Format("        <config name=\"resetallprefabs\" enabled=\"{0}\" days_between_resets=\"{1}\"  />", BMResetAllPrefabs.Enabled, BMResetAllPrefabs.DaysBetweenReset));
        sw.WriteLine(string.Format("        <config name=\"resetareas\" enabled=\"{0}\" days_between_resets=\"{1}\"  />", BMAreaReset.Enabled, BMAreaReset.DaysBetweenReset));
        sw.WriteLine(string.Format("        <config name=\"resetregions\" enabled=\"{0}\" prefabsonly=\"{1}\" days_between_resets=\"{2}\" remove_lcbs=\"{3}\" />", BMResetRegions.Enabled, BMResetRegions.PrefabsOnly, BMResetRegions.DaysBetweenReset, BMResetRegions.RemoveLCBs));
        sw.WriteLine(string.Format("        <config name=\"vehiclefiledelete\" enabled=\"{0}\" />", BMVehicleFileDelete.Enabled));
        sw.WriteLine(string.Format("        <config name=\"webmaptraceprefabs\" enabled=\"{0}\" color=\"{1}\" />", BMWebMapTracePrefabs.Enabled, BMWebMapTracePrefabs.Color));
        sw.WriteLine(string.Format("        <config name=\"webmaptracetraders\" enabled=\"{0}\" color=\"{1}\" />", BMWebMapTraceTraders.Enabled, BMWebMapTraceTraders.Color));
        sw.WriteLine(string.Format("        <config name=\"webmaptraceresetareas\" enabled=\"{0}\" color=\"{1}\"/>", BMWebMapTraceResetArea.Enabled, BMWebMapTraceResetArea.Color));
        sw.WriteLine(string.Format("        <config name=\"webmaptraceresetregions\" enabled=\"{0}\" color=\"{1}\"/>", BMWebMapTraceResetRegions.Enabled, BMWebMapTraceResetRegions.Color));
        sw.WriteLine(string.Format("        <config name=\"zombieannouncer\" enabled=\"{0}\" />", EntityWatch.Enabled));
        sw.WriteLine(string.Format("        <config name=\"zombiefreetime\" enabled=\"{0}\" start=\"{1}\" end=\"{2}\" />", BMZombieFreeTime.Enabled, BMZombieFreeTime.Start, BMZombieFreeTime.End));
        sw.WriteLine(string.Format("        <config name=\"zones\" enabled=\"{0}\" />", BMSanctuaries.Enabled));
        sw.WriteLine(string.Format("        <config name=\"version\" value=\"{0}\" />", Version));
        sw.WriteLine("    </Configs>");
        sw.WriteLine(string.Empty);
        sw.WriteLine("    <CustomMessages>");
        sw.WriteLine(string.Format("        <message name=\"login\" name_color=\"{0}\" message_color=\"{1}\" message=\"{2}\" />", GameMessage.LogInNameColor, GameMessage.LoginMessageColor, GameMessage.LogInMessage));
        sw.WriteLine(string.Format("        <message name=\"logout\" name_color=\"{0}\" message_color=\"{1}\" message=\"{2}\" />", GameMessage.LogOutNameColor, GameMessage.LogOutMessageColor, GameMessage.LogOutMessage));
        sw.WriteLine(string.Format("        <message name=\"died\" name_color=\"{0}\" message_color=\"{1}\" message=\"{2}\" />", GameMessage.PlayerDiedNameColor, GameMessage.PlayerDiedMessageColor, GameMessage.PlayerDied));
        sw.WriteLine(string.Format("        <message name=\"killed\" killer_name_color=\"{0}\" victim_name_color=\"{1}\" message_color=\"{2}\" message=\"{3}\" />", GameMessage.KillerColor, GameMessage.VictimColor, GameMessage.PlayerKilledMessageColor, GameMessage.PlayerKilled));
        sw.WriteLine("    </CustomMessages>");
        sw.WriteLine(string.Empty);
        sw.WriteLine("    <Milestones>");
        if (LevelSystem.MilestoneDict.Count > 0)
        {
          foreach (KeyValuePair<int, string> kvp in LevelSystem.MilestoneDict)
          {
            string itemname = "";
            string quantity = "0";
            string quality = "0";
            if (LevelSystem.MilestoneRewardDict.ContainsKey(kvp.Key))
            {
              string[] _p = LevelSystem.MilestoneRewardDict[kvp.Key].Split(' ');
              itemname = _p[0];
              quantity = "1";
              quality = "1";
              if (_p.Count() > 1)
              {
                quantity = _p[1];
              }
              if (_p.Count() > 2)
              {
                quality = _p[2];
              }
            }
            sw.WriteLine(string.Format("        <milestone lvl=\"{0}\" message=\"{1}\" reward=\"{2}\" quantity=\"{3}\" quality=\"{4}\" />", kvp.Key, kvp.Value, itemname, quantity, quality));
          }
        }
        else
        {
          sw.WriteLine(string.Format("       <milestone lvl=\"#\" message=\"[playername] has reached [lvl]!!\" />"));
          sw.WriteLine(string.Format("       <!-- Multiple lines accepted -->"));
          sw.WriteLine(string.Format("       <!-- Use [playername] and [lvl] anywhere in message to be replaced with players info -->"));
        }
        sw.WriteLine("    </Milestones>");
        sw.WriteLine(string.Empty);
        sw.WriteLine("    <ResetRegions>");
        if (BMResetRegions.ManualResetRegions.Count > 0)
        {
          foreach (string _region in BMResetRegions.ManualResetRegions)
          {

            sw.WriteLine(string.Format("        <Region type=\"manual\" region=\"{0}\" />", _region));
          }
        }
        sw.WriteLine("    </ResetRegions>");
        sw.WriteLine(string.Empty);
        sw.WriteLine("    <ZombieAnnouncer>");
        if (EntityWatch.AlertMessage.Count > 0)
        {
          foreach (KeyValuePair<string, string> kvp in EntityWatch.AlertMessage)
          {
            sw.WriteLine(string.Format("        <entity name=\"{0}\" message=\"{1}\"/>", kvp.Key, kvp.Value));
          }
        }
        else
        {
          sw.WriteLine(string.Format("       <entity name=\"zombieName\" message=\"A Boss zombie has spawned at COORDS\" />"));
          sw.WriteLine(string.Format("       <!-- For Multiple Entries copy & paste line and change name/message -->"));
          sw.WriteLine(string.Format("       <!-- Use COORDS if/where you want zombie coordinates displayed -->"));
        }
        sw.WriteLine("    </ZombieAnnouncer>");
        sw.WriteLine(string.Empty);
        sw.WriteLine("    <Zones>");
        if (BMSanctuaries.Sanctuaries.Count > 0)
        {
          foreach (KeyValuePair<string, SanctuaryData> kvp in BMSanctuaries.Sanctuaries)
          {
            SanctuaryData _zd = kvp.Value;
            sw.WriteLine(string.Format("        <zone name=\"{0}\" corner1=\"{1}\" corner2=\"{2}\" />", kvp.Key, _zd.Corner1.ToString(), _zd.Corner2.ToString()));
          }
        }
        else
        {
          sw.WriteLine(string.Format("       <zone name=\"killzone\" corner1=\"0,0,0\" corner2=\"0,0,0\" />"));
        }
        sw.WriteLine("    </Zones>");
        sw.WriteLine(string.Empty);
        sw.WriteLine("    <ExemptPrefabs>");
        if (PrefabReset.ExemptList.Count > 0)
        {
          foreach (string name in PrefabReset.ExemptList)
          {
            sw.WriteLine(string.Format("        <prefab name=\"{0}\"/>", name));
          }
        }
        else
        {
          sw.WriteLine(string.Format("       <prefab name=\"Prefab_Name_Here_01\"/>"));
        }
        sw.WriteLine("    </ExemptPrefabs>");
        sw.WriteLine(string.Empty);
        sw.WriteLine("    <ResetAreas>");
        if (BMAreaReset.ResetAreas.Count > 0)
        {
          foreach (KeyValuePair<string, string> value in BMAreaReset.ResetAreas)
          {
            string[] _params = value.Value.Split(' ');
            int.TryParse(_params[0].Replace("x", ""), out int num);
            int.TryParse(_params[1].Replace("z", ""), out int num2);
            int.TryParse(_params[2].Replace("xx", ""), out int num3);
            int.TryParse(_params[3].Replace("zz", ""), out int num4);
            sw.WriteLine(string.Format("        <resetarea name=\"{0}\" x1=\"{1}\" z1=\"{2}\" x2=\"{3}\" z2=\"{4}\"/>", value.Key, num, num2, num3, num4));
          }
        }
        else
        {
          sw.WriteLine(string.Format("<!--       <resetarea name=\"namehere\" x1=\"0\" z1=\"0\" x2=\"10\" z2=\"10\" /> -->"));
        }
        sw.WriteLine("    </ResetAreas>");


        sw.WriteLine(string.Empty);
        sw.WriteLine("</Botman>");
        sw.Flush();
        sw.Close();
      }
      if (MapUpdateRequired)
      {
        BMMapEditor.Update();
        MapUpdateRequired = false;
      }
      FileWatcher.EnableRaisingEvents = true;
    }
  }
}
