using UnityEngine;
using UnityEngine.Events;

public class CharacterHealth : MonoBehaviour, IEncounterEventListener
{
    private int hitPoints;
    public int HitPoints { get { return hitPoints; } }
    [SerializeField] private float hitImmunityTime;
    public UnityEvent onDeath; 
    private float lastTimeDamaged;

    public void Damage() {

        if (Time.time - lastTimeDamaged < hitImmunityTime) return;

        hitPoints--;
        lastTimeDamaged = Time.time;

        if (hitPoints <= 0) {
            onDeath.Invoke();
        }
    }

    public void DestroyGameObject() {
        Destroy(gameObject);
    }

    public void OnEncounterEvent(EncounterEventType eventType)
    {
        if (eventType == EncounterEventType.Begin) {
            var stats = GetComponent<CharacterStats>();
            if (stats) {
                hitPoints = stats.GetHP();
            }
        }
    }
}
