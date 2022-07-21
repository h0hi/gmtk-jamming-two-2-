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
            "SquareOff",
            "Siege",
            "Labyrinth"
        };

        var rnd = new System.Random(System.DateTime.Now.GetHashCode());
        var name = encounterNames[rnd.Next(encounterNames.Length)];

        encounterManager.LoadEncounter(name);
        encounterManager.onEncounterLoaded.AddListener(SetCameraEncounter);
        encounterManager.onEncounterLoaded.AddListener(encounterManager.DoAppearBoard);
        SetCameraDiceThrow();
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
    private void SetCameraEncounter(EncounterAsset encounter) {

        var encounterBoardSize = new Vector2(encounter.GetHeight(), encounter.GetLength());
        var camAngles = new Vector3(80, -45);
        camAngles.z = CameraFollow.CalculateMinDistanceForCamera(camAngles, encounterBoardSize);

        TransitionDriver.InitiateTransition(
            transitionTime,
            transitionCurve,
            camAngles,
            cameraControl
        );
        TransitionDriver.InitiateTransition(
            transitionTime,
            transitionCurve,
            new LightingData(6700, 1.5f, 0.33f),
            LightingControl.main
        );
    }

    private void SetCameraDiceThrow() {
        TransitionDriver.InitiateTransition(
            transitionTime,
            transitionCurve,
            new Vector3(80, 0, 7),
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
