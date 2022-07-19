using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCube : MonoBehaviour
{
    [SerializeField] private int health;

    private void OnTriggerEnter(Collider other) {
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
        transform.parent.parent.gameObject.SendMessage("TargetEliminated");
        Destroy(gameObject);
    }
}
