using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMRemove : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ despawns specified entity id";

    public override string[] GetCommands() => new[] { "bm-remove" };

    public override string GetHelp() =>
      "Removes a specified entity\n" +
      "Usage:\n" +
      "1. bm-remove <entityid> \n" +
      "   1. despawns specified entity id.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 1)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (!int.TryParse(_params[0], out var entityId))
      {
        SdtdConsole.Instance.Output("Entity id must be an integer.");

        return;
      }

      if (!GameManager.Instance.World.Entities.dict.TryGetValue(entityId, out var entity))
      {
        SdtdConsole.Instance.Output($"Could not find entity with id: {entityId}");

        return;
      }

      if (entity is EntityPlayer)
      {
        SdtdConsole.Instance.Output("Cannot remove a player!");

        return;
      }

      GameManager.Instance.World.RemoveEntity(entityId, EnumRemoveEntityReason.Despawned);

      SdtdConsole.Instance.Output($"Despawned {entity.EntityClass.entityClassName}");
    }
  }
}
