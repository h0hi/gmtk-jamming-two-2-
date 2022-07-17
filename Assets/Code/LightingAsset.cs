using UnityEngine;

[CreateAssetMenu]
public class LightingAsset : ScriptableObject
{
    public int temperature;
    [Range(0, 1)]
    public float chromaticAbberationIntensity;
    public float sunIntensity;
}
