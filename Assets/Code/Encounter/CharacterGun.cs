using UnityEngine;

public class CharacterGun : MonoBehaviour
{
    [SerializeField] private float pelletSpawnDistance;
    [SerializeField] private float cooldown = 1;

    private GameObject pelletPrefab;
    private float lastShotTime;

    // driven values
    [HideInInspector] public Vector3 shootDirection;

    // driven method
    public void TryShoot() {
        if (Time.time - lastShotTime > cooldown) {
            lastShotTime = Time.time;
            var pelletObj = Instantiate(pelletPrefab, transform.position + shootDirection * pelletSpawnDistance, Quaternion.FromToRotation(Vector3.forward, shootDirection), null);
            pelletObj.layer = gameObject.layer;
        }
    }

    private void Start() {
        pelletPrefab = Resources.Load<GameObject>("Encounter/pellet");
    }

    private void Update() {
        var angle = Mathf.Atan2(shootDirection.x, shootDirection.z);
        transform.eulerAngles = angle * Mathf.Rad2Deg * Vector3.up;
    }

    private void OnEnable() {
        var stats = GetComponent<CharacterStats>();
        cooldown = 1f / stats.GetSPS();
    }

    private void OnDisable() {
        // cleanup pellets
        var pellets = FindObjectsOfType<PelletBehaviour>();

        foreach (var pellet in pellets) {
            if (pellet != null)
                Destroy(pellet.gameObject);
        }
    }
}
