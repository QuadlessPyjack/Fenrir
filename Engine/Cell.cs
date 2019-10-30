using SFML.System;
using System;
using System.Collections.Generic;

namespace Fenrir
{
    public class Cell
    {
        public Vector2i GridPosition;
        public bool IsDirty = false;
        private Tile _baseTile;
        public List<Tile> Tiles { get; private set; }
        public List<Entity> Entities { get; private set; } 

        public Cell()
        {
            Tiles = new List<Tile>();
            Entities = new List<Entity>();
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
            Entities.Add(entity);
            updateTilePosition(entity);
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
            tile.Position = (Vector2f)(GridPosition * Constants.SpriteSize);
            Console.WriteLine("Tile position: " + tile.Position);
        }
    }
}