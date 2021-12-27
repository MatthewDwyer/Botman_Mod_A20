using Botman.Commands;
using Botman.Source.BotModEvents;

namespace Botman
{
  class SendMessage
  {
    public static void Public(string message)
    {
      GameManager.Instance.ChatMessageServer(
        _cInfo: null,
        _chatType: EChatType.Global,
        _senderEntityId: -1,
        _msg: $"[{ChatMessage.PublicTextColor}]{message}[-]",
        _mainName: $"{Config.BotName}[-]",
        _localizeMain: false,
        _recipientEntityIds: null);
    }

    public static void Private(ClientInfo cInfo, string message)
    {
      cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(
        _chatType: EChatType.Whisper,
        _senderEntityId: -1,
        _msg: $"[{ChatMessage.PrivateTextColor}]{message}[-]",
        _mainName: $"(PM) {Config.BotName}[-][-]",
        _localizeMain: false,
        _recipientEntityIds: null));
    }
  }
}
