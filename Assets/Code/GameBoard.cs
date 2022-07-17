using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public static GameBoard main;

    private BoardEnvManager boardManager;
    private ShooterEnvManager encounterManager;
    private CameraFollow cameraControl;

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
        cameraControl.SetRotation(30, 0);
        cameraControl.SetDistance(17);
    }
    private void SetCameraEncounter() {
        cameraControl.SetRotation(80, -45);
        cameraControl.SetDistance(7);
    }
}
