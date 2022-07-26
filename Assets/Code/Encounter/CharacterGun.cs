using UnityEngine;

public class CharacterGun : MonoBehaviour, IEncounterEventListener
{
    [SerializeField] private float pelletSpawnDistance;
    [SerializeField] private float cooldown = 1;

    [SerializeField] private GameObject pelletPrefab;
    private float lastShotTime;

    // driven values
    [HideInInspector] public Vector3 shootDirection;

    // driven method
    public bool TryShoot() {
        if (Time.time - lastShotTime > cooldown && enabled) {
            lastShotTime = Time.time;
            var pelletObj = Instantiate(pelletPrefab, transform.position + shootDirection * pelletSpawnDistance, Quaternion.FromToRotation(Vector3.forward, shootDirection), null);
            pelletObj.layer = gameObject.layer;
            pelletObj.transform.GetChild(0).gameObject.layer = gameObject.layer;
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

    public void OnEncounterEvent(EncounterEventType eventType)
    {
        switch (eventType) {
            case EncounterEventType.Load:
                enabled = false;
                break;
            case EncounterEventType.Begin:
                enabled = true;
                break;
        }
    }
}
