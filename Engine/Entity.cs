using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace Fenrir
{
    public class Entity : Tile
    {
        public bool IsSelectable = false;
        public bool IsSelectBlocking = false;
        public bool IsSelected = false;

        public List<Tile> AttachedOverlays;

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

            AttachedOverlays.Add(selectedOverlay);

            Console.WriteLine("Clicked: " + _name);
            Constants.SelectedEntities.Add(this);

            IsSelected = true;

            return IsSelectBlocking;
        }

        public virtual void OnDeselected()
        {
            if (!IsSelected)
            {
                return;
            }

            AttachedOverlays.Clear();
            IsSelected = false;
        }
    }
}
