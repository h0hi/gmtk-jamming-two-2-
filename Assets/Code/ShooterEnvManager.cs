using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShooterEnvManager : MonoBehaviour
{
    [SerializeField] private float tileScale;
    [SerializeField] private Vector3 tileOffset;
    [SerializeField] private float loadoutRiseTime;
    [SerializeField] private AnimationCurve loadoutRiseCurve;

    private ShooterEnvLoadout[] loadouts;
    private EncounterBoardController boardController;    
    private GameObject indestructibleBlockPrefab;
    private GameObject destructibleBlockPrefab;
    private int remainingTargets;
    private GameObject loadoutParent;
    private GameObject playerGameObject;
    private int selectedLoadoutId;

    public UnityEvent onEncounterCompleted;

    private void Start() {
        indestructibleBlockPrefab = Resources.Load<GameObject>("Top Down Env/Indestructible Cube");
        destructibleBlockPrefab = Resources.Load<GameObject>("Top Down Env/Destructible Cube");
        loadouts = new ShooterEnvLoadout[] {
            new ShooterEnvLoadout()
        };

        boardController = new EncounterBoardController(transform);

        playerGameObject = FindObjectOfType<CharacterControl>().gameObject;
        playerGameObject.SetActive(false);
    }

    public void LoadEncounter(int i) {
        var stats = playerGameObject.GetComponent<CharacterStats>();
        StartCoroutine(stats.RollCharacterStats(CreateLoadout));
        selectedLoadoutId = i;
    }

    private void CreateLoadout() {

        var loadout = loadouts[selectedLoadoutId];
        playerGameObject.SetActive(true);
        remainingTargets = 0;

        loadoutParent = new GameObject("Loadout");
        loadoutParent.transform.SetParent(transform);
        loadoutParent.transform.localPosition = Vector3.up * 1;

        List<Vector3> spawnPositions = new ();
        var res = new Vector2(loadout.blocks[0].Length, loadout.blocks.Length);

        for (int y = 0; y < res.y; y++) {
            for (int x = 0; x < res.x; x++) {
                var position = new Vector3(x, 0, y) * tileScale + tileOffset;

                int tileType = loadout.blocks[y][x];
                switch (tileType) {
                    default:
                        break;
                    case 1:
                        Instantiate(indestructibleBlockPrefab, position, Quaternion.identity, loadoutParent.transform);
                        break;
                    case 2:
                        Instantiate(destructibleBlockPrefab, position, Quaternion.identity, loadoutParent.transform);
                        remainingTargets++;
                        break;
                    case 3:
                        spawnPositions.Add(position);
                        break;
                }
            }
        }

        if (spawnPositions.Count > 0) {
            playerGameObject.transform.position = spawnPositions[Random.Range(0, spawnPositions.Count)];
        }

        var newBoardSize = new Vector3(res.x, 1, res.y) * tileScale;
        TransitionDriver.InitiateTransition(
            loadoutRiseTime,
            loadoutRiseCurve,
            newBoardSize,
            boardController
        );
    }

    private void UnloadEncounter() {
        playerGameObject.SetActive(false);
        Destroy(loadoutParent);
    }

    public void TargetEliminated() {
        remainingTargets--;

        if (remainingTargets == 0) {
            UnloadEncounter();
            onEncounterCompleted.Invoke();
        }
    }

    [System.Serializable]
    public class ShooterEnvLoadout {
        public int[][] blocks = new int[][] {
            new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            new int[] { 1, 0, 0, 0, 2, 2, 0, 0, 0, 1},
            new int[] { 1, 0, 0, 0, 2, 2, 0, 0, 0, 1},
            new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            new int[] { 1, 0, 3, 0, 0, 0, 0, 0, 0, 1},
            new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
        };
    }

    private class EncounterBoardController : ITransitionPassenger<Vector3> {
        
        private readonly Transform plane;
        private readonly Transform[] walls;
        
        public EncounterBoardController(Transform tf) {
            plane = tf.GetChild(0);
            walls = new Transform[] { 
                tf.GetChild(1),
                tf.GetChild(2),
                tf.GetChild(3),
                tf.GetChild(4)
            };
        }

        public Vector3 GetTransitionValue()
        {
            return plane.localScale * 10;
        }

        public void SetTransitionValue(Vector3 size) {
            plane.localScale = size * 0.1f;

            walls[0].localPosition = size.z * 0.5f * Vector3.back;
            walls[0].localScale = new Vector3(size.x + 0.1f, 0.1f, 0.1f);

            walls[1].localPosition = size.z * 0.5f * Vector3.forward;
            walls[1].localScale = new Vector3(size.x + 0.1f, 0.1f, 0.1f);

            walls[2].localPosition = size.x * 0.5f * Vector3.right;
            walls[2].localScale = new Vector3(size.z + 0.1f, 0.1f, 0.1f);

            walls[3].localPosition = size.x * 0.5f * Vector3.left;
            walls[3].localScale = new Vector3(size.z + 0.1f, 0.1f, 0.1f);
        }
    }
}
