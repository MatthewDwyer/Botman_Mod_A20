using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Botman
{
  [Serializable]
  public class PersistentContainer
  {
    private static readonly char Dsc = Path.DirectorySeparatorChar;
    public static string Filepath = $"{API.BotmanPath}{Dsc}Botman.bin";
    private Players players;
    private int undoSize;
    private int serverScore;
    private DateTime lastReset;
    private DateTime lastAreaReset;
    private DateTime lastPrefabReset;
    private DateTime serverScoreCurrentWeek;
    private bool hideChatCommands;
    private string hideChatCommandsPrefix;

    private static PersistentContainer instance;

    public Players Players => players ?? (players = new Players());

    public static PersistentContainer Instance => instance ?? (instance = new PersistentContainer());

    private PersistentContainer() { }

    public void Save()
    {
      Stream stream = File.Open(Filepath, FileMode.Create);
      var binaryFormatter = new BinaryFormatter();
      binaryFormatter.Serialize(stream, this);
      stream.Close();
    }

    public static bool Load()
    {
      if (!File.Exists(Filepath))
      {
        return false;
      }
      try
      {
        Stream stream = File.Open(Filepath, FileMode.Open);
        var binaryFormatter = new BinaryFormatter();
        var persistentContainer = (PersistentContainer)binaryFormatter.Deserialize(stream);
        stream.Close();
        instance = persistentContainer;

        return true;
      }
      catch (Exception ex)
      {
        Log.Error("Exception in PersistentContainer.Load");
        Log.Exception(ex);
      }

      return false;
    }

    public int UndoSize
    {
      get
      {
        if (undoSize <= 0)
        {
          undoSize = 1;
        }
        return undoSize;
      }

      set => undoSize = value;
    }

    public bool HideChatCommands
    {
      get => hideChatCommands;
      set => hideChatCommands = value;
    }

    public string HideChatCommandPrefix
    {
      get => hideChatCommandsPrefix;
      set => hideChatCommandsPrefix = value;
    }

    public DateTime LastReset
    {
      get => lastReset;
      set => lastReset = value;
    }

    public DateTime LastPrefabReset
    {
      get => lastPrefabReset;
      set => lastPrefabReset = value;
    }

    public DateTime LastAreaReset
    {
      get => lastAreaReset;
      set => lastAreaReset = value;
    }

    public DateTime ServerScoreCurrentWeek
    {
      get => serverScoreCurrentWeek;
      set => serverScoreCurrentWeek = value;
    }

    public int ServerScore
    {
      get => serverScore;
      set => serverScore = value;
    }
  }
}
