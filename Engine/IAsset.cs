﻿namespace Fenrir
{
    public interface IAsset
    {
        string GetTypeName();
        string GetName();

        int GetId();
    }
}