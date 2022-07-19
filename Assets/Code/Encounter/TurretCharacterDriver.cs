using UnityEngine;

[RequireComponent(typeof(CharacterGun))]
public class TurretCharacterDriver : MonoBehaviour
{
    private CharacterGun gunDevice;
    private Transform target;
    
    private void Start() {
        gunDevice = GetComponent<CharacterGun>();
        target = GameObject.FindWithTag("Player").transform;
    }

    private void Update() {
        var direction = (target.position - transform.position).normalized;
        direction.y = 0;
        gunDevice.shootDirection = direction;
        gunDevice.TryShoot();
    }
}
