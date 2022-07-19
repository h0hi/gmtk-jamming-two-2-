using UnityEngine;

public class CharacterGun : MonoBehaviour
{
    [SerializeField] private float pelletSpawnDistance;
    [SerializeField] private float cooldown = 1;

    private GameObject pelletPrefab;
    private float lastShotTime;

    private void Start() {
        pelletPrefab = Resources.Load<GameObject>("Top Down Env/pellet");
    }

    private void Update() {
        var position = transform.position;
        var cursor = GetProjectedMousePoint();
        var dir = (cursor - position).normalized;
        dir.y = 0;
        var angle = Mathf.Atan2(dir.x, dir.z);
        transform.eulerAngles = angle * Mathf.Rad2Deg * Vector3.up;
        
        if (Input.GetMouseButton(0) && Time.time - lastShotTime > cooldown) {
            ShootPellet(dir);   
        }
    }

    private void OnEnable() {
        var stats = GetComponent<CharacterStats>();
        cooldown = 1f / stats.GetSPS();
    }

    private void OnDisable() {
        // cleanup pellets
        var pellets = FindObjectsOfType<PelletBehaviour>();

        foreach (var pellet in pellets) {
            Destroy(pellet.gameObject);
        }
    }

    private Vector3 GetProjectedMousePoint() {
        var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(cameraRay, out var hitInfo, Mathf.Infinity, 1)) {
            return hitInfo.point;
        }
        return cameraRay.origin;    
    }

    private void ShootPellet(Vector3 dir) {
        lastShotTime = Time.time;
        Instantiate(pelletPrefab, transform.position + dir * pelletSpawnDistance, Quaternion.FromToRotation(Vector3.forward, dir), null);
    }
}