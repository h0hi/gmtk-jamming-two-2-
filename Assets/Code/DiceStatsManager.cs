using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceStatsManager : MonoBehaviour
{
    [SerializeField] private Transform dieTransform;
    private Rigidbody dieRb;
    [SerializeField] private float maxTorque;

    private bool busyRolling;

    private readonly Vector3[] dieNormals = new Vector3[] {
        Vector3.back,
        Vector3.left,
        Vector3.right,
        Vector3.forward,
        Vector3.up,
        Vector3.down
    };

    private void Start() {
        dieRb = dieTransform.GetComponent<Rigidbody>();
    }

    private void Update() {
        if (busyRolling && dieRb.angularVelocity.magnitude < 0.1f) {
            busyRolling = false;
            dieRb.angularVelocity = Vector3.zero;

            Debug.Log("New value is " + GetScoreBasedOnOrientation());
        }
    }

    private void FixedUpdate() {
        if (Input.GetKey(KeyCode.E) && !busyRolling) {
            RollStat();
        }
    }

    private void RollStat() {
        dieTransform.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(Random.value, Random.value, Random.value) * maxTorque);
        busyRolling = true;
    }

    private int GetScoreBasedOnOrientation() {
        var maxDot = -2f;
        int id = -1;
        for (int i = 0; i < 6; i++) {
            var transformedNormal = dieTransform.TransformDirection(dieNormals[i]);
            var dot = Vector3.Dot(transformedNormal, Vector3.up);
            if (dot > maxDot) {
                maxDot = dot;
                id = i;
            }
        }

        return id + 1;
    }
}
