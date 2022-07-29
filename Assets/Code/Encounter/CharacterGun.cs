using UnityEngine;

public class CharacterGun : MonoBehaviour, IEncounterEventListener
{
    [SerializeField] private float pelletSpawnDistance;
    [SerializeField] private float cooldown = 1;
    [SerializeField] private float topAngularSpeed;
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
        var targetRadians = Mathf.Atan2(shootDirection.x, shootDirection.z);
        var currentRadians = Mathf.Atan2(transform.forward.x, transform.forward.z);
        var deltaDegrees = Mathf.MoveTowardsAngle(currentRadians * Mathf.Rad2Deg, targetRadians * Mathf.Rad2Deg, topAngularSpeed * Time.deltaTime);
        transform.eulerAngles = deltaDegrees * Vector3.up;
    }

    public void OnEncounterEvent(EncounterEventType eventType)
    {
        switch (eventType) {
            case EncounterEventType.Load:
                enabled = false;
                break;
            case EncounterEventType.Begin:
                enabled = true;
                var stats = GetComponent<CharacterStats>();
                cooldown = 1f / stats.GetSPS();
                break;
        }
    }
}
