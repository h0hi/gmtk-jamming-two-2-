using UnityEngine;

[RequireComponent(typeof(CharacterControl))]
[RequireComponent(typeof(CharacterGun))]
public class InputCharacterDriver : MonoBehaviour
{
    private CharacterControl movementDevice;
    private CharacterGun gunDevice;
    private Transform camTransform;

    private void Start() {
        camTransform = Camera.main.transform;
        movementDevice = GetComponent<CharacterControl>();
        movementDevice.intentionToJump = false;
        gunDevice = GetComponent<CharacterGun>();
        gunDevice.pelletPrefab = Resources.Load<GameObject>("Encounter/pellet");
    }

    private void Update() {
        // drive movementDevice
        var moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        var v = new Vector3(moveVector.normalized.x, 0, moveVector.normalized.y);
        v = Quaternion.Euler(0, camTransform.eulerAngles.y, 0) * v;
        movementDevice.moveVector = new Vector2(v.x, v.z);

        // drive gun device
        var position = transform.position;
        var cursor = GetProjectedMousePoint();
        var dir = (cursor - position).normalized;
        dir.y = 0;
        gunDevice.shootDirection = dir;
        
        if (Input.GetMouseButton(0)) {
            gunDevice.TryShoot();   
        }
    }

    private Vector3 GetProjectedMousePoint() {
        var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(cameraRay, out var hitInfo, Mathf.Infinity, 1)) {
            return hitInfo.point;
        }
        return cameraRay.origin;    
    }
}
