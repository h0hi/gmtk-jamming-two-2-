using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GameBoard : MonoBehaviour
{
    public static GameBoard main;

    [SerializeField] private Vector3[] boardPoints;
    [SerializeField] private bool boardLooping;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private float playerHopTime;
    [SerializeField] private float playerHopHeight;

    private GameObject playerPipPrefab;
    private LineRenderer lineRenderer;

    private GameObject[] pointGameObjects;
    private List<GameObject> playerPips = new ();
    private List<int> playerLocation = new ();
    private int playerWhoseTurnItIsNow;

    private GameState gameState;

    private void Awake() {
        if (main != null) {
            Destroy(this);
        }

        main = this;
    }

    private void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        playerPipPrefab = Resources.Load<GameObject>("player_pip");

        DrawBoard();
        AddPlayer();
    }

    private void Update() {
        if (Input.GetKey(KeyCode.E) && gameState == GameState.WaitingForInput) {
            gameState = GameState.InCinematic;
            DieThrower.main.ThrowD6(Camera.main.transform.position, Camera.main.transform.forward, MovePlayer);
        }
    }

    private void DrawBoard() {
        lineRenderer.positionCount = boardPoints.Length;
        lineRenderer.SetPositions(boardPoints);
        lineRenderer.loop = boardLooping;

        pointGameObjects = new GameObject[boardPoints.Length];
        for (int i = 0; i < boardPoints.Length; i++) {
            pointGameObjects[i] = Instantiate(pointPrefab, boardPoints[i], Quaternion.identity, transform);
        }
    }

    private void AddPlayer() {
        playerLocation.Add(0);
        playerPips.Add(Instantiate(playerPipPrefab, boardPoints[0], Quaternion.identity, transform));
    }

    private void MovePlayer(int move) {
        var location = playerLocation[playerWhoseTurnItIsNow];
        if (!boardLooping && location + move >= boardPoints.Length) {
            move = boardPoints.Length - location - 1;
        }
        if (move == 0) return;
        StartCoroutine(DoPlayerHops(playerWhoseTurnItIsNow, location, move, OnUserTurnEnd));        
    }

    private IEnumerator DoPlayerHops(int playerId, int startPoint, int hops, System.Action callback) {
        for (int h = 0; h < hops; h++) {
            var a = GetPointModLength(startPoint + h);
            var b = GetPointModLength(startPoint + h + 1);
            yield return DoPlayerHop(playerId, a, b);
            playerLocation[playerId] = (startPoint + h + 1) % boardPoints.Length;
        }

        callback();
    }

    private void OnUserTurnEnd() {
        Debug.Log("Player now at " + playerLocation[playerWhoseTurnItIsNow]);
        gameState = GameState.WaitingForInput;

        playerWhoseTurnItIsNow += 1;
        playerWhoseTurnItIsNow %= playerPips.Count;
    }

    private IEnumerator DoPlayerHop(int playerId, Vector3 a, Vector3 b) {
        var startTime = Time.time;

        while (Time.time - startTime < playerHopTime) {
            var t = (Time.time - startTime) / playerHopTime;
            var pos = Vector3.Lerp(a, b, t);
            pos += (1 - Mathf.Pow(t * 2 - 1, 2)) * playerHopHeight * Vector3.up;
            playerPips[playerId].transform.position = pos;
            yield return null;
        }
    }

    public Vector3 GetPointModLength(int i) => boardPoints[i % boardPoints.Length];

    public enum GameState {
        WaitingForInput,
        InCinematic
    }
}
