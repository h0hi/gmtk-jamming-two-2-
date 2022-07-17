using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public bool lockRotation;
    private float rotationX;
    private float rotationY;
    private float lastManualRotationTime;
    private float currentDistance;
    private float skin;
    private Vector3 focusPoint, focusPointLastFrame;

    private Transform focusTransform;
    private Transform cameraTransform;
    private Transform viewModelTransform;
    private Transform playerTransform;

    private const int obstructionMask = -2097153;
    private Vector3 cameraHalfExtends;

    [SerializeField] private CameraConfig config;

    private void Update() {

        UpdateFocusPoint();

        Quaternion lookRotation;
        if (lockRotation && (ManualRotation() || AutomaticRotation())) {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(rotationX, rotationY, 0);
        } else {
            lookRotation = cameraTransform.localRotation;
        }

        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * config.swimDistance;

        Vector3 rectOffset = lookDirection * Camera.main.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = focusTransform.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;

        if (Physics.BoxCast(castFrom, cameraHalfExtends, castDirection, out var hit, lookRotation, castDistance, obstructionMask)) {
            rectPosition = castFrom + castDirection * SmoothMoveToDistance(hit.distance);
            lookPosition = rectPosition - rectOffset;
        } else {
            lookPosition = focusPoint - lookDirection * SmoothMoveToDistance(config.swimDistance);
        }

        cameraTransform.SetPositionAndRotation(lookPosition, lookRotation);
        UpdateViewModel();
    }

    private float SmoothMoveToDistance(float newDistance) {
        currentDistance = Mathf.MoveTowards(currentDistance, newDistance, Time.deltaTime * config.cameraDistanceDelta);
        return currentDistance;
    }
    
    private void UpdateFocusPoint() {
        focusPointLastFrame = focusPoint;
        
        Vector3 targetPoint = focusTransform.position;

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

    private bool ManualRotation() {

        Vector2 input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
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

        Vector2 movement = new Vector2(
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

    // Updates this component's transform, as well as player viewmodel
    private void UpdateViewModel() {

        var headingAngles = new Vector3(rotationX, rotationY);
        transform.localEulerAngles = Vector3.zero;
        transform.localPosition = Vector3.down * skin;
        viewModelTransform.localEulerAngles = Vector3.zero;
        viewModelTransform.localPosition = transform.localPosition;
        
        // Player transform needs only Y input, since view model, which is its child, is rotated on the X axis via animation
        playerTransform.localEulerAngles = Vector3.up * headingAngles.y;
        transform.localEulerAngles = Vector3.right * headingAngles.x;
    }

    private void ConstrainAngles() {
        rotationX = Mathf.Clamp(rotationX % 360, -80, 80);
        rotationY %= 360;
    }
    private static float GetAngle(Vector2 direction) {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x > 0 ? angle : 360 - angle;
    }
}
