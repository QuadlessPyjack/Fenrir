using SFML.System;

namespace Fenrir
{
    public interface IAsset
    {
        string GetTypeName();
        string GetName();
        int GetId();
        void SetSize(Vector2i size);

        void SetCollision(string collisionType);

        void Update();
    }
}