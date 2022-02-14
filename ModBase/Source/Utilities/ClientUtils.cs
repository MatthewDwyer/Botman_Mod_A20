using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Botman
{
    class ClientCollection
    {
        public static ClientInfo GetClientInfoFromNameOrId(string _id)
        {
            return SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.GetForNameOrId(_id);
        }

        public static ClientInfo GetClientInfoFromEntityId(int _entityId)
        {
            return SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(_entityId);
        }

        public static ClientInfo GetClientInfoFromUId(PlatformUserIdentifierAbs _uId)
        {
            return SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForUserId(_uId);
        }

        public static ClientInfo GetClientInfoFromName(string _name)
        {
            return SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.GetForPlayerName(_name);
        }
    }
}
