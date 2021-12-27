using System.Collections.Generic;
using System.Threading;

namespace Botman.Commands
{
  public class BMPUndo : BMCmdAbstract
  {
    private static readonly Dictionary<string, List<PrefabUndoObj>> UndoPrefabs = new Dictionary<string, List<PrefabUndoObj>>();

    public override string GetDescription() => "Undo last prefab command";

    public override string[] GetCommands() => new[] { "bm-pundo", "bm-undo" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-pundo\n" +
      "   1. Undoes prefab commands. It works with bm-prender, bm-pblock and bm-pdup\n" +
      "* By default, the size of undo history is set to 1. You can change the undo history size using \"bm-setpundosize\"\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      var cInfo = _senderInfo.RemoteClientInfo;
      var entityId = cInfo == null ? "server_" : $"{cInfo.entityId}";

      var undoObj = GetUndoPrefab(entityId);

      if (undoObj == null)
      {
        SdtdConsole.Instance.Output("Unable to undo the last prefab command");

        return;
      }

      var chunks = new Dictionary<long, Chunk>();
      for (var j = 0; j < undoObj.Prefab.size.x; j++)
      {
        for (var k = 0; k < undoObj.Prefab.size.z; k++)
        {
          for (var m = 0; m < undoObj.Prefab.size.y; m++)
          {
            if (GameManager.Instance.World.IsChunkAreaLoaded(undoObj.Position.x + j, undoObj.Position.y + m, undoObj.Position.z + k) &&
                GameManager.Instance.World.GetChunkFromWorldPos(undoObj.Position.x + j, undoObj.Position.y + m, undoObj.Position.z + k) is Chunk chunk)
            {
              if (!chunks.ContainsKey(chunk.Key))
              {
                chunks.Add(chunk.Key, chunk);
              }
            }
            else
            {
              SdtdConsole.Instance.Output("The render prefab is far away. Chunk not loaded on that area");

              return;
            }
          }
        }
      }

      var pyState = GameManager.bPhysicsActive;

      GameManager.bPhysicsActive = false;

      undoObj.Prefab.CopyIntoLocal(GameManager.Instance.World.ChunkCache, undoObj.Position, true, true);

      GameManager.bPhysicsActive = pyState;

      Thread.Sleep(50);

      BMReload.ReloadForClients(chunks);

      var stabCalc = new StabilityCalculator();
      stabCalc.Init(GameManager.Instance.World);

      for (var j = 0; j < undoObj.Prefab.size.x; j++)
      {
        for (var k = 0; k < undoObj.Prefab.size.z; k++)
        {
          for (var m = 0; m < undoObj.Prefab.size.y; m++)
          {
            var block = GameManager.Instance.World.GetBlock(undoObj.Position.x + j, undoObj.Position.y + m, undoObj.Position.z + k);
            if (block.type == BlockValue.Air.type) { continue; }

            var _position = new Vector3i(undoObj.Position.x + j, undoObj.Position.y + m, undoObj.Position.z + k);
            stabCalc.BlockPlacedAt(_position, false);
          }
        }
      }

      stabCalc.Cleanup();

      SdtdConsole.Instance.Output($"Prefab Undone loaded at {undoObj.Position.x} {undoObj.Position.y} {undoObj.Position.z}");
    }

    public static PrefabUndoObj GetUndoPrefab(string entityId)
    {
      if (!UndoPrefabs.TryGetValue(entityId, out var list))
      {
        return null;
      }

      if (list == null || list.Count == 0)
      {
        return null;
      }

      var obj = list[0];
      list.RemoveAt(0);

      return obj;
    }

    public static void SetUndo(string entityId, Prefab prefab, Vector3i location)
    {
      if (!UndoPrefabs.TryGetValue(entityId, out var list))
      {
        list = new List<PrefabUndoObj>();
        UndoPrefabs.Add(entityId, list);
      }

      if (list.Count >= PersistentContainer.Instance.UndoSize)
      {
        list.RemoveAt(list.Count - 1);
      }
      var obj = new PrefabUndoObj(prefab, location);
      list.Insert(0, obj);

      UndoPrefabs.Remove(entityId);
      UndoPrefabs.Add(entityId, list);
    }
  }

  public class PrefabUndoObj
  {
    public Prefab Prefab { get; set; }
    public Vector3i Position { get; set; }

    public PrefabUndoObj(Prefab prefab, Vector3i position)
    {
      this.Prefab = prefab;
      this.Position = position;
    }
  }
}
