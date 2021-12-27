using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMSetUndoSize : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ Set the size of history on pundo";

    public override string[] GetCommands() => new[] { "bm-setpundosize", "bm-setundosize" };

    public override string GetHelp() =>
      "Set the size of history on pundo\n" +
      "Usage:\n" +
      "1. bm-setpundosize <size> \n" +
      "2. bm-setpundosize\n" +
      "   1. Sets the Pundo History Size\n" +
      "   2. Gets the Pundo History Size\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output($"PUndo history size is {PersistentContainer.Instance.UndoSize}");

        return;
      }

      if (!int.TryParse(_params[0], out var size) || size <= 0)
      {
        SdtdConsole.Instance.Output("Invalid PUndo history size. It must be greater than 0.");

        return;
      }

      PersistentContainer.Instance.UndoSize = size;

      SdtdConsole.Instance.Output($"PUndo history size set to {size}");
    }
  }
}
