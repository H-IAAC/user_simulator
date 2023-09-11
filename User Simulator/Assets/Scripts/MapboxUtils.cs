using UnityEngine;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;

public static class MapboxUtils
{
    
    public static Vector3 gpsToUnity(AbstractMap map, float lat, float lon)
    {
        var tileIDUnwrapped = TileCover.CoordinateToTileId(new Mapbox.Utils.Vector2d(lat, lon), (int)map.Zoom);

        //get tile
        UnityTile tile = map.MapVisualizer.GetUnityTileFromUnwrappedTileId(tileIDUnwrapped);

        //lat lon to meters because the tiles rect is also in meters
        Vector2d v2d = Conversions.LatLonToMeters(new Mapbox.Utils.Vector2d(lat, lon));
        //get the origin of the tile in meters
        Vector2d v2dcenter = tile.Rect.Center - new Mapbox.Utils.Vector2d(tile.Rect.Size.x / 2, tile.Rect.Size.y / 2);
        //offset between the tile origin and the lat lon point
        Vector2d diff = v2d - v2dcenter;

        //maping the diffetences to (0-1)
        float Dx = (float)(diff.x / tile.Rect.Size.x);
        float Dy = (float)(diff.y / tile.Rect.Size.y);

        //height in unity units
        var h = tile.QueryHeightData(Dx,Dy );

        h += map.transform.position.y;

        //lat lon to unity units
        Vector3 location = Conversions.GeoToWorldPosition(lat, lon, map.CenterMercator, map.WorldRelativeScale).ToVector3xz();
        //replace y in position
        location = new Vector3(location.x, h, location.z);

        return location;
    } 
}