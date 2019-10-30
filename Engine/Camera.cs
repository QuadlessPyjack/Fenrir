using System;
using SFML.Graphics;
using SFML.System;

namespace Fenrir
{
    public class Camera
    {
        public Vector2f Position { get; private set; }
        //private Vector2f ViewportSize;
        private View _view;

        public Camera()
        {
            //ViewportSize = new Vector2f(Constants.MainViewportWidth, Constants.MainViewportHeight);
            _view = new View();
            SetPosition(Constants.CameraStart);
        }

        public View GetView()
        {
            return _view;
        }

        public void SetPosition(Vector2f position)
        {
            Position = new Vector2f(position.X, position.Y);
            _view.Reset(new FloatRect(position.X, position.Y, Constants.MainViewportWidth, Constants.MainViewportHeight));
        }

        public void SetViewport(Vector2f position)
        {
            _view.Viewport = new FloatRect(position.X, position.Y, 1.0f, 1.0f);
        }

        public void Translate(Vector2f direction)
        {
            Position += direction;
            _view.Move(direction);
        }
    }
}