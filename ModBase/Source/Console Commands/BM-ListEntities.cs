using System.Collections.Generic;

namespace Botman.Commands
{
  public class BMListEntities : BMCmdAbstract
  {
    public override string GetDescription() => "~Botman~ List Entities";

    public override string[] GetCommands() => new[] { "bm-listentities", "bm-le" };

    public override string GetHelp() =>
      "Usage:\n" +
      "1. bm-listentities <filter>\n";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (GameManager.Instance.World == null)
      {
        SdtdConsole.Instance.Output("World isn't loaded.");

        return;
      }

      if (_params.Count == 0)
      {
        SdtdConsole.Instance.Output(GetHelp());

        return;
      }

      if (_params.Count > 1)
      {
        SdtdConsole.Instance.Output("Only 1 parameter accepted for filter.");

        return;
      }

      SdtdConsole.Instance.Output(ListEntities(_params[0]));
    }

    public static string ListEntities(string _filter)
    {
      var output = "";
      var entities = GameManager.Instance.World.Entities.dict;
      if (entities.Count > 0)
      {
        output += " { ";
        var x = 1;
        foreach (var entity in entities.Values)
        {
          if (!entity.EntityClass.entityClassName.ToLower().Contains(_filter.ToLower())) { continue; }

          var name = entity.EntityClass.entityClassName;
          output += $"\"{x}\" : {{ \"name\" : \"{name}\" , \"id\" : {entity.entityId} , \"pos\" : \"{entity.GetPosition()}\" }} ,";
          x++;
        }
        output = output.Substring(0, output.Length - 1);
        output += " }  ";
      }
      else
      {
        output = " { \"0\" }";
      }
      return output;
    }
  }
}
