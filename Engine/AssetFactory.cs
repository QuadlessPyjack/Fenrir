using IniParser;
using IniParser.Model;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fenrir
{
    public class AssetFactory
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

        private IAsset spawnObjectByString(string typeName, KeyDataCollection data)
        {
            switch (typeName)
            {
                case "tile":
                    return new TileProvider(data).GetAsset();
                case "entity":
                    return new EntityProvider(data).GetAsset();
                default:
                    return null;
            }
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
            _ = int.TryParse(_iniSection["Size"], out int size);

            Texture cachedTexture = TextureFactory.GetTexture(textureName);

            Entity entity = new Entity(cachedTexture, assetName, id);
            entity.IsSelectable = isSelectable;
            entity.SetSize(size);

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
            _ = int.TryParse(_iniSection["Size"], out int size);

            Texture cachedTexture = TextureFactory.GetTexture(textureName);

            Tile tile = new Tile(cachedTexture, assetName, id);
            tile.SetSize(size);

            return tile;
        }
    }
}