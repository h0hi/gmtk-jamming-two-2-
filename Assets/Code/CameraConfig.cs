[System.Serializable]
public struct CameraConfig {

    //[Slider("Radius of camera sphere (swimming)", 1, 10, DefaultValue = 3)]
    public float swimDistance;

    //[Slider("Radius of camera sphere (seamoth/prawn)", 1, 10, DefaultValue = 6)]
    public float vehicleDistance;

    //[Slider("Radius of camera sphere (piloting cyclops)", 1, 10, DefaultValue = 1)]
    public float cyclopsDistance;

    //[Toggle("Switch to first person in bases/cyclops"), OnChange(nameof(OnFirstPersonInBaseChange))]
    public bool switchToFirstPersonWhenInside;

    //[Slider("Sensitivity (deg/s)", 1, 180, DefaultValue = 90)]
    public float rotationSpeed;

    //[Slider("Camera distance change speed (m/s)", 0.1f, 10, DefaultValue = 5)]
    public float cameraDistanceDelta;

    //[Slider("Focus area radius", 0, 1.5f, DefaultValue = 0.4f)]
    public float focusRadius;

    //[Slider("Focus centering strength", 0, 1, DefaultValue = 0.7f)]
    public float focusCentering;

    //[Slider("Delay of camera aligning to movement", 0, 10, DefaultValue = 3)]
    public float alignDelay;

    //[Slider("Range of camera alignment (deg)", 0, 90, DefaultValue = 45)]
    public float alignSmoothRange;
}
