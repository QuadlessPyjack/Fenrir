using SFML.System;

namespace Fenrir.IO
{
    public class TileInfo : AssetInfo
    {
        public int Z;
        public Vector2i Size;
        public Texture TileTexture;
        public int CollisionType;
    }
}
