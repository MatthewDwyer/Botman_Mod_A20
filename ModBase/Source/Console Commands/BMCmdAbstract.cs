using System;
using System.Collections.Generic;
using System.Reflection;

namespace Botman.Commands
{
  public abstract class BMCmdAbstract : ConsoleCmdAbstract
  {
    public abstract void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo);

    public sealed override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
      try
      {
        TryExecute(_params, _senderInfo);
      }
      catch (Exception e)
      {
        Log.Out($"~Botman Notice~ Error in {GetType().Name}.{MethodBase.GetCurrentMethod().Name}: {e}");
      }
    }
  }
}
