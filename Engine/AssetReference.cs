using SFML.System;

namespace Fenrir
{
    // Dummy asset used to cover multi-cell assets (for example - collision purposes)
    // Acts as a reference to the "root" cell of the multi-cell asset it belongs to
    public class AssetReference : Entity
    {
        private Cell _assetRootCell;
        public override int GetId()
        {
            return Constants.AssetReferenceId;
        }

        public override string GetName()
        {
            return "Asset Reference";
        }

        public override string GetTypeName()
        {
            return "reference";
        }

        public Vector2i GetAssetRootPosition()
        {
            return _assetRootCell.GridPosition;
        }

        public Cell GetAssetRootCell()
        {
            return _assetRootCell;
        }

        public override void SetSize(Vector2i _)
        {
            // N/A
        }

        public override void Update()
        {
        }
    }
}
