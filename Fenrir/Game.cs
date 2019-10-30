using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Fenrir
{
    public class Game
    {
        RenderWindow renderWindow;
        public Game()
        {
            renderWindow = new RenderWindow(new VideoMode(80, 600), "Fenrir Test");
            while (true)
            {
                renderWindow.Clear();
                renderWindow.Display();
            }
        }
    }
}
