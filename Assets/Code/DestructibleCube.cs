using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCube : MonoBehaviour
{
    [SerializeField] private int health;

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Cube Trigger from " + other.gameObject.name);
        if (other.gameObject.GetComponent<PelletBehaviour>()) {
            OnHit();
        }
    }

    private void OnHit() {
        health--;

        if (health <= 0) {
            Break();
        }
    }

    private void Break() {
        Destroy(gameObject);
    }
}
