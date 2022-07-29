using UnityEngine;
using UnityEngine.Events;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private int hitPoints;
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
}
