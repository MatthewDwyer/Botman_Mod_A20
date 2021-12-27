using System.Collections.Generic;
using UnityEngine;
using System;


namespace Botman
{
  public class AsyncCommand : IConsoleConnection
  {

    public AsyncCommand() { }

    public void SendLines(List<string> _output)
    {

    }

    public void SendLine(string _text)
    {
      //throw new NotImplementedException ();
    }

    public void SendLog(string _formattedMessage, string _plainMessage, string _trace, LogType _type, DateTime _timestamp, long _uptime)
    {
      //throw new NotImplementedException ();
    }

    public void EnableLogLevel(UnityEngine.LogType _type, bool _enable)
    {
      //throw new NotImplementedException ();
    }

    public string GetDescription()
    {
      return "Fire cmds asynchronously from used thread";
    }
  }
}

// Use e.g.:
// AsyncCommand iConsole = new AsyncCommand();
// SdtdConsole.Instance.ExecuteAsync(cmd, iConsole);

