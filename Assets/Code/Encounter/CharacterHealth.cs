using UnityEngine;
using UnityEngine.Events;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private int hitPoints;
    [SerializeField] private float hitImmunityTime;
    public UnityEvent onDeath; 
    private float lastTimeDamaged;
    
    private void OnEnable() {
        var stats = GetComponent<CharacterStats>();
        if (stats) {
            hitPoints = stats.GetHP();
        }
    }    

    public void Damage() {

        if (Time.time - lastTimeDamaged < hitImmunityTime) return;

        hitPoints--;
        lastTimeDamaged = Time.time;

        if (hitPoints <= 0) {
            onDeath.Invoke();
        }
    }
}
