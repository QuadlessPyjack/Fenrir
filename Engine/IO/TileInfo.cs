using SFML.Graphics;
using SFML.System;

namespace Engine.IO
{
    public class TileInfo : AssetInfo
    {
        public int Z;
        public Vector2i Size;
        public Fenrir.Texture TileTexture;
        public int CollisionType;
    }
}
