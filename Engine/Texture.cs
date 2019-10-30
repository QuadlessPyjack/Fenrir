using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;

namespace Fenrir
{
    public class Texture : SFML.Graphics.Texture
    {
        string Name { get; set; }
        string TexturePath;

        public Texture(string name = "", string texturePath = "")
            : base(texturePath)
        {
            Name = name;
            TexturePath = texturePath;
        }

    }
}
