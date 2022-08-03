using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        var health = other.transform.parent.GetComponent<CharacterHealth>();
        if (health && !other.gameObject.CompareTag("Shielded")) {
            health.Damage();
        }

        var rb = other.transform.parent.GetComponent<Rigidbody>();
        if (rb) {
            var direction = (other.transform.position - transform.position).normalized;
            rb.AddForce(direction * 10, ForceMode.VelocityChange);
        }
    }
}