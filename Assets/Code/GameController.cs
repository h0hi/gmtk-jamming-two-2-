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

        boardManager.AdvanceTurn();
        SetCameraBoard();
    }

    // persistent listener of BoardEnvController.onTurnCompleted
    public void TurnOverCallback() {
        var poiData = boardManager.GetPointOfInterestForPlayer(boardManager.PlayerWhoseTurnItIsNow);
        encounterLoader.LoadEncounter(poiData.environmentBundleName, poiData.waveAsset);
        SetCameraEncounter(encounterLoader.loadedEncounter.GetComponent<EncounterAsset>());
        Invoke(nameof(DiceThrowOverCallback), transitionCurve.keys[transitionCurve.length - 1].time);
    }

    private void DiceThrowOverCallback() {
        var encounter = encounterLoader.loadedEncounter.GetComponent<EncounterAsset>();
        encounter.Begin();
        encounter.onEncounterFinished.AddListener(EncounterOverCallback);
    }

    private void EncounterOverCallback() {
        encounterLoader.UnloadEncounter();
        boardManager.NextTurn();
        SetCameraBoard();
    }

    private void SetCameraBoard() {
        TransitionDriver.InitiateTransition(
            transitionCurve,
            new Vector3(30, 0, 17),
            cameraControl,
            null
        );
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
        TransitionDriver.InitiateTransition(
            transitionCurve,
            camAngles,
            cameraControl,
            null
        );

        TransitionDriver.InitiateTransition(
            transitionCurve,
            new LightingData(7700, 0.2f, 0.33f),
            LightingControl.main,
            null
        );
    }

    private void SetCameraDiceThrow() {
        TransitionDriver.InitiateTransition(
            transitionCurve,
            new Vector3(80, 0, 7),
            cameraControl,
            null
        );
        TransitionDriver.InitiateTransition(
            transitionCurve,
            new LightingData(7700, 0.5f, 0.33f),
            LightingControl.main,
            null
        );
    }
}
