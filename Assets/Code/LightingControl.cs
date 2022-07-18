using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightingControl : MonoBehaviour, ITransitionPassenger<LightingData>
{
    public static LightingControl main;

    [SerializeField] private LightingSettings sceneLightingSettings;

    private Light sunLight;
    private ChromaticAberration globalVolumeChromaticAbberation;

    private void Awake() {
        if (main != null) {
            Destroy(this);
        }

        main = this;
    }

    private void Start() {
        sunLight = GetComponentInChildren<Light>();
        var globalVolume = GetComponentInChildren<Volume>();

        if (globalVolume.profile.TryGet<ChromaticAberration>(out var ca)) {
            globalVolumeChromaticAbberation = ca;
        }
    }

    public LightingData GetTransitionValue() { 
        return new LightingData(
            main.sunLight.colorTemperature,
            main.sunLight.intensity,
            globalVolumeChromaticAbberation.intensity.value
        );
    }

    public void SetTransitionValue(LightingData value)
    {
        main.sunLight.colorTemperature = value.temperature;
        main.sunLight.intensity = value.sunIntensity;
        globalVolumeChromaticAbberation.intensity.value = value.chromaticAberrationIntensity;
    }
}
