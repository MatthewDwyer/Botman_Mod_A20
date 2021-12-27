using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Botman.Commands
{
  class BM_ReadConfigV2 : BMCmdAbstract
  {
    public override string[] GetCommands() => new[] { "bm-readconfigv2" };

    public override string GetDescription() => "Echo limited Config.xml to console";

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-readconfigv2\n" +
      "   1. It reads the botman mod config xml (limited) file.";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count != 0)
      {
        SdtdConsole.Instance.Output("Command does not take any arguments.");

        return;
      }

      var xmlDoc = new XmlDocument();

      xmlDoc.Load(Config.ConfigFilePath);

      XmlNode xmlNode = xmlDoc.DocumentElement;

      if (xmlNode == null)
      {
        SdtdConsole.Instance.Output("Unable to load config file.");

        return;
      }

      foreach (XmlNode childNode in xmlNode.ChildNodes)
      {
        if (!childNode.Name.Equals("Configs"))
        {
          continue;
        }

        SdtdConsole.Instance.Output("\n");
        foreach (XmlNode subChild in childNode.ChildNodes)
        {
          if (subChild.NodeType == XmlNodeType.Comment)
          {
            continue;
          }

          if (subChild.NodeType != XmlNodeType.Element)
          {
            continue;
          }

          using (var sw = new StringWriter())
          {
            using (var xw = new XmlTextWriter(sw))
            {
              subChild.WriteTo(xw);
            }

            SdtdConsole.Instance.Output(sw.ToString());
          }
        }
        SdtdConsole.Instance.Output("\n");
      }
    }
  }
}
