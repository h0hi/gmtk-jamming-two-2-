using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class DiceBehaviour : MonoBehaviour
{
    public UnityEvent<int> onLand = new UnityEvent<int>();
    [SerializeField] private float forceMul;
    [SerializeField] private float restTimeToDissapear;
    [SerializeField] private float dissapearTime;
    private Rigidbody rb;
    private float timeResting;

    private readonly Vector3[] dieNormals = new Vector3[] {
        Vector3.back,
        Vector3.left,
        Vector3.right,
        Vector3.forward,
        Vector3.up,
        Vector3.down
    };

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * forceMul);
    }

    private void Update() {
        if (rb.velocity.sqrMagnitude < 0.1f) {
            timeResting += Time.deltaTime;
        }

        if (timeResting > restTimeToDissapear) {
            StartCoroutine(Dissapear());
        }
    }

    private IEnumerator Dissapear() {
        
        var maxDot = -2f;
        int id = -1;
        for (int i = 0; i < 6; i++) {
            var transformedNormal = transform.TransformDirection(dieNormals[i]);
            var dot = Vector3.Dot(transformedNormal, Vector3.up);
            if (dot > maxDot) {
                maxDot = dot;
                id = i;
            }
        }

        onLand.Invoke(id + 1);

        var timeStart = Time.time;
        while (Time.time - timeStart < dissapearTime) {
            var t = 1 - (Time.time - timeStart) / dissapearTime;
            transform.localScale = Vector3.one * t;
            yield return null;
        }

        Destroy(gameObject);
    }
}
