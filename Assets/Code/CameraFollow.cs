using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform follow;
    [SerializeField] private float angleFollowTheta;
    [SerializeField] private float angleFollowPhi;
    [SerializeField] private float distanceFollow;

    private void Update() {

        var x = Mathf.Sin(angleFollowPhi * Mathf.Deg2Rad) * Mathf.Cos(angleFollowTheta * Mathf.Deg2Rad);
        var y = Mathf.Cos(angleFollowPhi * Mathf.Deg2Rad);
        var z = Mathf.Sin(angleFollowPhi * Mathf.Deg2Rad) * Mathf.Sin(angleFollowTheta * Mathf.Deg2Rad);

        var localPos = new Vector3(x, y, z) * distanceFollow;
        transform.position = follow.position + localPos;

        transform.LookAt(follow);
    } 
}
