using UnityEngine;

public class PelletBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    private void Start() {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.gameObject.CompareTag("Shielded")) {
            var characterHealth = other.transform.parent.GetComponent<CharacterHealth>();
            if (characterHealth) {
                characterHealth.Damage();
            }
        }
        
        if (!other.isTrigger) {
            Evaporate();
        }
    }

    public void Evaporate() {
        Destroy(gameObject);
    }
}
