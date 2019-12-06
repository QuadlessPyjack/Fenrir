using SFML.Graphics;
using SFML.System;

namespace Fenrir.IO
{
    public class AnimatedEntityInfo : AssetInfo
    {
        public Fenrir.Texture Texture;
        public IntRect FrameSize;
        public int FramesPerSecond;
        public AnimatedSprite SpriteAnimated;
        public bool IsLooping;
        public bool IsPingPong;
        public string CollisionType;
        public Vector2i Size;
    }
}
