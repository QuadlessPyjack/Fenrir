using SFML.Graphics;
using SFML.System;

namespace Fenrir
{
    public class Cursor
    {
        public Sprite[] StateIcons;
        public Sprite CurrentIcon;
        public State CurrentState = State.ARROW;
        public Vector2f Position { get { return _position; }  set { CurrentIcon.Position = value; _position = value; } }
        private Vector2f _position;

        private Vector2f _offset = new Vector2f(-Constants.SpriteSize * 0.5f, -Constants.SpriteSize * 0.5f);
        public enum State
        {
            ARROW = 0,
            SELECT = 1,
            MOVE = 2,
            DENIED = 3,
            ATTACK = 4,
            DEPLOY = 5,
            SIZE = 6
        };

        public Cursor()
        {
            StateIcons = new Sprite[(int)State.SIZE];
            LoadIcons();

            CurrentIcon = StateIcons[(int)State.ARROW];
        }

        public void Update()
        {
            if (CurrentState == State.SELECT)
            {
                (CurrentIcon as AnimatedSprite).Run();
            }
        }

        private void LoadIcons()
        {
            SFML.Graphics.Texture arrowTexture = new SFML.Graphics.Texture(Constants.TexturesRoot + "/cursor" + "/arrow.tga");
            SFML.Graphics.Texture forbiddenTexture = new SFML.Graphics.Texture(Constants.TexturesRoot + "/cursor" + "/forbidden.tga");
            SFML.Graphics.Texture crosshairTexture = new SFML.Graphics.Texture(Constants.TexturesRoot + "/cursor" + "/crosshair.tga");
            SFML.Graphics.Texture selectAnimTexture = new SFML.Graphics.Texture(Constants.TexturesRoot + "/cursor" + "/select_a.tga");

            Sprite arrow = new Sprite(arrowTexture);
            Sprite forbidden = new Sprite(forbiddenTexture);
            Sprite crosshair = new Sprite(crosshairTexture);
            AnimatedSprite select = new AnimatedSprite(selectAnimTexture, new IntRect(0,0,32,32));
            select.IsLooping = true;
            select.FramesPerSecond = 3;
            select.Play();

            StateIcons[(int)State.ARROW] = arrow;
            StateIcons[(int)State.DENIED] = forbidden;
            StateIcons[(int)State.SELECT] = select;
            StateIcons[(int)State.MOVE] = select;

            for (int iconIdx = 0; iconIdx < (int)State.SIZE; iconIdx++)
            {
                if (StateIcons[iconIdx] == null)
                {
                    continue;
                }

                StateIcons[iconIdx].Position = Position;
            }
        }

        public void SetState(State newState)
        {
            CurrentState = newState;
            CurrentIcon = StateIcons[(int)newState];
            CurrentIcon.Position = Position + ((newState != State.ARROW) ? _offset : new Vector2f(0.0f, 0.0f));
        }
    }
}
