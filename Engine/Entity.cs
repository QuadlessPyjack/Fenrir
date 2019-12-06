using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace Fenrir
{
    public class Entity : IAsset
    {
        public string Name { get; set; }
        public bool IsSelectable = false;
        public bool IsSelectBlocking = false;
        public bool IsSelected = false;

        public bool IsFixed = true;
        public Vector2f Position;
        public int Z { get { return _z; } set { _z = value; } }
        private int _z = 1;

        public Vector2i Size { get; private set; }

        public Cell currentCell = null;

        public List<Tile> AttachedOverlays;

        public Sprite Sprite;

        protected int _id;
        protected Vector2i _overlaySize;

        public Collider CollisionType;

        public Entity()
        {
            _id = -1;
            Name = "";
            Size = new Vector2i();
            _overlaySize = new Vector2i();
            AttachedOverlays = new List<Tile>();
            Sprite = new Sprite();
            Sprite.Position = Position;
        }

        public Entity(Entity e)
        {
            _id = e._id;
            Name = e.Name;
            Size = e.Size;
            _overlaySize = e._overlaySize;
            AttachedOverlays = e.AttachedOverlays;
            IsSelectable = e.IsSelectable;
            IsSelectBlocking = e.IsSelectBlocking;
            IsSelected = e.IsSelected;
            Sprite = e.Sprite;
            Position = e.Position;
            Z = e._z;
            CollisionType = e.CollisionType;
        }

        public Entity(Texture texture, string name, int id)
        {
            Name = name;
            _id = id;
            Sprite = new Sprite(texture);
            Sprite.Position = Position;
            Size = new Vector2i();
            _overlaySize = new Vector2i();
            AttachedOverlays = new List<Tile>();
        }

        public virtual void SetSize(Vector2i size)
        {
            Size = size;
        }

        public virtual bool OnClicked()
        {
            if (!IsSelectable)
            {
                return false;
            }

            Tile cachedTile = AssetFileFactory.GetAsset("3") as Tile;
            Tile selectedOverlay = new Tile(cachedTile);
            selectedOverlay.Position = Position;
            _overlaySize = selectedOverlay.Size;

            if (_overlaySize != Size)
            {
                float scaleX = (float)Size.X / selectedOverlay.Size.X;
                float scaleY = (float)Size.Y / selectedOverlay.Size.Y;
                selectedOverlay.Scale = new Vector2f(scaleX, scaleY);
            }

            AttachedOverlays.Add(selectedOverlay);

            Console.WriteLine("Clicked: " + Name);
            Constants.SelectedEntities.Add(this);

            IsSelected = true;

            // we're displaying an overlay at the current cell position
            // so mark for redraw
            currentCell.IsDirty = true;

            return IsSelectBlocking;
        }

        public virtual void OnDeselected()
        {
            if (!IsSelected)
            {
                return;
            }

            AttachedOverlays.Clear();
            currentCell.IsDirty = true;
            Constants.CurrentMap.MarkAdjacentCellsForRedraw(currentCell.GridPosition, _overlaySize);
            IsSelected = false;
        }

        public virtual string GetTypeName()
        {
            throw new NotImplementedException();
        }

        public virtual string GetName()
        {
            throw new NotImplementedException();
        }

        public virtual int GetId()
        {
            throw new NotImplementedException();
        }

        public virtual void Update()
        {
            //Console.WriteLine("Entity update called!");
        }

        public void SetCollision(string collisionType)
        {
            bool isOk = Constants.LoadedColliders.TryGetValue(collisionType, out Collider collider);

            if (!isOk)
            {
                CollisionType = new Collider();
                return;
            }

            CollisionType = collider;
        }

        private bool isAccessible(Vector2i gridLocation)
        {
            Cell locationCell = Constants.CurrentMap.GetCellAt(gridLocation);

            int tileCollider = locationCell.Tiles[0].CollisionType;

            bool isTileAccessible = CollisionType.CheckCollision(tileCollider);

            if (!isTileAccessible)
            {
                return false;
            }

            // check if there are entities here blocking access

            foreach (var entity in locationCell.Entities)
            {
                bool isEntityNotBlocking = CollisionType.CheckCollision(entity.CollisionType);
                if (!isEntityNotBlocking)
                {
                    return false;
                }
            }

            return true;
        }
    }
}