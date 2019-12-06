using SFML.System;
using SFML.Graphics;
using System.Collections.Generic;
using Fenrir.IO;

namespace Fenrir
{
    public class GridMap
    {
        public int Width = 0;
        public int Height = 0;
        public bool MapNeedsUpdate = false;

        private Cell[,] _cells;

        private AssetCopyFactory _copyFactory = new AssetCopyFactory();

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
                    Tile cachedTile = AssetFileFactory.GetAsset(assetId.ToString()) as Tile;
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

                    Entity instance = AssetFileFactory.GetEntity(entityId.ToString());

                    currentCell.AddEntity(instance);
                }
            }
        }

        public void DrawMap(RenderTexture renderSurface)
        {
            List<Entity> mapEntities = new List<Entity>();
            for (int z = 0; z < (int)Constants.ZLayers.Count; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Cell currentCell = _cells[x, y];

                        foreach (Tile tile in currentCell.Tiles)
                        {
                            if (tile.Z != z)
                            {
                                continue;
                            }

                            renderSurface.Draw(tile);

                            //tile.DumpTextureToFile("./tile_" + x + "_" + y + ".jpg");
                        }

                        if (currentCell.Entities.Count > 0)
                        {
                            mapEntities.AddRange(currentCell.Entities);
                        }
                    }
                }
            }

            mapEntities.Sort(new ZComparer());
            foreach (Entity entity in mapEntities)
            {
                renderSurface.Draw(entity.Sprite);
                foreach (Tile attachedOverlay in entity.AttachedOverlays)
                {
                    renderSurface.Draw(attachedOverlay);
                }
            }
        }

        public void UpdateMap(RenderTexture renderSurface)
        {
            RedrawMapAt(renderSurface, new Vector2i(0, 0), new Vector2i(Width, Height));
        }

        private Vector2i clampCoordsIfInvalid(Vector2i coordinates)
        {
            Vector2i newCoords = coordinates;
            if (coordinates.X < 0)  newCoords.X = 0;
            //if (coordinates.X > Width) newCoords.X = Width;
            if (coordinates.Y < 0) newCoords.Y = 0;
            //if (coordinates.Y > Height) newCoords.Y = Height;

            return newCoords;
        }

        public void UpdateEntitiesAt(Vector2i start, Vector2i end)
        {
            Vector2i startCoords = clampCoordsIfInvalid(start);
            Vector2i endCoords = clampCoordsIfInvalid(end);

            List<Entity> entities = new List<Entity>();

            for (int x = startCoords.X; x < endCoords.X; x++)
            {
                for (int y = startCoords.Y; y < endCoords.Y; y++)
                {
                    entities.AddRange(_cells[x, y].Entities);
                }
            }

            foreach (var entity in entities)
            {
                entity.Update();
            }
        }

        public void RedrawMapAt(RenderTexture renderSurface, Vector2i start, Vector2i end)
        {
            List<Entity> dirtyEntities = new List<Entity>();

            Vector2i startClamped = clampCoordsIfInvalid(start);
            Vector2i endClamped = clampCoordsIfInvalid(end);

            for (int x = startClamped.X; x < endClamped.X; x++)
            {
                for (int y = startClamped.Y; y < endClamped.Y; y++)
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
                if (entity.Size.X > 1 && entity.Size.Y > 1)
                {
                    MarkAdjacentCellsForRedraw(entity.currentCell.GridPosition, entity.Size);
                }
                renderSurface.Draw(entity.Sprite);
                //(entity as IAsset)?.Update();

                foreach (Tile attachedOverlay in entity.AttachedOverlays)
                {
                    renderSurface.Draw(attachedOverlay);
                    MarkAdjacentCellsForRedraw(entity.currentCell.GridPosition, attachedOverlay.Size);
                }
            }
        }

        public void MarkAdjacentCellsForRedraw(Vector2i rootCell, Vector2i size)
        {
                for (int adjacentCellXIdx = 0; adjacentCellXIdx < size.X; adjacentCellXIdx++)
                {
                    for (int adjacentCellYIdx = 0; adjacentCellYIdx < size.Y; adjacentCellYIdx++)
                    {
                        if ((adjacentCellXIdx == 0) && (adjacentCellYIdx == 0))
                        {
                            continue;
                        }
                        Cell adjacentCell = GetCellAt(rootCell.X + adjacentCellXIdx, rootCell.Y + adjacentCellYIdx);
                        if (adjacentCell.IsDirty)
                        {
                            continue;
                        }
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
