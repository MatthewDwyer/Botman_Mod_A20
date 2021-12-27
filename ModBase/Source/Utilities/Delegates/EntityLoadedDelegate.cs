using Botman.Commands;

namespace Botman
{
  class EntityLoadedDelegate
  {
    public static void OnSpawn(Entity _entity)
    {
      if (EntityWatch.Enabled)
      {
        if (EntityWatch.AlertMessage.ContainsKey(_entity.EntityClass.entityClassName.ToLower()))
        {
          if (EntityWatch.TrackingZombie.Contains(_entity))
          {
            return;
          }
          else
          {
            EntityWatch.AnnounceEntity(_entity);
          }
        }
      }

      BMSanctuaries.DespawnEntityInActiveSanctuaries(_entity);
    }
  }
}
