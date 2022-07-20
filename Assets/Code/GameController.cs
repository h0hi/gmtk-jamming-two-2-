using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController main;

    private BoardEnvManager boardManager;
    private ShooterEnvManager encounterManager;
    private CameraFollow cameraControl;
    [SerializeField] private AnimationCurve transitionCurve;
    [SerializeField] private float transitionTime;

    private void Awake() {
        if (main != null) {
            Destroy(this);
        }

        main = this;
    }

    private void Start() {
        boardManager = GetComponentInChildren<BoardEnvManager>();
        encounterManager = GetComponentInChildren<ShooterEnvManager>();
        cameraControl = FindObjectOfType<CameraFollow>();

        boardManager.onTurnCompleted.AddListener(TurnOverCallback);
        encounterManager.onEncounterCompleted.AddListener(EncounterOverCallback);

        boardManager.NextTurn();
        SetCameraBoard();
    }

    private void TurnOverCallback() {

        var encounterNames = new string[] {
            "Narrow",
            "SquareOff"
        };

        var name = encounterNames[Random.Range(0, encounterNames.Length)];

        encounterManager.LoadEncounter(name);
        SetCameraEncounter();
    }

    private void EncounterOverCallback() {
        boardManager.NextTurn();
        SetCameraBoard();
    }

    private void SetCameraBoard() {
        TransitionDriver.InitiateTransition(
            transitionTime,
            transitionCurve,
            new Vector3(30, 0, 17),
            cameraControl
        );
        TransitionDriver.InitiateTransition(
            transitionTime,
            transitionCurve,
            new LightingData(5000, 2, 0),
            LightingControl.main
        );
    }
    private void SetCameraEncounter() {
        TransitionDriver.InitiateTransition(
            transitionTime,
            transitionCurve,
            new Vector3(80, -45, 7),
            cameraControl
        );
        TransitionDriver.InitiateTransition(
            transitionTime,
            transitionCurve,
            new LightingData(6700, 1.5f, 0.33f),
            LightingControl.main
        );
    }
}
