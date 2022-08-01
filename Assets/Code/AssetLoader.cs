using UnityEngine;
using UnityEngine.AddressableAssets;

public static class AssetLoader
{
    public static TObject LoadAsset<TObject> (string address) where TObject : Object {
        var handle = Addressables.LoadAssetAsync<TObject>(address);
        return handle.WaitForCompletion();
    }

    public static void UnloadAsset<TObject> (TObject genericObject) {
        Addressables.Release(genericObject);
    }
}
