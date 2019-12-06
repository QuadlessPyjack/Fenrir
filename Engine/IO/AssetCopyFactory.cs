using Fenrir;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    public class AssetCopyFactory
    {
        public T GetProvider<T>() where T : AssetCopyProvider
        {
            
        }
    }

    public abstract class AssetCopyProvider
    {
        public abstract string Name { get; }
        public abstract V GetAsset<T,V>(T fileInfo) where T : AssetInfo where V : IAsset;
    }

    public class TileCopyProvider : AssetCopyProvider
    {
        public override string Name { get { return "tileCopyProvider"; } }

        public override V GetAsset<T, V>(T fileInfo) where V : IAsset
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
