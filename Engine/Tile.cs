using SFML.Graphics;
using SFML.System;

namespace Fenrir
{
    public class Tile : Sprite, IAsset
    {
        public float Z { get; set; }

        public int CollisionType = -1;
        public Vector2i Size { get; private set; }

        protected string _name;
        protected int _id;

        public Tile(Texture texture, string name = "", int id = -1) : base(texture)
        {
            _name = name;
            _id = id;
        }

        public Tile(Tile tile)
        {
            _name = tile._name;
            _id = tile._id;
            Z = tile.Z;
            Size = tile.Size;
            Texture = tile.Texture;
        }

        public void SetPosition(float x, float y, float z)
        {
            Position = new Vector2f(x, y);
            Z = z;
        }

        public void SetPosition(Vector3f position)
        {
            Position = new Vector2f(position.X, position.Y);
            Z = position.Z;
        }

        public virtual string GetTypeName()
        {
            return "tile";
        }

        public virtual string GetName()
        {
            return _name;
        }

        public virtual void SetSize(Vector2i size)
        {
            Size = size;
        }

        public int GetId()
        {
            return _id;
        }

        public void SetCollision(string collisionType)
        {
            bool isOk = Constants.LoadedColliders.TryGetValue(collisionType, out Collider collider);
            if (!isOk)
            {
                CollisionType = -1;
                return;
            }

            CollisionType = collider.CollisionIndex;
        }

        public void DumpTextureToFile(string fileName)
        {
            SFML.Graphics.Image i = new Image(Texture.Size.X, Texture.Size.Y, Color.Magenta);
            i = Texture.CopyToImage();

            i.SaveToFile(fileName);
        }

        public virtual void Update()
        {
        }
    }
}
