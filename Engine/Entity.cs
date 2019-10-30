using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace Fenrir
{
    public class Entity : Tile
    {
        public bool IsSelectable = false;
        public bool IsSelectBlocking = false;
        public bool IsSelected = false;

        public Cell currentCell = null;

        public List<Tile> AttachedOverlays;
        private int _overlaySize;

        public Entity(Entity entity) : base(entity)
        {
            IsSelectable = entity.IsSelectable;
            IsSelectBlocking = entity.IsSelectBlocking;

            AttachedOverlays = new List<Tile>();
        }

        public Entity(Texture texture, string name = "Item", int id = -1) 
                     : base(texture, name, id)
        {
            
        }

        public override string GetTypeName()
        {
            return "item";
        }

        public virtual bool OnClicked()
        {
            if (!IsSelectable)
            {
                return false;
            }

            Tile cachedTile = AssetFactory.GetAsset("3") as Tile;
            Tile selectedOverlay = new Tile(cachedTile);
            selectedOverlay.Position = Position;
            _overlaySize = selectedOverlay.Size;

            if (_overlaySize != Size)
            {
                float scale = (float)Size / selectedOverlay.Size;
                selectedOverlay.Scale = new Vector2f(scale, scale);
            }

            AttachedOverlays.Add(selectedOverlay);

            Console.WriteLine("Clicked: " + _name);
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
    }
}
