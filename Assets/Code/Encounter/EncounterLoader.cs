using System.IO;
using UnityEngine;

public class EncounterLoader : MonoBehaviour
{
    [SerializeField] private string[] encounterBundleNames;
    private System.Random rnd;
    private AssetBundle encounterBundle;
    [HideInInspector] public GameObject loadedEncounter;

    private void Start() {
        AssetBundle.LoadFromFile(Path.Join(Application.dataPath, "AssetBundles", "encounterassets"));
    }

    public void LoadEncounter() {
        rnd = new System.Random(System.DateTime.Now.GetHashCode());
        var name = encounterBundleNames[rnd.Next(encounterBundleNames.Length)];
        var encounterAssetBundlesFolder = Path.Join(Application.dataPath, "AssetBundles", "encounters");
        encounterBundle = AssetBundle.LoadFromFile(Path.Join(encounterAssetBundlesFolder, name));

        var prefab = encounterBundle.LoadAsset<GameObject>(name);
        if (prefab == null) {
            prefab = encounterBundle.LoadAsset<GameObject>(name + " Variant");
        }
        if (prefab == null) {
            Debug.LogError("Could not find encounter object in bundle! Make sure the encounter parent has the bundle\'s name");
        }
        loadedEncounter = Instantiate(prefab, transform);
        loadedEncounter.GetComponent<EncounterAsset>().Load();
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
}
