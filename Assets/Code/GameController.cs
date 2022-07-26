using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController main;

    private BoardEnvManager boardManager;
    private EncounterLoader encounterLoader;
    private CameraFollow cameraControl;
    [SerializeField] private AnimationCurve transitionCurve;

    private void Awake() {
        if (main != null) {
            Destroy(this);
        }

        main = this;
    }

    private void Start() {
        boardManager = GetComponentInChildren<BoardEnvManager>();
        encounterLoader = GetComponentInChildren<EncounterLoader>();
        cameraControl = FindObjectOfType<CameraFollow>();

        boardManager.NextTurn();
        SetCameraBoard();
    }

    // persistent listener of BoardEnvController.onTurnCompleted
    public void TurnOverCallback() {
        encounterLoader.LoadEncounter();
        StartCoroutine(encounterLoader.loadedEncounter.GetComponentInChildren<InputCharacterDriver>().gameObject.GetComponent<CharacterStats>().RollCharacterStats(DiceThrowOverCallback));
        
        SetCameraDiceThrow();
    }

    private void DiceThrowOverCallback() {
        var encounter = encounterLoader.loadedEncounter.GetComponent<EncounterAsset>();
        encounter.Begin();
        encounter.onEncounterFinished.AddListener(EncounterOverCallback);
        SetCameraEncounter(encounter);
    }

    private void EncounterOverCallback() {
        encounterLoader.UnloadEncounter();
        boardManager.NextTurn();
        SetCameraBoard();
    }

    private void SetCameraBoard() {
        cameraControl.SetTransitionValue(new Vector3(30, 0, 17));
        TransitionDriver.InitiateTransition(
            transitionCurve,
            new LightingData(5000, 2, 0),
            LightingControl.main,
            null
        );
    }
    private void SetCameraEncounter(EncounterAsset encounter) {

        var camAngles = new Vector3(80, -45);
        camAngles.z = CameraFollow.CalculateMinDistanceForCamera(camAngles, encounter.GetBoardSize());
        cameraControl.SetTransitionValue(camAngles);

        TransitionDriver.InitiateTransition(
            transitionCurve,
            new LightingData(6700, 1.5f, 0.33f),
            LightingControl.main,
            null
        );
    }

    private void SetCameraDiceThrow() {
        cameraControl.SetTransitionValue(new Vector3(80, 0, 7));
        TransitionDriver.InitiateTransition(
            transitionCurve,
            new LightingData(6700, 1.5f, 0.33f),
            LightingControl.main,
            null
        );
    }
}
