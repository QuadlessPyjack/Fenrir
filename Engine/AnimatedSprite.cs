using SFML.Graphics;
using SFML.System;
using System;

namespace Fenrir
{
    public class AnimatedSprite : Sprite, ICloneable
    {
        public IntRect FrameRect = new IntRect(0, 0, Constants.SpriteSize, Constants.SpriteSize);
        private float _frameDuration = 1.0f / 30.0f;
        public int FramesPerSecond { get { return _framesPerSecond; } set { _framesPerSecond = value;  _frameDuration = 1.0f / _framesPerSecond; } }
        private int _framesPerSecond = 30;
        public bool IsLooping = false;
        public bool IsPingPong = false;
        private IntRect initialFrame;
        private Clock _animationClock;
        private bool isRunning = false;
        private int _direction = 1;

        public AnimatedSprite()
        {
            _animationClock = new Clock();
            initialFrame = FrameRect;

            updateTexture();
        }

        public AnimatedSprite(AnimatedSprite ae)
        {
            FrameRect = ae.FrameRect;
            _frameDuration = ae._frameDuration;
            FramesPerSecond = ae.FramesPerSecond;
            IsLooping = ae.IsLooping;
            IsPingPong = ae.IsPingPong;
            initialFrame = ae.initialFrame;
            isRunning = ae.isRunning;
            _direction = ae._direction;
        }

        public AnimatedSprite(SFML.Graphics.Texture texture, IntRect frameSize) : base(texture)
        {
            FrameRect = frameSize;
            initialFrame = FrameRect;
            _animationClock = new Clock();

            updateTexture();
        }

        public AnimatedSprite(SFML.Graphics.Texture texture, IntRect rectangle, IntRect frameSize) : base(texture, rectangle)
        {
            FrameRect = frameSize;
            initialFrame = FrameRect;
            _animationClock = new Clock();

            updateTexture();
        }

        public void Play()
        {
            isRunning = true;
        }

        public void Pause()
        {
            isRunning = false;
        }

        public void Reset()
        {
            FrameRect = initialFrame;
        }

        public void Run()
        {
            if (!isRunning && !IsLooping)
            {
                return;
            }

            if (_animationClock.ElapsedTime.AsSeconds() > _frameDuration)
            {
                updateTexture();
            }

            if (FrameRect.Left >= (this.Texture.Size.X - FrameRect.Width))
            {
                // we are at the last frame
                if (IsLooping)
                {
                    FrameRect.Left = 0;
                    _direction = 1;
                    return;
                }

                if (IsPingPong)
                {
                    _direction = -1;
                    return;
                }
            }

            FrameRect.Left += _direction * FrameRect.Width;
        }

        private void updateTexture()
        {
            TextureRect = FrameRect;
            _animationClock.Restart();
        }

        public object Clone()
        {
            AnimatedSprite animSprite = new AnimatedSprite();
            animSprite.FrameRect = FrameRect;
            animSprite._frameDuration = _frameDuration;
            animSprite.FramesPerSecond = FramesPerSecond;
            animSprite.IsLooping = IsLooping;
            animSprite.IsPingPong = IsPingPong;
            animSprite.initialFrame = initialFrame;
            animSprite.isRunning = isRunning;
            animSprite._direction = _direction;

            return animSprite;
        }
    }
}
