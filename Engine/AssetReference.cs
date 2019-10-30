using SFML.System;

namespace Fenrir
{
    // Dummy asset used to cover multi-cell assets (for example - collision purposes)
    // Acts as a reference to the "root" cell of the multi-cell asset it belongs to
    public class AssetReference : IAsset
    {
        public Vector2i Position { get; set; }

        private Cell _assetRootCell;
        public int GetId()
        {
            return Constants.AssetReferenceId;
        }

        public string GetName()
        {
            return "Asset Reference";
        }

        public string GetTypeName()
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

        public void SetSize(int _)
        {
            // N/A
        }
    }
}
