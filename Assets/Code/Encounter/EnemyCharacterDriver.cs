using UnityEngine;

[RequireComponent(typeof(CharacterControl))]
[RequireComponent(typeof(CharacterGun))]
public class EnemyCharacterDriver : MonoBehaviour
{
    private CharacterControl movementDevice;
    private CharacterGun gunDevice;
    private Transform target;
    
    private void Start() {
        movementDevice = GetComponent<CharacterControl>();
        gunDevice = GetComponent<CharacterGun>();

        movementDevice.intentionToJump = false;
    }

    private void Update() {

        if (target == null) {
            target = GameObject.FindWithTag("Player").transform;
        }

        var direction = (target.position - transform.position).normalized;
        direction.y = 0;
        var move = new Vector2(direction.x, direction.z);
        movementDevice.moveDirection = move.normalized;

        movementDevice.lookRadians = Mathf.Atan2(direction.x, direction.z);
        
        gunDevice.TryShoot();
    }
}
