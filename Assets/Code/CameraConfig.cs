using UnityEngine;

[System.Serializable]
public struct CameraConfig {

    [Tooltip("Radius of camera sphere (default)")] // 1-10, DefaultValue = 3
    public float defaultDistance;
    public Vector2 initialAngles;

    [Tooltip("Sensitivity (deg/s)")] // 1-180, DefaultValue = 90
    public float rotationSpeed;

    [Tooltip("Camera distance change speed (m/s)")] // 0.1f-10, DefaultValue = 5
    public float cameraDistanceDelta;

    [Tooltip("Focus area radius")] // 0, 1.5f, DefaultValue = 0.4f
    public float focusRadius;

    [Tooltip("Focus centering strength")] //, 0, 1, DefaultValue = 0.7f)]
    public float focusCentering;

    [Tooltip("Delay of camera aligning to movement")] //, 0, 10, DefaultValue = 3)]
    public float alignDelay;

    [Tooltip("Range of camera alignment (deg)")] //, 0, 90, DefaultValue = 45)]
    public float alignSmoothRange;

    public LayerMask obstructionMask;
}
