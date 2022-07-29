using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    private Material mat;

    private void OnCollisionEnter(Collision collision) {
        var health = collision.gameObject.GetComponent<CharacterHealth>();
        if (health) {
            health.Damage();
        }

        var rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb) {
            var direction = (collision.transform.position - transform.position).normalized;
            rb.AddForce(direction * 30, ForceMode.Impulse);
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