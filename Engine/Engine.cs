using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Fenrir
{
    public class Engine
    {
        RenderWindow renderWindow;
        private bool shouldQuit = false;

        private TextureFactory textureFactory;
        private AssetFileFactory assetFactory;
        private MapLoader mapLoader;

        private RenderTexture mapSurface;
        private Camera camera;

        private bool viewNeedsUpdate = false;

        private GridMap currentMap;
        private ColliderGenerator colliderGenerator;
        private Cursor cursor;

        private void Initialize()
        {
            textureFactory = new TextureFactory();
            assetFactory = new AssetFileFactory();
            mapLoader = new MapLoader();
            colliderGenerator = new ColliderGenerator();
            colliderGenerator.LoadCollisionData();

            textureFactory.LoadTextures();
            assetFactory.LoadAssets();
            MapInfo mapInfo = mapLoader.Load("m_01");

            currentMap = new GridMap();
            Constants.CurrentMap = currentMap;
            currentMap.GenerateCells(mapInfo);

            uint mapWidth = (uint)mapInfo.Width * Constants.SpriteSize;
            uint mapHeight = (uint)mapInfo.Height * Constants.SpriteSize;
            mapSurface = new RenderTexture(mapWidth, mapHeight);
            currentMap.DrawMap(mapSurface);

            camera = new Camera();
            camera.SetViewport(new Vector2f(0.0f, 0.0f));
            mapSurface.Display();

            SFML.Graphics.Texture t = mapSurface.Texture;
            //SFML.Graphics.Image i = new Image(t.Size.X, t.Size.Y, Color.Magenta);
            //i = t.CopyToImage();

            //i.SaveToFile("./test.jpeg");

            renderWindow.SetView(camera.GetView());

            cursor = new Cursor();
            cursor.Position = new Vector2f(renderWindow.Size.X * 0.5f, renderWindow.Size.Y * 0.5f);
            renderWindow.SetMouseCursorVisible(false);

            cursor.SetState(Cursor.State.ARROW);
        }
        public Engine()
        {
            renderWindow = new RenderWindow(new VideoMode(800, 600), "Fenrir Test");
            renderWindow.KeyPressed += HandleQuitKeyPressed;
            renderWindow.KeyPressed += HandleCameraMoved;
            renderWindow.MouseWheelScrolled += HandleMouseWheelScrolled;
            renderWindow.MouseButtonReleased += HandleMouseButtonReleased;
            renderWindow.Closed += HandleMainWindowClosed;
            renderWindow.MouseMoved += HandleMouseMoved;

            Initialize();
            

            while (!shouldQuit)
            {
                renderWindow.Clear();
                if (viewNeedsUpdate)
                {
                    renderWindow.SetView(camera.GetView());
                    mapSurface.Display();
                    viewNeedsUpdate = false;
                }

                currentMap.UpdateMap(mapSurface);

                Vector2i cameraPosition = (Vector2i)camera.Position / Constants.SpriteSize;
                Vector2i viewEdge = cameraPosition + (Vector2i)(camera.GetView().Size / Constants.SpriteSize);
                currentMap.UpdateEntitiesAt(cameraPosition, viewEdge);
                mapSurface.Display();

                if (currentMap.MapNeedsUpdate)
                {
                    // update current view only
                    currentMap.RedrawMapAt(mapSurface, cameraPosition, viewEdge);
                    mapSurface.Display();
                    currentMap.MapNeedsUpdate = false;
                }

                Sprite renderedMap = new Sprite(mapSurface.Texture);
                //renderedMap.Texture.CopyToImage().SaveToFile("./mapDump" + DateTime.Now.Millisecond + ".jpg");
                renderWindow.Draw(renderedMap);

                cursor.Update();
                renderWindow.Draw(cursor.CurrentIcon);

                renderWindow.Display();

                renderWindow.DispatchEvents();
            }

            renderWindow.Close();
        }

        private void HandleMouseMoved(object sender, MouseMoveEventArgs e)
        {
            cursor.Position = new Vector2f(e.X, e.Y);
            Vector2f screenCoordinates = new Vector2f(e.X, e.Y);

            if (Constants.SelectedEntities.Count == 0 && 
                cursor.CurrentState != Cursor.State.ARROW)
            {
                cursor.SetState(Cursor.State.ARROW);
            }

            Cell hoveredCell = GetCellAtScreenCoordinates(screenCoordinates);
            if (hoveredCell == null) return;

            if (hoveredCell.Tiles.Count == 0)
            {
                return;
            }

            if (cursor.CurrentState == Cursor.State.MOVE || cursor.CurrentState == Cursor.State.DENIED)
            {
                int tileCollider = hoveredCell.Tiles[0].CollisionType;
                //Console.WriteLine("tileCollider: " + tileCollider);

                foreach (var entity in Constants.SelectedEntities)
                {
                    bool isColliding = entity.CollisionType.CheckCollision(tileCollider);

                    if (isColliding)
                    {
                        cursor.SetState(Cursor.State.DENIED);
                        return;
                    }
                }

                cursor.SetState(Cursor.State.MOVE);
            }

            if (hoveredCell.Entities.Count == 0)
            {
                return;
            }

            if (cursor.CurrentState == Cursor.State.ARROW)
            {
                cursor.SetState(Cursor.State.SELECT);
            }
        }

        private void HandleMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Right)
            {
                foreach (Entity selectedEntity in Constants.SelectedEntities)
                {
                    selectedEntity.OnDeselected();
                }
                Constants.SelectedEntities.Clear();
                currentMap.MapNeedsUpdate = true;
                cursor.SetState(Cursor.State.ARROW);
                return;
            }

            Vector2f coords = new Vector2f(e.X, e.Y);
            Cell clickedCell = GetCellAtScreenCoordinates(coords);

            clickedCell.OnClicked();
            cursor.SetState(Cursor.State.MOVE);
        }

        private Cell GetCellAtScreenCoordinates(Vector2f coordinates)
        {
            Vector2f screenCoordinates = coordinates;

            Vector2i worldTextureCoordinates = (Vector2i)(camera.Position + screenCoordinates);
            Vector2i worldGridCoordinates = worldTextureCoordinates / Constants.SpriteSize;

            Cell clickedCell = currentMap.GetCellAt(worldGridCoordinates);
            return clickedCell;
        }

        private void HandleMouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            Console.WriteLine("Scroll delta: " + e.Delta);

            camera.GetView().Zoom(e.Delta * 0.0001f);
            viewNeedsUpdate = true;
        }

        private void HandleCameraMoved(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.A:
                    camera.Translate(new Vector2f(-Constants.CameraSpeed, 0.0f));
                    viewNeedsUpdate = true;
                    return;
                case Keyboard.Key.D:
                    camera.Translate(new Vector2f(Constants.CameraSpeed, 0.0f));
                    viewNeedsUpdate = true;
                    return;
                case Keyboard.Key.S:
                    camera.Translate(new Vector2f(0.0f, Constants.CameraSpeed));
                    viewNeedsUpdate = true;
                    return;
                case Keyboard.Key.W:
                    camera.Translate(new Vector2f(0.0f, -Constants.CameraSpeed));
                    viewNeedsUpdate = true;
                    return;
                case Keyboard.Key.Left:
                    camera.Translate(new Vector2f(-Constants.CameraSpeed, 0.0f));
                    viewNeedsUpdate = true;
                    return;
                case Keyboard.Key.Right:
                    camera.Translate(new Vector2f(Constants.CameraSpeed, 0.0f));
                    viewNeedsUpdate = true;
                    return;
                case Keyboard.Key.Up:
                    camera.Translate(new Vector2f(0.0f, -Constants.CameraSpeed));
                    viewNeedsUpdate = true;
                    return;
                case Keyboard.Key.Down:
                    camera.Translate(new Vector2f(0.0f, Constants.CameraSpeed));
                    viewNeedsUpdate = true;
                    return;
                default:
                    return;
            }
        }

        private void HandleMainWindowClosed(object sender, EventArgs e)
        {
            
        }

        private void HandleQuitKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                shouldQuit = true;
            }
        }
    }
}
