using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour, ITransitionPassenger<Vector3>
{
    public bool lockRotationX;
    public bool lockRotationY;
    private float rotationX;
    private float rotationY;
    private float lastManualRotationTime;
    private float currentDistance;
    private Vector3 focusPoint, focusPointLastFrame;

    private Vector3 cameraHalfExtends;

    [SerializeField] private Transform cameraFollowTransform;
    [SerializeField] private CameraConfig config;

    private void Start() {
        cameraHalfExtends.y = Camera.main.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * Camera.main.fieldOfView);
        cameraHalfExtends.x = cameraHalfExtends.y * Camera.main.aspect;
        cameraHalfExtends.z = 0;

        transform.localRotation = Quaternion.Euler(config.initialAngles.x, config.initialAngles.y, 0);

        currentDistance = config.defaultDistance;
    }

    private void Update() {

        UpdateFocusPoint();

        Quaternion lookRotation;
        if (ManualRotation() || AutomaticRotation()) {
            ConstrainAngles();
        }
        lookRotation = Quaternion.Euler(rotationX, rotationY, 0);

        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * SmoothMoveToDistance(config.defaultDistance);

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    private float SmoothMoveToDistance(float newDistance) {
        currentDistance = Mathf.MoveTowards(currentDistance, newDistance, Time.deltaTime * config.cameraDistanceDelta);
        return currentDistance;
    }
    
    private void UpdateFocusPoint() {
        focusPointLastFrame = focusPoint;
        
        Vector3 targetPoint = cameraFollowTransform.position;

        if (config.focusRadius > 0) {
            var distance = Vector3.Distance(targetPoint, focusPoint);
            var t = 1f;
            if (distance > 0.01f && config.focusCentering > 0) {
                t = Mathf.Pow(1 - config.focusCentering, Time.unscaledDeltaTime);
            }
            if (distance > config.focusRadius) {
                t = Mathf.Min(t, config.focusRadius / distance);
            }
            focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
        } else {
            focusPoint = targetPoint;
        }
    }

    public static float CalculateMinDistanceForCamera(Vector2 camAngles, Vector2 planeSize) {
        var frustumHeight = Mathf.Sqrt(planeSize.x * planeSize.x + planeSize.y * planeSize.y);
        frustumHeight *= Mathf.Sin(camAngles.x * Mathf.Deg2Rad);
        return frustumHeight * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
    } 

    private bool ManualRotation() {

        Vector2 input = new Vector2 (
            lockRotationY ? 0 : Input.GetAxisRaw("Mouse X"), 
            lockRotationX ? 0 : Input.GetAxisRaw("Mouse Y"));
        const float e = 0.001f;
        if (input.x < -e || input.x > e || input.y < -e || input.y > e) {
            rotationX -= config.rotationSpeed * Time.unscaledDeltaTime * input.y;
            rotationY += config.rotationSpeed * Time.unscaledDeltaTime * input.x;
            lastManualRotationTime = Time.unscaledTime;
            return true;
        }
        return false;
    }

    private bool AutomaticRotation() {
        if (Time.unscaledTime - lastManualRotationTime < config.alignDelay) {
            return false;
        }
        if (lockRotationY) return false;

        Vector2 movement = new (
            focusPoint.x - focusPointLastFrame.x,
            focusPoint.z - focusPointLastFrame.z
        ); // here we're concerned only about rotating about Y axis, so we look only at movement in XZ plane
        float movementDeltaSqr = movement.sqrMagnitude;
        if (movementDeltaSqr < 0.000001f) {
            return false;
        }

        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(rotationY, headingAngle));
        float rotationChange = config.rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
        if (deltaAbs < config.alignSmoothRange) {
            rotationChange *= deltaAbs / config.alignSmoothRange;
        } 
        else if (180f - deltaAbs < config.alignSmoothRange) {
            rotationChange *= (180f - deltaAbs) / config.alignSmoothRange;
        }
        rotationY = Mathf.MoveTowardsAngle(rotationY, headingAngle, rotationChange);

        return true;
    }

    private void ConstrainAngles() {
        rotationX = Mathf.Clamp(rotationX % 360, -80, 80);
        rotationY %= 360;
    }
    private static float GetAngle(Vector2 direction) {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x > 0 ? angle : 360 - angle;
    }

    public Vector3 GetTransitionValue() => new Vector3(rotationX, rotationY, currentDistance);
    public void SetTransitionValue(Vector3 value) {
        rotationX = value.x;
        rotationY = value.y;
        config.defaultDistance = value.z;
        currentDistance = value.z;
    }
}
