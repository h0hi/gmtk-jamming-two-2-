using UnityEngine;

[RequireComponent(typeof(CharacterGun))]
public class TurretCharacterDriver : MonoBehaviour
{
    private CharacterGun[] gunDevices;
    private Transform target;

    int shotNumber;
    private float lastGunSwitchTime;
    [SerializeField] private float gunSwitchCooldown;
    
    private void Start() {
        gunDevices = GetComponents<CharacterGun>();

        gunDevices[0].pelletPrefab = Resources.Load<GameObject>("Encounter/Destructible Pellet");
        gunDevices[1].pelletPrefab = Resources.Load<GameObject>("Encounter/Indestructible Pellet");
        
        target = GameObject.FindWithTag("Player").transform;
        shotNumber = 0;
    }

    private void Update() {
        var direction = (target.position - transform.position).normalized;
        direction.y = 0;
        
        var activeGun = gunDevices[shotNumber % 2];
        activeGun.shootDirection = direction;
        if (Time.time - lastGunSwitchTime > gunSwitchCooldown && activeGun.TryShoot()) {
            shotNumber++;
            lastGunSwitchTime = Time.time;
        }
    }
}
