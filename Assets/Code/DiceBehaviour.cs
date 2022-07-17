using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class DiceBehaviour : MonoBehaviour
{
    public UnityEvent<int> onLand = new ();
    [SerializeField] private float forceMul;
    [SerializeField] private float restTimeToDissapear;
    [SerializeField] private float dissapearTime;
    private Rigidbody rb;
    private float timeResting;
    private bool rested;

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
    }

    private void Update() {
        if (rb.velocity.sqrMagnitude < 0.1f) {
            timeResting += Time.deltaTime;
        }

        if (timeResting > restTimeToDissapear && !rested) {
            rested = true;
            InvokeEvent();
            StartCoroutine(Dissapear());
        }
    }

    private void InvokeEvent() {
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

        Debug.Log("Die result is " + (id + 1));
        onLand.Invoke(id + 1);
    }

    private IEnumerator Dissapear() {
        var timeStart = Time.time;
        while (Time.time - timeStart < dissapearTime) {
            var t = (Time.time - timeStart) / dissapearTime;
            transform.localScale = Vector3.one * (1 - t * t);
            yield return null;
        }

        Destroy(gameObject);
    }
}
