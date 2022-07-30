using UnityEngine;

[RequireComponent(typeof(CharacterControl))]
[RequireComponent(typeof(CharacterGun))]
public class RandomEnemyDriver : MonoBehaviour
{
    private CharacterControl movementDevice;
    private CharacterGun gunDevice;
    private float lookAngleRadians;
    private const float rotationSpeedRadians = 1;
    
    private void Start() {
        movementDevice = GetComponent<CharacterControl>();
        gunDevice = GetComponent<CharacterGun>();

        movementDevice.intentionToJump = false;

        var angle = Mathf.PI * -2f / 3f;
        lookAngleRadians = angle;
        movementDevice.moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    private void Update() {

        var moveDirection3D = new Vector3(movementDevice.moveDirection.x, 0, movementDevice.moveDirection.y);
        if (Physics.Raycast(new Ray(transform.position, moveDirection3D), out var hit, 1f, 1)) {
            moveDirection3D = -2 * Vector3.Dot(moveDirection3D, hit.normal) * hit.normal + moveDirection3D;
            movementDevice.moveDirection = new Vector2(moveDirection3D.x, moveDirection3D.z);
        }

        lookAngleRadians += Time.deltaTime * rotationSpeedRadians;
        movementDevice.lookRadians = lookAngleRadians;

        gunDevice.TryShoot();
    }
}
