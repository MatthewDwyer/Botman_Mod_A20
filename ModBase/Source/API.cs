using System;
using System.IO;
using Botman.Source.BotModEvents;
using System.Collections.Generic;

namespace Botman
{
  public class API : IModApi
  {
    private static readonly char Dsc = Path.DirectorySeparatorChar;
    public static string ModsPath = $"{AppDomain.CurrentDomain.BaseDirectory}{Dsc}Mods{Dsc}";
    public static string BotmanPath = $"{ModsPath}Botman";
    public static string MapJsPath = $"{ModsPath}Allocs_WebAndMapRendering{Dsc}webserver{Dsc}js{Dsc}map.js";
    public static readonly Dictionary<Vector3i, long> rlpQueue = new Dictionary<Vector3i, long>(); 

    public void InitMod(Mod mod)
    {
      try
      {
        ModEvents.GameAwake.RegisterHandler(GameAwake.Handler);
        ModEvents.ChatMessage.RegisterHandler(ChatMessage.Handler);
        ModEvents.PlayerSpawnedInWorld.RegisterHandler(PlayerSpawnedInWorld.Handler);
        ModEvents.PlayerDisconnected.RegisterHandler(PlayerDisconnected.Handler);
        ModEvents.GameStartDone.RegisterHandler(GameStartDone.Handler);
        ModEvents.GameShutdown.RegisterHandler(GameShutdown.Handler);
        ModEvents.GameUpdate.RegisterHandler(GameUpdate.Handler);
        ModEvents.SavePlayerData.RegisterHandler(SavePlayerData.Handler);
        ModEvents.GameMessage.RegisterHandler(GameMessage.Handler);
        ModEvents.PlayerLogin.RegisterHandler(PlayerLogin.Handler);
        ModEvents.PlayerSpawning.RegisterHandler(PlayerSpawning.Handler);
      }
      catch (Exception e)
      {
        Log.Out($"~Botman~ Could not load mod reason: {e}");
      }
    }
  }
}
