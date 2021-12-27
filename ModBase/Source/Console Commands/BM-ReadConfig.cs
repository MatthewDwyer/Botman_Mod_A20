using Botman.Commands;
using System.Collections.Generic;
using System.IO;

namespace Botman
{
  class BM_ReadConfig : BMCmdAbstract
  {
    public override string[] GetCommands() => new[] { "bm-readconfig" };

    public override string GetDescription() => "Echo Config.xml to console";

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-readconfig\n" +
      "   1. It reads the botman mod config xml file.\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 0)
      {
        SdtdConsole.Instance.Output("Command does not take any arguments.");

        return;
      }

      var reader = new StreamReader(Config.ConfigFilePath);
      string line;

      SdtdConsole.Instance.Output("\n");
      while ((line = reader.ReadLine()) != null)
      {
        SdtdConsole.Instance.Output(line);
      }

      SdtdConsole.Instance.Output("\n");

      reader.Close();
    }
  }
}
