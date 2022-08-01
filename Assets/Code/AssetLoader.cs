using System.IO;
using UnityEngine;

public static class AssetLoader
{
    private const string Path2 = "AssetBundles";
    public static string AssetBundlePath => Path.Join(Application.dataPath, Path2);
}
