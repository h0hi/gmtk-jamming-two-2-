using UnityEngine;
using UnityEngine.Events;

public class EncounterAsset : MonoBehaviour, ITransitionPassenger<float>
{
    private int threatCount;
    public UnityEvent onEncounterFinished = new ();

    public void Load() {
        AnnounceEncounterEvent(EncounterEventType.Load);
        var playerGameObject = GetComponentInChildren<InputCharacterDriver>().gameObject;
        playerGameObject.GetComponent<CharacterHealth>().onDeath.AddListener(OnPlayerDeath);

        transform.localPosition = Vector3.down * 1.1f;

        threatCount = 0;
        // enumerate threats
        foreach (var threat in GetComponentsInChildren<EnemyCharacterDriver>()) {
            threatCount++;
            threat.gameObject.GetComponent<CharacterHealth>().onDeath.AddListener(ThreatEliminated);
        }
        foreach (var threat in GetComponentsInChildren<TurretCharacterDriver>()) {
            threatCount++;
            threat.gameObject.GetComponent<CharacterHealth>().onDeath.AddListener(ThreatEliminated);
        }
    }

    public void Begin() {
        transform.localPosition = Vector3.zero;
        AnnounceEncounterEvent(EncounterEventType.Begin);
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
            Debug.Log("ThreatEliminated -> ExitEncounter");
            ExitEncounter();
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

    public Vector2 GetBoardSize() => new Vector2(transform.GetChild(0).localScale.x, transform.GetChild(0).localScale.z) * 10;
}

public enum EncounterEventType {
    Load,
    Begin,
    End
}