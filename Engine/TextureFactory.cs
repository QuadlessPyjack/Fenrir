using IniParser;
using IniParser.Model;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fenrir
{
    public class TextureFactory
    {
        public void LoadTextures()
        {
            string texturesIniFilePath = "./" + Constants.AssetsRoot + "/textures.ini";
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(texturesIniFilePath);

            string raw = data["Textures"]["Count"];
            int textureCount = int.Parse(raw);
            Constants.LoadedTextures = new Dictionary<string, Texture>(textureCount);

            for (int idx = 0; idx < textureCount; idx++)
            {
                string tSection = idx.ToString();
                string tName = data[tSection]["Name"].Trim('"');
                string tPath = data[tSection]["Path"].Trim('"');

                tPath = Constants.AssetsRoot + '/' + tPath;

                Texture t = new Texture(tName, tPath);
                Constants.LoadedTextures.Add(tName, t);
            }
        }

        public static Texture GetTexture(string name)
        {
            Constants.LoadedTextures.TryGetValue(name, out Texture t);

            Debug.Assert(t != null, "Texture is null. Have you checked your Texture Count?");

            return t;
        }
    }
}
