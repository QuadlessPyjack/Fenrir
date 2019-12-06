using SFML.System;
using System.Collections.Generic;

namespace Fenrir.IO
{
    public class SimpleEntityInfo : AssetInfo
    {
        public bool IsSelectable;
        public bool IsSelectBlocking;
        public Vector2i SpritePosition;
        public List<TileInfo> AttachedOverlays;

        public Vector2i Size;
        public Vector2i OverlaySize;
        public bool IsSelected;
        public Texture Sprite;
        public Vector2i Position;
        public int Z;
        public Collider CollisionType;
    }
}
