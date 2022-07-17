using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ShooterEnvManager : MonoBehaviour
{
    [SerializeField] private float tileScale;
    [SerializeField] private Vector3 tileOffset;
    [SerializeField] private float loadoutRiseTime;

    private ShooterEnvLoadout[] loadouts;    
    private GameObject indestructibleBlockPrefab;
    private GameObject destructibleBlockPrefab;
    private int remainingTargets;
    private GameObject loadoutParent;
    private GameObject playerGameObject;

    public UnityEvent onEncounterCompleted;

    private void Start() {
        indestructibleBlockPrefab = Resources.Load<GameObject>("Top Down Env/Indestructible Cube");
        destructibleBlockPrefab = Resources.Load<GameObject>("Top Down Env/Destructible Cube");
        loadouts = new ShooterEnvLoadout[] {
            new ShooterEnvLoadout()
        };

        playerGameObject = FindObjectOfType<CharacterControl>().gameObject;
        playerGameObject.SetActive(false);
    }

    public void LoadEncounter(int i) {
        var loadout = loadouts[i];
        playerGameObject.SetActive(true);

        remainingTargets = 0;

        loadoutParent = new GameObject("Loadout " + i);
        loadoutParent.transform.SetParent(transform);
        loadoutParent.transform.localPosition = Vector3.up * 1;

        for (int y = 0; y < loadout.blocks.Length; y++) {
            for (int x = 0; x < loadout.blocks[y].Length; x++) {
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
                }
            }
        }
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
            new int[] { 1, 2, 0, 0, 0, 0, 0, 0, 0, 1},
            new int[] { 1, 2, 2, 0, 0, 0, 0, 0, 0, 1},
            new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            new int[] { 1, 0, 0, 0, 2, 2, 0, 0, 0, 1},
            new int[] { 1, 0, 0, 0, 2, 2, 0, 0, 0, 1},
            new int[] { 1, 0, 0, 0, 0, 0, 0, 2, 2, 1},
            new int[] { 1, 0, 0, 0, 0, 0, 0, 2, 0, 1},
            new int[] { 1, 0, 0, 0, 0, 0, 0, 2, 2, 1},
            new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
        };
    }
}
