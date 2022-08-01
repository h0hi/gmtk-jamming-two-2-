using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    private Material mat;

    private void OnTriggerEnter(Collider other) {
        var health = other.transform.parent.GetComponent<CharacterHealth>();
        if (health && !other.gameObject.CompareTag("Shielded")) {
            health.Damage();
        }

        var rb = other.transform.parent.GetComponent<Rigidbody>();
        if (rb) {
            var direction = (other.transform.position - transform.position).normalized;
            rb.AddForce(direction * 100, ForceMode.Impulse);
        }
    }

    private void Start() {
        mat = GetComponent<MeshRenderer>().material;
    }

    private void Update() {
        var t = Time.time * 0.25f;
        mat.mainTextureOffset = new Vector2(t, -t);
    }
}