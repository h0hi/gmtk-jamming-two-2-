using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    private void Start() {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other) {
        Destroy(gameObject);
    }
}
