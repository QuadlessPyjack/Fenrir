using SFML.System;
using SFML.Graphics;
using System.Collections.Generic;

namespace Fenrir
{
    public class GridMap
    {
        public int Width = 0;
        public int Height = 0;

        private Cell[,] _cells;

        public GridMap()
        {

        }

        public GridMap(MapInfo mapData)
        {
            Width = mapData.Width;
            Height = mapData.Height;

            if (Width == 0 || Height == 0)
            {
                _cells = new Cell[1, 1];
                return;
            }
            _cells = new Cell[Width, Height];
        }

        public void GenerateCells(MapInfo mapInfo)
        {
            Width = mapInfo.Width;
            Height = mapInfo.Height;

            _cells = new Cell[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int currentIdx = y * Width + x;
                    int assetId = mapInfo.Tiles[currentIdx];
                    Tile cachedTile = AssetFactory.GetAsset(assetId.ToString()) as Tile;
                    Tile instancedTile = new Tile(cachedTile);
                    Cell currentCell = new Cell();
                    currentCell.GridPosition = new Vector2i(x, y);
                    currentCell.SetTile(instancedTile);
                    _cells[x, y] = currentCell;

                    int entityId = mapInfo.Entities[currentIdx];
                    if (entityId == -1)
                    {
                        continue;
                    }

                    Entity cachedEntity = AssetFactory.GetAsset(entityId.ToString()) as Entity;
                    Entity instance = new Entity(cachedEntity);

                    currentCell.AddEntity(instance);
                }
            }
        }

        public void DrawMap(RenderTexture renderSurface)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Cell currentCell = _cells[x, y];

                    foreach (Tile tile in currentCell.Tiles)
                    {
                        renderSurface.Draw(tile);

                        //tile.DumpTextureToFile("./tile_" + x + "_" + y + ".jpg");
                    }

                    foreach (Entity entity in currentCell.Entities)
                    {
                        renderSurface.Draw(entity);
                        foreach (Tile attachedOverlay in entity.AttachedOverlays)
                        {
                            renderSurface.Draw(attachedOverlay);
                        }
                    }
                }
            }
        }

        public void UpdateMap(RenderTexture renderSurface)
        {
            RedrawMapAt(renderSurface, new Vector2i(0, 0), new Vector2i(Width, Height));
        }

        public void RedrawMapAt(RenderTexture renderSurface, Vector2i start, Vector2i end)
        {
            List<Entity> dirtyEntities = new List<Entity>();

            for (int x = start.X; x < end.X; x++)
            {
                for (int y = start.Y; y < end.Y; y++)
                {
                    Cell currentCell = _cells[x, y];

                    if (!currentCell.IsDirty)
                    {
                        continue;
                    }

                    foreach (Tile tile in currentCell.Tiles)
                    {
                        renderSurface.Draw(tile);
                        //if (tile.Size > 1)
                        {
                            //markAdjacentCellsForRedraw(currentCell.GridPosition, tile.Size);
                        }
                        //tile.DumpTextureToFile("./tile_" + x + "_" + y + ".jpg");
                    }

                    dirtyEntities.AddRange(currentCell.Entities);
                    currentCell.IsDirty = false;
                }
            }

            foreach (Entity entity in dirtyEntities)
            {
                renderSurface.Draw(entity);
                foreach (Tile attachedOverlay in entity.AttachedOverlays)
                {
                    renderSurface.Draw(attachedOverlay);
                    //if (attachedOverlay.Size > 1)
                    {
                        //markAdjacentCellsForRedraw(entity.currentCell.GridPosition, attachedOverlay.Size);
                    }
                }
            }
        }

        public void MarkAdjacentCellsForRedraw(Vector2i rootCell, int size)
        {
                for (int adjacentCellXIdx = 0; adjacentCellXIdx < size; adjacentCellXIdx++)
                {
                    for (int adjacentCellYIdx = 0; adjacentCellYIdx < size; adjacentCellYIdx++)
                    {
                        if ((adjacentCellXIdx == 0) && (adjacentCellYIdx == 0))
                        {
                            continue;
                        }
                        Cell adjacentCell = GetCellAt(rootCell.X + adjacentCellXIdx, rootCell.Y + adjacentCellYIdx);
                        adjacentCell.IsDirty = true;
                    }
                }
        }

        public Cell GetCellAt(int x, int y)
        {
            return _cells[x, y];
        }

        public Cell GetCellAt(Vector2i location)
        {
            return _cells[location.X, location.Y];
        }
    }
}
