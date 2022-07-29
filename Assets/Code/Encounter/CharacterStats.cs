 using System.Collections;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private int hitpoints = 1;
    [SerializeField] private float shotsPerSecond = 1;
    [SerializeField] private float characterSpeed = 1;

    public IEnumerator RollCharacterStats(System.Action callback) {
        var throwPos = Camera.main.transform.position;
        var throwForward = Camera.main.transform.forward;
        var throwRight = Camera.main.transform.right;
        
        hitpoints = -1;
        shotsPerSecond = -1;
        characterSpeed = -1;

        DieThrower.main.ThrowD6(throwPos - throwRight * 2, throwForward, SetHP, Color.red * 2);
        DieThrower.main.ThrowD6(throwPos, throwForward, SetSPS, Color.blue * 2);
        DieThrower.main.ThrowD6(throwPos + throwRight * 2, throwForward, SetCS, Color.green * 2);

        while (hitpoints == -1 || shotsPerSecond == -1 || characterSpeed == -1) {
            yield return null;
        }

        callback?.Invoke();
    }
    
    private void SetHP (int value) => hitpoints = value;
    private void SetSPS (int value) => shotsPerSecond = value;
    private void SetCS (int value) => characterSpeed = value;
    public int GetHP() => hitpoints;
    public float GetSPS() => shotsPerSecond;
    public float GetCS() => characterSpeed;
}
