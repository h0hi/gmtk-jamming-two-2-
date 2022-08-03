using UnityEngine;

[RequireComponent(typeof(CharacterControl))]
public class FrogEnemyDriver : MonoBehaviour
{
    private CharacterControl movementDevice;
    private Transform target;
    private float lastJumpTime;
    private GameObject shockwavePrefab;
    [SerializeField] private float waitSecondsAfterLand;

    private void Start() {
        movementDevice = GetComponent<CharacterControl>();
        shockwavePrefab = AssetLoader.LoadAsset<GameObject>("Assets/Prefabs/EncPrefabs/Frog Shockwave.prefab");
    }

    private void Update() {

        if (target == null) {
            target = GameObject.FindWithTag("Player").transform;
        }

        var direction = (target.position - transform.position).normalized;
        direction.y = 0;
        var move = new Vector2(direction.x, direction.z);
        movementDevice.lookRadians = Mathf.Atan2(direction.x, direction.z);
        
        if (Time.time - lastJumpTime > waitSecondsAfterLand) {
            Jump(move);
        }
    }
    
    private void Jump(Vector2 moveVector) {
        movementDevice.intentionToJump = true;
        movementDevice.moveDirection = moveVector.normalized;
        var jumpDuration = movementDevice.GetJumpDuration();
        lastJumpTime = Time.time + jumpDuration;
        Invoke(nameof(OnLand), jumpDuration);
    }

    private void OnLand() {
        movementDevice.intentionToJump = false;
        movementDevice.moveDirection = Vector2.zero;

        Instantiate(shockwavePrefab, transform.position, transform.rotation, null);
    }
}
