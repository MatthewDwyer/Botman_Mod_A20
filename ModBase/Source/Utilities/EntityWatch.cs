using System;
using System.Collections.Generic;
using Botman.Commands;

namespace Botman
{
  class EntityWatch
  {
    public static bool Enabled = false;
    public static int TimeBetweenEntityCheck = 5;
    public static List<Entity> TrackingZombie = new List<Entity>();
    public static Dictionary<string, string> AlertMessage = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

    public static int _tick;

    public static void Update()
    {
      if (Enabled)
      {
        _tick++;
        if (_tick >= TimeBetweenEntityCheck)
        {
          Run();
          _tick = 0;
        }
      }
      else
      {
        _tick = 0;
      }
    }

    public static void Run()
    {
      if (!Enabled) { return; }

      foreach (var entity in GameManager.Instance.World.Entities.list)
      {

        if (!AlertMessage.ContainsKey(entity.EntityClass.entityClassName.ToLower())) { continue; }

        if (!TrackingZombie.Contains(entity))
        {
          AnnounceEntity(entity);
        }
      }
    }

    public static void AnnounceEntity(Entity entity)
    {
      AlertMessage.TryGetValue(entity.EntityClass.entityClassName.ToLower(), out var message);

      if (null == message) { return; }

      if (message.Contains("COORDS"))
      {
        var pos = entity.GetBlockPosition();
        message = message.Replace("COORDS", pos.x + "," + pos.y + "," + pos.z);
      }

      SendMessage.Public(message);

      if (!TrackingZombie.Contains(entity))
      {
        TrackingZombie.Add(entity);
      }
    }
  }
}
