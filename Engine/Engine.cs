using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fenrir;
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
        private AssetFactory assetFactory;
        private MapLoader mapLoader;

        private RenderTexture mapSurface;
        private Camera camera;

        private bool viewNeedsUpdate = false;

        private GridMap currentMap;

        private void Initialize()
        {
            textureFactory = new TextureFactory();
            assetFactory = new AssetFactory();
            mapLoader = new MapLoader();

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
        }
        public Engine()
        {
            renderWindow = new RenderWindow(new VideoMode(800, 600), "Fenrir Test");
            renderWindow.KeyPressed += HandleQuitKeyPressed;
            renderWindow.KeyPressed += HandleCameraMoved;
            renderWindow.MouseWheelScrolled += HandleMouseWheelScrolled;
            renderWindow.MouseButtonReleased += HandleMouseButtonReleased;
            renderWindow.Closed += HandleMainWindowClosed;

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

                renderWindow.Display();

                renderWindow.DispatchEvents();
            }

            renderWindow.Close();
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
                return;
            }

            Vector2i screenCoordinates = new Vector2i(e.X, e.Y);
            Console.WriteLine("Clicked at: " + screenCoordinates);

            Vector2i worldTextureCoordinates = (Vector2i)camera.Position + screenCoordinates;
            Vector2i worldGridCoordinates = worldTextureCoordinates / Constants.SpriteSize;

            Console.WriteLine(worldGridCoordinates);
            //Console.WriteLine("WorldTextureCoords: " + worldTextureCoordinates);
            //Console.WriteLine("WorldGridCoords: " + worldGridCoordinates);

            Cell clickedCell = currentMap.GetCellAt(worldGridCoordinates);
            clickedCell.OnClicked();
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
