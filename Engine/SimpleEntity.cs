using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace Fenrir
{
    public class SimpleEntity : Entity, IAsset
    {
        public SimpleEntity(SimpleEntity entity) : base(entity)
        {
            IsSelectable = entity.IsSelectable;
            IsSelectBlocking = entity.IsSelectBlocking;
            Sprite.Position = entity.Position;
            AttachedOverlays = new List<Tile>();
        }

        public SimpleEntity(Texture texture, string name = "Item", int id = -1) 
        {
            Sprite = new Sprite(texture);
            Sprite.Position = Position;
            _id = id;
            Name = name;
        }

        public override int GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            throw new NotImplementedException();
        }

        public override string GetTypeName()
        {
            return "item";
        }

        public override void Update()
        {
            // nothing to do here
        }
    }
}
