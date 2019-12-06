using Newtonsoft.Json;
using System.IO;

namespace Fenrir
{
    public class MapInfo
    {
        public string Name;
        public string Version;
        public string Description;
        public int[] Tiles;
        public int[] Entities;

        public int Width;
        public int Height;
    }
    public class MapLoader
    {
        //private MapInfo currentMap;
        public MapInfo Load(string mapName)
        {
            string mapFilePath = "./" + Constants.DataRoot + "/" + Constants.MapsRoot + "/" + mapName + ".json";
            MapInfo map = new MapInfo();
            map.Name = "INVALID";

            using (StreamReader file = File.OpenText(mapFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                map = (MapInfo)serializer.Deserialize(file, typeof(MapInfo));
            }

            return map;
        }
    }
}
