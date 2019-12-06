using SFML.System;
using System;
using System.Collections.Generic;

namespace Fenrir
{

    internal class ZComparer : IComparer<Entity>
    {
        public int Compare(Entity x, Entity y)
        {
            return x.Z.CompareTo(y.Z);
        }
    }
    public class Cell
    {
        public Vector2i GridPosition;
        public bool IsDirty { get { return _isDirty;  }
                              set {
                                    _isDirty = value;
                                    Constants.CurrentMap.MapNeedsUpdate = value;
                                  }
                            }
        private bool _isDirty = false;
        private Tile _baseTile;
        public List<Tile> Tiles { get; private set; }
        public List<Entity> Entities { get; private set; } 

        public Cell()
        {
            Tiles = new List<Tile>();
            Entities = new List<Entity>(64);
        }

        public bool OnClicked()
        {
            foreach (var entity in Entities)
            {
                bool shouldBlock = entity.OnClicked();
                if (shouldBlock)
                {
                    return false;
                }
            }
            return false;
        }

        // Sets this cell's base tile
        // If already set, replaces existing one
        public void SetTile(Tile tile)
        {
            _baseTile = tile;
            _baseTile.Z = 0;

            if (Tiles.Count == 0)
            {
                Tiles.Add(tile);
                updateTilePosition(tile);
                return;
            }

            updateTilePosition(tile);
            Tiles[0] = tile;
        }

        public void AddEntity(Entity entity)
        {
            if (Entities.Count >= entity.Z && Entities[entity.Z] == null)
            {
                Entities.Insert(entity.Z, entity);
            } else
            {
                Entities.Add(entity);
                if (Entities.Count > 0)
                {
                    Entities.Sort(new ZComparer());
                }
            }

            entity.currentCell = this;
            updateEntityPosition(entity);
        }

        public void AddTile(Tile tile, int z = 0)
        {
            int lastTileIdx = Tiles.Count - 1;
            updateTilePosition(tile);

            if (z > (lastTileIdx - 1))
            {
                Tiles.Insert(lastTileIdx - 1, tile);
                return;
            }

            Tiles.Insert(z, tile);
        }

        private void updateTilePosition(Tile tile)
        {
            tile.Position = updatePosition();
            //Console.WriteLine("Tile position: " + tile.Position);
        }

        private void updateEntityPosition(Entity entity)
        {
            entity.Position = updatePosition();
            entity.Sprite.Position = updatePosition();
            //Console.WriteLine("Entity position: " + entity.Position);
        }

        private Vector2f updatePosition()
        {
            Vector2f position = (Vector2f)(GridPosition * Constants.SpriteSize);
            return position;
        }
    }
}