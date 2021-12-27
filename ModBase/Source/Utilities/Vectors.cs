using System.Collections.Generic;

namespace Botman
{
  class Vectors
  {
    public static bool TryParse(string vectorString, out Vector3i vector3i)
    {
      vector3i = Vector3i.zero;

      if (string.IsNullOrWhiteSpace(vectorString))
      {
        return false;
      }

      var strArray = vectorString.Split(',');

      if (strArray.Length != 3) { return false; }

      if (!int.TryParse(strArray[0], out var x) ||
          !int.TryParse(strArray[1], out var y) ||
          !int.TryParse(strArray[2], out var z))
      {
        return false;
      }

      vector3i = new Vector3i(x, y, z);

      return true;
    }

    public static List<ClientInfo> WithinZone = new List<ClientInfo>();

    public static bool PrefabCornerIsWithinRegion(PrefabInstance prefab, string region)
    {
      //var x = prefab.boundingBoxPosition.x;
      //var y = prefab.boundingBoxPosition.y;
      //var z = prefab.boundingBoxPosition.z;
      ////Southwest Corner Check
      //if (GetRegion(new Vector3i(x, y, z)) == region)
      //{
      //  return true;
      //}

      ////Northwest Corner Check
      //if (GetRegion(PrefabBoundingBoxPlusSize(prefab)) == region)
      //{
      //  return true;
      //}
      ////SouthEast Corner Check
      //var x2 = x + prefab.boundingBoxSize.x;
      //if (GetRegion(new Vector3i(x2, y, z)) == region)
      //{
      //  return true;
      //}

      ////NorthWest Corner Check
      //var z2 = z + prefab.boundingBoxSize.z;
      //if (GetRegion(new Vector3i(x, y, z2)) == region)
      //{
      //  return true;
      //}

      if (GetRegion(PrefabCenter(prefab)) != region)
      {
        return false;
      }

      SdtdConsole.Instance.Output("true");
      return true;
    }

    public static Vector3i PrefabCenter(PrefabInstance prefab)
    {
      var x = prefab.boundingBoxPosition.x + (prefab.boundingBoxSize.x / 2);
      var y = prefab.boundingBoxPosition.y + (prefab.boundingBoxSize.y / 2);
      var z = prefab.boundingBoxPosition.z + (prefab.boundingBoxSize.z / 2);

      return new Vector3i(x, y, z);
    }

    public static Vector3i PrefabBoundingBoxPlusSize(PrefabInstance prefab)
    {
      var x = prefab.boundingBoxPosition.x + prefab.boundingBoxSize.x;
      var y = prefab.boundingBoxPosition.y + prefab.boundingBoxSize.y;
      var z = prefab.boundingBoxPosition.z + prefab.boundingBoxSize.z;

      return new Vector3i(x, y, z);
    }

    public static bool IsWithinRestrictedPrefab(PrefabInstance prefab, ClientInfo cInfo)
    {
      var player = GameManager.Instance.World.Players.dict[cInfo.entityId];
      var southWest = prefab.boundingBoxPosition;
      var northEast = PrefabBoundingBoxPlusSize(prefab);
      var swX = southWest.x - 25;
      var swZ = southWest.z - 25;
      var neX = northEast.x + 25;
      var neZ = northEast.z + 25;
      var pos = player.GetBlockPosition();
      var x = pos.x;
      var z = pos.z;

      return x > swX && x < neX && z > swZ && z < neZ;
    }

    public static bool WithinPrefabBoundaries(Vector3i pos, Vector3i location, int range)
    {
      var posX = pos.x;
      var posZ = pos.z;

      var prefabX = location.x;
      var prefabZ = location.z;

      return prefabX - posX <= range &&
             prefabX - posX >= -range &&
             prefabZ - posZ <= range &&
             prefabZ - posZ >= -range;
    }

    public static bool PlayerInZone(Vector3i playerPos, Vector3i southWestCorner, Vector3i northEastCorner) =>
      playerPos.x > southWestCorner.x &&
      playerPos.x < northEastCorner.x &&
      playerPos.z > southWestCorner.z &&
      playerPos.z < northEastCorner.z &&
      playerPos.y > southWestCorner.y &&
      playerPos.y < northEastCorner.y;

    public static string GetRegion(Vector3i position) => 
      $"r.{Utils.Fastfloor(position.x / 512f)}.{Utils.Fastfloor(position.z / 512f)}";

    public static string GetPrefabRegion(Vector3i position)
    {
      var region = RegionFile.ChunkXzToRegionXz(new Vector2i(Utils.Fastfloor(position.x / 15f), Utils.Fastfloor(position.z / 15f)));
      return $"r.{region.x}.{region.y}";
    }
  }
}
