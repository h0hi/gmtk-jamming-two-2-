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
