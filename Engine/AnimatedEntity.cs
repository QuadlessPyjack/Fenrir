using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace Fenrir
{
    public class AnimatedEntity : Entity
    {
        public bool IsLooping = false;
        public bool IsPingPong = false;

        public AnimatedEntity()
        {
            Sprite = new AnimatedSprite();
        }

        public AnimatedEntity(Texture texture, string name, int id, IntRect frameSize, int framesPerSecond)
        {
            AnimatedSprite animsprite = new AnimatedSprite(texture, frameSize);
            animsprite.FramesPerSecond = framesPerSecond;
            Name = name;
            _id = id;
            Sprite = animsprite;
        }

        public AnimatedEntity(AnimatedEntity ae)
            : base(ae)
        {
            AnimatedSprite animsprite = new AnimatedSprite(ae.Sprite as AnimatedSprite);
            Name = ae.Name;
            _id = ae._id;
            Sprite = animsprite;
        }

        public void PlayAnimation()
        {
            (Sprite as AnimatedSprite).Play();
        }

        public void PauseAnimation()
        {
            (Sprite as AnimatedSprite).Pause();
        }

        public override void Update()
        {
            updateProperties();
            (Sprite as AnimatedSprite).Run();
            currentCell.IsDirty = true;
        }

        private void updateProperties()
        {
            AnimatedSprite animsprite = (Sprite as AnimatedSprite);
            if (animsprite.IsLooping != IsLooping)
            {
                animsprite.IsLooping = IsLooping;
            }

            if (animsprite.IsPingPong != IsPingPong)
            {
                animsprite.IsPingPong = IsPingPong;
            }
        }

        public override int GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Name;
        }

        public override string GetTypeName()
        {
            return "entity_animated";
        }

        public override object Clone()
        {
            AnimatedEntity ae = new AnimatedEntity();
            AnimatedSprite animsprite = ((Sprite as AnimatedSprite).Clone() as AnimatedSprite);
            ae.Name = Name;
            ae._id = _id;
            ae.Sprite = animsprite;

            return ae;
        }
    }
}
