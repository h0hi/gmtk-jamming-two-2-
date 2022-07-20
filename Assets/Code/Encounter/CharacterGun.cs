using UnityEngine;

public class CharacterGun : MonoBehaviour
{
    [SerializeField] private float pelletSpawnDistance;
    [SerializeField] private float cooldown = 1;

    [HideInInspector] public GameObject pelletPrefab;
    private float lastShotTime;

    // driven values
    [HideInInspector] public Vector3 shootDirection;

    // driven method
    public bool TryShoot() {
        if (Time.time - lastShotTime > cooldown) {
            lastShotTime = Time.time;
            var pelletObj = Instantiate(pelletPrefab, transform.position + shootDirection * pelletSpawnDistance, Quaternion.FromToRotation(Vector3.forward, shootDirection), null);
            pelletObj.layer = gameObject.layer;
            return true;
        }
        return false;
    }

    private void Update() {
        var angle = Mathf.Atan2(shootDirection.x, shootDirection.z);
        transform.eulerAngles = angle * Mathf.Rad2Deg * Vector3.up;
    }

    private void OnEnable() {
        var stats = GetComponent<CharacterStats>();
        cooldown = 1f / stats.GetSPS();
    }
}
