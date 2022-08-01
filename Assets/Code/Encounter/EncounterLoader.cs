using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EncounterLoader : MonoBehaviour
{
    public static EncounterLoader instance; 
    [HideInInspector] public GameObject loadedEncounter;

    private void Awake() {
        if (instance != null) {
            Destroy(this);
        }
        instance = this;
    }

    public void LoadEncounter(string environmentName, EnemyWaves waveAsset) {
        var loadedEnvironmentAssetName = $"Assets/Prefabs/Encounters/{environmentName}.prefab";
        var prefab = AssetLoader.LoadAsset<GameObject>(loadedEnvironmentAssetName);
        loadedEncounter = Instantiate(prefab, transform);
        loadedEncounter.GetComponent<EncounterAsset>().Load(waveAsset);
        AssetLoader.UnloadAsset(prefab);
    }

    public void UnloadEncounter() {
        Destroy(loadedEncounter);

        var pellets = FindObjectsOfType<PelletBehaviour>();

        foreach (var pellet in pellets) {
            if (pellet != null)
                Destroy(pellet.gameObject);
        }
    }

    public GameObject GetEncounterPrefab(string name) => AssetLoader.LoadAsset<GameObject>($"Assets/Prefabs/EncPrefabs/{name}.prefab");
}
