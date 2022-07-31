using UnityEngine;
using UnityEngine.Events;

public class EncounterAsset : MonoBehaviour, ITransitionPassenger<float>
{
    private int threatCount;
    private int currentWave;
    public UnityEvent onEncounterFinished = new ();
    private EnemyWaves waveAsset;

    public void Load(EnemyWaves waveAsset) {
        AnnounceEncounterEvent(EncounterEventType.Load);
        var playerGameObject = GetComponentInChildren<InputCharacterDriver>().gameObject;
        playerGameObject.GetComponent<CharacterHealth>().onDeath.AddListener(OnPlayerDeath);

        transform.localPosition = new Vector3(0, -10, 0);
        this.waveAsset = waveAsset;

        threatCount = 0;
        currentWave = -1;
    }

    public void Begin() {
        transform.localPosition = Vector3.up * 2;
        AdvanceWave();
    }

    public void AdvanceWave() {
        currentWave++;
        if (currentWave >= waveAsset.waves.Length) {
            Debug.Log("AdvanceWave -> ExitEncounter");
            ExitEncounter();
        } else {
            Debug.Log("Advancing wave...");
            SpawnWave(currentWave);
            AnnounceEncounterEvent(EncounterEventType.Begin);

            if (threatCount == 0) {
                Invoke(nameof(AdvanceWave), 3f);
            }
        }
    }

    public void SpawnWave(int i) {
        var wave = waveAsset.waves[i];

        foreach (var prefabName in wave.enemyPrefabNames) {
            var prefab = EncounterLoader.instance.GetEncounterPrefab(prefabName);
            var randomAngle = Random.Range(0, Mathf.PI * 2);
            var instance = Instantiate(prefab, transform);
            instance.transform.localPosition = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)) * 7;
            instance.transform.localEulerAngles = new Vector3(0, randomAngle * Mathf.Rad2Deg, 0);

            instance.GetComponent<CharacterHealth>().onDeath.AddListener(ThreatEliminated);
            threatCount++;
        }
    }

    private void AnnounceEncounterEvent(EncounterEventType eventType) {
        System.Array.ForEach(GetComponentsInChildren<IEncounterEventListener>(), (i) => i.OnEncounterEvent(eventType));
    }

    private void OnPlayerDeath() {
        Debug.Log("OnPlayerDeath -> ExitEncounter");
        ExitEncounter();
    }

    private void ThreatEliminated() {
        threatCount--;
        Debug.Log(threatCount + " targets remaining!");

        if (threatCount == 0) {
            AdvanceWave();
        }
    }

    private void ExitEncounter() {
        AnnounceEncounterEvent(EncounterEventType.End);
        onEncounterFinished.Invoke();
    } 

    public float GetTransitionValue()
    {
        return transform.localPosition.y;
    }

    public void SetTransitionValue(float value)
    {
        transform.localPosition = Vector3.up * value;
    }

    public Vector2 GetBoardSize() => new (transform.GetChild(0).localScale.x, transform.GetChild(0).localScale.z);
}

public enum EncounterEventType {
    Load,
    Begin,
    End
}