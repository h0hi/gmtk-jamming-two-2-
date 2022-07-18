using UnityEngine;

[CreateAssetMenu]
public class LightingData
{
    public float temperature;
    [Range(0, 1)]
    public float chromaticAberrationIntensity;
    public float sunIntensity;

    public LightingData(float temperature, float sunIntensity, float chromaticAberrationIntensity) {
        this.temperature = temperature;
        this.sunIntensity = sunIntensity;
        this.chromaticAberrationIntensity = chromaticAberrationIntensity;
    }

    public static LightingData Lerp(LightingData a, LightingData b, float f) {
        return new LightingData(
            Mathf.Lerp(a.temperature, b.temperature, f),
            Mathf.Lerp(a.sunIntensity, b.sunIntensity, f),
            Mathf.Lerp(a.chromaticAberrationIntensity, b.chromaticAberrationIntensity, f)
        );
    }
}
