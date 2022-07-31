using UnityEngine;

[CreateAssetMenu]
public class EnemyWaves : ScriptableObject
{
    public EnemyWave[] waves;

    [System.Serializable]
    public class EnemyWave {
        public string[] enemyPrefabNames;
    }
}
