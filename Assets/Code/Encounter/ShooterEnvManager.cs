using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShooterEnvManager : MonoBehaviour
{
    [SerializeField] private float tileScale;
    [SerializeField] private float loadoutRiseTime;
    [SerializeField] private AnimationCurve loadoutRiseCurve;

    private EncounterBoardController boardController;
    private int remainingTargets;
    private GameObject loadoutParent;
    private GameObject playerGameObject;
    private string selectedLoadoutName;

    public UnityEvent onEncounterCompleted;

    private void Start() {

        boardController = new EncounterBoardController(transform);

        playerGameObject = FindObjectOfType<CharacterControl>().gameObject;
        playerGameObject.SetActive(false);
    }

    public void LoadEncounter(string name) {
        var stats = playerGameObject.GetComponent<CharacterStats>();
        StartCoroutine(stats.RollCharacterStats(CreateLoadout));
        selectedLoadoutName = name;
    }

    private void CreateLoadout() {

        var loadout = Resources.Load<EncounterAsset>("Encounter/Encounter Assets/" + selectedLoadoutName);

        playerGameObject.SetActive(true);
        var characterHealthDevice = playerGameObject.GetComponent<CharacterHealth>();
        if (characterHealthDevice) {
            characterHealthDevice.onDeath.AddListener(OnCharacterDeath);
        }

        remainingTargets = 0;

        loadoutParent = new GameObject("Loadout " + selectedLoadoutName);
        loadoutParent.transform.SetParent(transform);
        loadoutParent.transform.localPosition = Vector3.up * 1;

        List<Vector3> spawnPositions = new ();
        var res = new Vector2(loadout.GetHeight(), loadout.GetLength());
        var tileOffset = res * -0.5f + tileScale * 0.5f * Vector2.one;

        var indestructibleBlockPrefab = Resources.Load<GameObject>("Encounter/Indestructible Cube");
        var destructibleBlockPrefab = Resources.Load<GameObject>("Encounter/Destructible Cube");
        var enemyGenericPrefab = Resources.Load<GameObject>("Encounter/Enemy Generic");
        var turretEnemyPrefab = Resources.Load<GameObject>("Encounter/Enemy Turret");

        for (int y = 0; y < res.y; y++) {
            for (int x = 0; x < res.x; x++) {
                var position = new Vector3(x + tileOffset.x, 0, y + tileOffset.y) * tileScale;

                int tileType = loadout.GetTile(y, x);
                switch (tileType) {
                    default:
                        break;
                    case 1:
                        Instantiate(indestructibleBlockPrefab, position, Quaternion.identity, loadoutParent.transform);
                        break;
                    case 2:
                        Instantiate(destructibleBlockPrefab, position, Quaternion.identity, loadoutParent.transform);
                        break;
                    case 3:
                        spawnPositions.Add(position);
                        break;
                    case 4:
                        var enemyGo = Instantiate(enemyGenericPrefab, position, Quaternion.identity, loadoutParent.transform);
                        var enemyHealthDevice = enemyGo.GetComponent<CharacterHealth>();
                        if (enemyHealthDevice) {
                            remainingTargets++;
                            enemyHealthDevice.onDeath.AddListener(TargetEliminated);
                        }
                        break;
                    case 5:
                        var turretGo = Instantiate(turretEnemyPrefab, position, Quaternion.identity, loadoutParent.transform);
                        var turretHealthDevice = turretGo.GetComponent<CharacterHealth>();
                        if (turretHealthDevice) {
                            remainingTargets++;
                            turretHealthDevice.onDeath.AddListener(TargetEliminated);
                        }
                        break;
                }
            }
        }

        if (spawnPositions.Count > 0) {
            playerGameObject.transform.position = spawnPositions[Random.Range(0, spawnPositions.Count)];
        }

        if (remainingTargets == 0) {
            Invoke(nameof(ExitEncounter), 5 + loadoutRiseTime);
        }

        Debug.Log("Eliminate " + remainingTargets + " targets!");

        var newBoardSize = new Vector3(res.x, 1, res.y) * tileScale;
        TransitionDriver.InitiateTransition(
            loadoutRiseTime,
            loadoutRiseCurve,
            newBoardSize,
            boardController
        );
    }

    private void UnloadLoadout() {
        playerGameObject.SetActive(false);

        // cleanup pellets
        var pellets = FindObjectsOfType<PelletBehaviour>();

        foreach (var pellet in pellets) {
            if (pellet != null)
                Destroy(pellet.gameObject);
        }

        Destroy(loadoutParent);
    }

    private void OnCharacterDeath() {
        ExitEncounter();
    }

    private void TargetEliminated() {
        remainingTargets--;
        Debug.Log(remainingTargets + " targets remaining!");

        if (remainingTargets == 0) {
            ExitEncounter();
        }
    }

    public void ExitEncounter() {
        UnloadLoadout();
        onEncounterCompleted.Invoke();
    }

    private class EncounterBoardController : ITransitionPassenger<Vector3> {
        
        private readonly Transform plane;
        private readonly Transform[] walls;
        private readonly Material planeMaterial;
        
        public EncounterBoardController(Transform tf) {
            plane = tf.GetChild(0);
            planeMaterial = plane.GetComponent<MeshRenderer>().material;
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
            planeMaterial.mainTextureScale = new Vector2(size.x, size.z);

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
