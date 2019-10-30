using SFML.System;
using System.Collections.Generic;

namespace Fenrir
{
    public static class Constants
    {
        public static string DataRoot     = "/Data";
        public static string AssetsRoot   = "Data/Assets";
        public static string TexturesRoot = "Data/Assets/textures";
        public static string MapsRoot     = "/Maps";
        public static Vector2f CameraStart = new Vector2f(0.0f, 0.0f);
        public static float MainViewportWidth = 800.0f;
        public static float MainViewportHeight = 600.0f;
        public const int SpriteSize = 32;
        public const int AssetReferenceId = -2; // this is a built-in type, we don't want it altered in the ini files

        public static Dictionary<string, Texture> LoadedTextures = new Dictionary<string, Texture>();
        public static Dictionary<string, IAsset> LoadedAssets = new Dictionary<string, IAsset>();

        public static List<Entity> SelectedEntities = new List<Entity>();
        public static GridMap CurrentMap;

        public static float CameraSpeed = 3.0f;
    }
}
