using Fenrir;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace Engine.IO
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
        public Fenrir.Texture Sprite;
        public Vector2i Position;
        public int Z;
        public Collider CollisionType;
    }
}
