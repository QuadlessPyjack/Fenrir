namespace Fenrir.IO
{
    public class AssetCopyFactory
    {
        public T GetProvider<T>() where T : AssetCopyProvider
        {
            switch (typeof(T).Name)
            {
                case "TileCopyProvider":
                    return new TileCopyProvider() as T;
                case "EntitySimpleCopyProvider":
                    return new EntitySimpleCopyProvider() as T;
                case "AnimatedEntityCopyProvider":
                    return new AnimatedEntityCopyProvider() as T;
                default:
                    return null;
            }
        }
    }

    public abstract class AssetCopyProvider
    {
        public abstract string Name { get; }
        public abstract IAsset GetAsset<T>(T fileInfo) where T : AssetInfo;
    }

    public class TileCopyProvider : AssetCopyProvider
    {
        public override string Name { get { return "tileCopyProvider"; } }

        public override IAsset GetAsset<T>(T fileInfo)
        {
            if (!fileInfo.Name.Contains("tile"))
            {
                return default;
            }

            TileInfo tileInfo = fileInfo as TileInfo;
            Tile tile = new Tile(tileInfo.TileTexture, tileInfo.Name, tileInfo.Id);

            return tile;
        }
    }

    public class EntitySimpleCopyProvider : AssetCopyProvider
    {
        public override string Name { get { return "entitySimpleCopyProvider"; } }

        public override IAsset GetAsset<T>(T fileInfo)
        {
            if (!fileInfo.Name.Contains("entity"))
            {
                return null;
            }

            SimpleEntityInfo sEntityInfo = fileInfo as SimpleEntityInfo;
            SimpleEntity sEntity = new SimpleEntity(sEntityInfo.Sprite, sEntityInfo.Name, sEntityInfo.Id);
            sEntity.CollisionType = sEntityInfo.CollisionType;
            sEntity.IsFixed = true;
            sEntity.IsSelectable = sEntityInfo.IsSelectable;
            sEntity.IsSelectBlocking = sEntityInfo.IsSelectBlocking;
            sEntity.IsSelected = sEntityInfo.IsSelected;
            sEntity.Z = sEntityInfo.Z;

            return sEntity;
        }
    }

    public class AnimatedEntityCopyProvider : AssetCopyProvider
    {
        public override string Name { get { return "animatedEntityCopyProvider"; } }

        public override IAsset GetAsset<T>(T fileInfo)
        {
            if (!fileInfo.Name.Contains("animated"))
            {
                return null;
            }

            AnimatedEntityInfo aeInfo = fileInfo as AnimatedEntityInfo;
            AnimatedEntity ae = new AnimatedEntity(aeInfo.Texture, aeInfo.Name, aeInfo.Id, aeInfo.FrameSize, aeInfo.FramesPerSecond);
            ae.IsLooping = aeInfo.IsLooping;
            ae.IsPingPong = aeInfo.IsPingPong;
            ae.SetCollision(aeInfo.CollisionType);
            ae.SetSize(aeInfo.Size);

            return ae;
        }
    }
}
