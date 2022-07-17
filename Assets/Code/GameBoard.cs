using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public static GameBoard main;

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
        encounterManager.LoadEncounter(0);
        SetCameraEncounter();
    }

    private void EncounterOverCallback() {
        boardManager.NextTurn();
        SetCameraBoard();
    }

    private void SetCameraBoard() {
        cameraControl.UpdateRotationAndDistance(transitionTime, transitionCurve, 30, 0, 17);
        LightingControl.ApplyLightingAsset(transitionTime, transitionCurve, 0);
    }
    private void SetCameraEncounter() {
        cameraControl.UpdateRotationAndDistance(transitionTime, transitionCurve, 80, -45, 7);
        LightingControl.ApplyLightingAsset(transitionTime, transitionCurve, 1);
    }
}
