using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class BoardEnvManager : MonoBehaviour
{
    [SerializeField] private float playerHopTime;
    [SerializeField] private float playerHopHeight;
    [SerializeField] private Color boardDieColor;

    private BoardGraph graph;
    private readonly List<GameObject> playerPips = new ();
    private readonly List<int> playerLocation = new ();
    public int PlayerWhoseTurnItIsNow { get; private set; }
    private bool waitingForInput;
    private BoardToEncounterTransitionDriver boardToEncounterTransitionDriver;

    public UnityEvent onTurnCompleted;

    private void Start() {
        graph = GetComponentInChildren<BoardGraph>();
        AddPlayer();
    }

    private void Update() {
        if (waitingForInput && Input.GetKey(KeyCode.E)) {
            waitingForInput = false;
            DieThrower.main.ThrowD6(Camera.main.transform.position, Camera.main.transform.forward, MovePlayer, boardDieColor);
        }
    }

    public void NextTurn() {
        TransitionDriver.InitiateTransition(
            AnimationCurve.EaseInOut(0, 0, 1, 1),
            0,
            boardToEncounterTransitionDriver,
            AdvanceTurn
        );
    }

    public void AdvanceTurn() {
        PlayerWhoseTurnItIsNow += 1;
        PlayerWhoseTurnItIsNow %= playerPips.Count;
        waitingForInput = true;
        boardToEncounterTransitionDriver = null;
    }

    public PointOfInterestData GetPointOfInterestForPlayer(int id) {
        return graph.GetTransformAtDistance(playerLocation[id]).GetComponent<PointOfInterestData>();
    }

    public GameObject GetPlayerPip(int id) => playerPips[id];

    private void OnUserTurnEnd() {
        onTurnCompleted.Invoke();
        boardToEncounterTransitionDriver = new BoardToEncounterTransitionDriver(graph, playerLocation[PlayerWhoseTurnItIsNow]);
        TransitionDriver.InitiateTransition(
            AnimationCurve.EaseInOut(0, 0, 1, 1),
            1,
            boardToEncounterTransitionDriver,
            null
        );
    }

    private void AddPlayer() {
        var boardAssetBundle = AssetBundle.LoadFromFile(System.IO.Path.Join(Application.dataPath, "AssetBundles", "boardassets"));
        var playerPipPrefab = boardAssetBundle.LoadAsset<GameObject>("player_pip");
        playerLocation.Add(0);
        playerPips.Add(Instantiate(playerPipPrefab, graph.GetStartingPosition(), Quaternion.identity, transform));
    }

    private void MovePlayer(int move) {
        var location = playerLocation[PlayerWhoseTurnItIsNow];
        StartCoroutine(DoPlayerHops(PlayerWhoseTurnItIsNow, location, move, OnUserTurnEnd));        
    }

    private IEnumerator DoPlayerHops(int playerId, int startPoint, int hops, System.Action callback) {
        for (int h = 0; h < hops; h++) {
            var a = graph.GetPointAtDistance(startPoint + h);
            var b = graph.GetPointAtDistance(startPoint + h + 1);
            yield return DoPlayerHop(playerId, a, b);
            playerLocation[playerId] = (startPoint + h + 1) % graph.Length;
        }

        callback();
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

    private class BoardToEncounterTransitionDriver : ITransitionPassenger<float>
    {
        private static readonly Vector3 encounterPointScale = new (14.142f, 0.01f, 14.142f);
        private readonly Transform targetPoint;
        private readonly Transform[] otherPoints;
        private readonly Vector3 targetPointStartPos;
        public float startValue;
        public BoardToEncounterTransitionDriver(BoardGraph graph, int playerLocation) {
            var otherPointsList = new List<Transform> ();

            for (var i = 0; i < graph.Length; i++) {
                if (i == playerLocation) {
                    targetPoint = graph.GetTransformAtDistance(i);
                } else {
                    otherPointsList.Add(graph.GetTransformAtDistance(i));
                }
            }

            otherPoints = otherPointsList.ToArray();
            targetPointStartPos = targetPoint.position;
            startValue = 0;
        }

        public float GetTransitionValue() => startValue;

        public void SetTransitionValue(float value)
        {
            targetPoint.position = Vector3.LerpUnclamped(targetPointStartPos, new Vector3(0, 1.45f, 0), value);
            targetPoint.localScale = Vector3.LerpUnclamped(Vector3.one, encounterPointScale, value);
            foreach (var item in otherPoints) {
                item.localScale = Vector3.one * (1 - value);
            }
            startValue = value;
        }
    }
}
