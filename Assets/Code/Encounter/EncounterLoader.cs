using System.IO;
using UnityEngine;

public class EncounterLoader : MonoBehaviour
{
    public static EncounterLoader instance; 
    private AssetBundle encounterBundle;
    private AssetBundle encounterAssetBundle;
    private string encounterAssetBundlesFolder;
    [HideInInspector] public GameObject loadedEncounter;

    private void Awake() {
        if (instance != null) {
            Destroy(this);
        }
        instance = this;
    }

    private void Start() {
        encounterAssetBundlesFolder = Path.Join(Application.dataPath, "AssetBundles", "encounters");
        encounterAssetBundle = AssetBundle.LoadFromFile(Path.Join(Application.dataPath, "AssetBundles", "encounterassets"));
    }

    public void LoadEncounter(string environmentName, EnemyWaves waveAsset) {
        encounterBundle = AssetBundle.LoadFromFile(Path.Join(encounterAssetBundlesFolder, environmentName));

        var prefab = encounterBundle.LoadAsset<GameObject>(environmentName);
        if (prefab == null) {
            prefab = encounterBundle.LoadAsset<GameObject>(environmentName + " Variant");
        }
        if (prefab == null) {
            Debug.LogError("Could not find object " + environmentName + " in bundle! Make sure the encounter parent has the bundle\'s name");
        }
        loadedEncounter = Instantiate(prefab, transform);
        loadedEncounter.GetComponent<EncounterAsset>().Load(waveAsset);
    }

    public void UnloadEncounter() {
        encounterBundle.Unload(true);
        Destroy(loadedEncounter);

        var pellets = FindObjectsOfType<PelletBehaviour>();

        foreach (var pellet in pellets) {
            if (pellet != null)
                Destroy(pellet.gameObject);
        }
    }

    public GameObject GetEncounterPrefab(string name) => encounterAssetBundle.LoadAsset<GameObject>(name);
}
