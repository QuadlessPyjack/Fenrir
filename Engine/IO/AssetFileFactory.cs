using Engine.IO;
using IniParser;
using IniParser.Model;
using SFML.Graphics;
using System.Diagnostics;

namespace Fenrir
{
    public class AssetFileFactory
    {
        public void LoadAssets()
        {
            string assetsIniFilePath = "./" + Constants.AssetsRoot + "/assets.ini";
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(assetsIniFilePath);

            int assetCount = int.Parse(data["Assets"]["Count"]);

            for (int idx = 0; idx < assetCount; idx++)
            {
                string aSection = idx.ToString();
                string aId = data[aSection]["Id"].Trim('"');
                string aType = data[aSection]["Type"].Trim('"');

                IAsset a = spawnObjectByString(aType, data[aSection]);
                Constants.LoadedAssets.Add(aId, a);
            }
        }

        public static IAsset GetAsset(string id)
        {
            Constants.LoadedAssets.TryGetValue(id, out IAsset asset);
            Debug.Assert(asset != null, "Asset is null. Have you checked your Asset Count?");

            return asset;
        }

        public static Entity GetEntity(string id)
        {
            Constants.LoadedAssets.TryGetValue(id, out IAsset asset);
            Debug.Assert(asset != null, "Asset is null. Have you checked your Asset Count?");
            string typeName = asset.GetTypeName();

            switch (typeName)
            {
                case "entity":
                    {
                        Entity e = new Entity(asset as Entity);
                        return e;
                    }
                case "entity_animated":
                    {
                        return asset as AnimatedEntity;
                    }
                default:
                    return null;
            }
        }

        private IAsset spawnObjectByString(string typeName, KeyDataCollection data)
        {
            switch (typeName)
            {
                case "tile":
                    return new TileProvider(data).GetAsset();
                case "entity":
                    return new EntityProvider(data).GetAsset();
                case "entity_animated":
                    return new AnimatedEntityProvider(data).GetAsset();
                default:
                    return null;
            }
        }
    }

    public class AnimatedEntityProvider : AssetProvider
    {
        public AnimatedEntityProvider(KeyDataCollection section) : base(section)
        {
        }

        public override string Name { get { return "entity"; } }

        public override IAsset GetAsset()
        {
            string textureName = _iniSection["Texture"].Trim('"');
            string assetName = _iniSection["Name"].Trim('"');
            _ = bool.TryParse(_iniSection["IsSelectable"], out bool isSelectable);
            _ = bool.TryParse(_iniSection["IsLooping"], out bool isLooping);
            _ = int.TryParse(_iniSection["Id"], out int id);
            _ = int.TryParse(_iniSection["FramesPerSecond"], out int fps);
            bool isZSpecified = int.TryParse(_iniSection["Z"], out int z);
            string size = _iniSection["Size"];
            string collisionType = _iniSection["CollisionType"];
            int width = 0;
            int height = 0;

            if (size.Contains("x"))
            {
                string[] values = size.Split('x');
                _ = int.TryParse(values[0], out width);
                _ = int.TryParse(values[1], out height);
            }
            else
            {
                _ = int.TryParse(size, out width);
                height = width;
            }

            Texture cachedTexture = TextureFactory.GetTexture(textureName);
            IntRect frameSize = new IntRect(0, 0, width * Constants.SpriteSize, height * Constants.SpriteSize);
            AnimatedEntity entity = new AnimatedEntity(cachedTexture, assetName, id, frameSize, fps);
            entity.IsSelectable = isSelectable;
            entity.IsLooping = isLooping;
            entity.SetSize(new SFML.System.Vector2i(width, height));
            entity.SetCollision(collisionType);

            if (isZSpecified)
            {
                entity.Z = z;
            }

            return entity;
        }
    }

    public class EntityProvider : AssetProvider
    {
        public EntityProvider(KeyDataCollection section) : base(section)
        {
        }

        public override string Name { get { return "entity"; } }

        public override IAsset GetAsset()
        {
            string textureName = _iniSection["Texture"].Trim('"');
            string assetName = _iniSection["Name"].Trim('"');
            _ = bool.TryParse(_iniSection["IsSelectable"], out bool isSelectable);
            _ = int.TryParse(_iniSection["Id"], out int id);
            bool isZSpecified = int.TryParse(_iniSection["Z"], out int z);
            string size = _iniSection["Size"];
            string collisionType = _iniSection["CollisionType"];
            int width = 0;
            int height = 0;

            if (size.Contains("x"))
            {
                string[] values = size.Split('x');
                _ = int.TryParse(values[0], out width);
                _ = int.TryParse(values[1], out height);
            } else
            {
                _ = int.TryParse(size, out width);
                height = width;
            }

            Texture cachedTexture = TextureFactory.GetTexture(textureName);

            SimpleEntity entity = new SimpleEntity(cachedTexture, assetName, id);
            entity.IsSelectable = isSelectable;
            entity.SetSize(new SFML.System.Vector2i(width, height));
            entity.SetCollision(collisionType);

            if (isZSpecified)
            {
                entity.Z = z;
            }

            return entity;
        }
    }

    public abstract class AssetProvider
    {
        public abstract string Name { get; }
        protected KeyDataCollection _iniSection;
        public abstract IAsset GetAsset();

        public AssetProvider(KeyDataCollection section)
        {
            _iniSection = section;
        }
    }

    public class TileProvider : AssetProvider
    {
        public TileProvider(KeyDataCollection section) : base(section)
        {
        }

        public override string Name { get { return "tile"; } }

        public override IAsset GetAsset()
        {
            string textureName = _iniSection["Texture"].Trim('"');
            string assetName = _iniSection["Name"].Trim('"');
            _ = int.TryParse(_iniSection["Id"], out int id);
            bool isZSpecified = int.TryParse(_iniSection["Z"], out int z);
            string size = _iniSection["Size"];
            string collisionType = _iniSection["CollisionType"];
            int width = 0;
            int height = 0;

            if (size.Contains("x"))
            {
                string[] values = size.Split('x');
                _ = int.TryParse(values[0], out width);
                _ = int.TryParse(values[1], out height);
            }
            else
            {
                _ = int.TryParse(size, out width);
                height = width;
            }

            Texture cachedTexture = TextureFactory.GetTexture(textureName);

            Tile tile = new Tile(cachedTexture, assetName, id);
            tile.SetSize(new SFML.System.Vector2i(width, height));
            tile.SetCollision(collisionType);

            if (isZSpecified)
            {
                tile.Z = z;
            }

            return tile;
        }
    }
}