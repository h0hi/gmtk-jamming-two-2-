using UnityEngine;

public class CharacterGun : MonoBehaviour, IEncounterEventListener
{
    [SerializeField] private float pelletSpawnDistance;
    [SerializeField] private float cooldown = 1;
    [SerializeField] private GameObject pelletPrefab;
    private float lastShotTime;

    // driven method
    public bool TryShoot() {
        if (Time.time - lastShotTime > cooldown && enabled) {
            lastShotTime = Time.time;
            var pelletObj = Instantiate(pelletPrefab, transform.position + transform.forward * pelletSpawnDistance, Quaternion.FromToRotation(Vector3.forward, transform.forward), null);
            pelletObj.layer = gameObject.layer;
            pelletObj.transform.GetChild(0).gameObject.layer = gameObject.layer;
            return true;
        }
        return false;
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
