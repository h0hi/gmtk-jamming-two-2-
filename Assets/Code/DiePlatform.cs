using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DiePlatform : MonoBehaviour
{
    [SerializeField] private float activationVelocity; 
    [SerializeField] private float turningTime;

    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {

            Debug.Log("Collision with player!");
            var rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb) {
                Debug.Log("Velocity magnitude: " + rb.velocity.magnitude);
                Activate();
            }
        }
    }

    private void Activate() {
        StartCoroutine(Turn());
    }

    private IEnumerator Turn() {
        var startTime = Time.time;
        var omega = 90f / turningTime;
        while (Time.time - startTime < turningTime) {
            transform.Rotate(omega * Time.deltaTime * Vector3.right);
            yield return null;
        }
    }
}
