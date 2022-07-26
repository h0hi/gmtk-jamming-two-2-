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

        target = GameObject.FindWithTag("Player").transform;
        movementDevice.intentionToJump = false;
    }

    private void Update() {
        var direction = (target.position - transform.position).normalized;
        direction.y = 0;
        var move = new Vector2(direction.x, direction.z);
        movementDevice.moveVector = move.normalized;

        gunDevice.shootDirection = direction;
        gunDevice.TryShoot();
    }
}
