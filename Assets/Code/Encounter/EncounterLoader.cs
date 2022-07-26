using System.IO;
using UnityEngine;

public class EncounterLoader : MonoBehaviour
{
    [SerializeField] private string[] encounterBundleNames;
    private System.Random rnd;
    private AssetBundle encounterBundle;
    [HideInInspector] public GameObject loadedEncounter;

    private void Start() {
        rnd = new System.Random(System.DateTime.Now.GetHashCode());
        AssetBundle.LoadFromFile(Path.Join(Application.dataPath, "AssetBundles", "encounterassets"));
    }

    public void LoadEncounter() {
        var name = encounterBundleNames[rnd.Next(encounterBundleNames.Length)];
        var encounterAssetBundlesFolder = Path.Join(Application.dataPath, "AssetBundles", "encounters");
        encounterBundle = AssetBundle.LoadFromFile(Path.Join(encounterAssetBundlesFolder, name));

        loadedEncounter = Instantiate(encounterBundle.LoadAsset<GameObject>(name), transform);
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
