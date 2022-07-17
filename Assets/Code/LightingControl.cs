using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightingControl : MonoBehaviour
{
    private static LightingControl main;

    [SerializeField] private LightingAsset[] lightingAssets;
    [SerializeField] private LightingSettings sceneLightingSettings;

    private Light sunLight;
    private Volume globalVolume;

    private void Awake() {
        if (main != null) {
            Destroy(this);
        }

        main = this;
    }

    private void Start() {
        sunLight = GetComponentInChildren<Light>();
        globalVolume = GetComponentInChildren<Volume>();
    }

    public static void ApplyLightingAsset(float transitionTime, AnimationCurve transitionCurve, int id) {
        if (id >= main.lightingAssets.Length) {
            Debug.LogError("No lighting asset with id " + id + " was found.");
        }

        var asset = main.lightingAssets[id];
        main.StartCoroutine(main.TransitionToNewSettingsCoroutine(transitionTime, transitionCurve, asset));
    }

    private IEnumerator TransitionToNewSettingsCoroutine(float transitionTime, AnimationCurve transitionCurve, LightingAsset newSettings) {
        var t = 0f;
        var startTime = Time.time;

        var tempOld = sunLight.colorTemperature;
        var intensityOld = sunLight.intensity;
        var chromaOld = 0f;
        if (main.globalVolume.profile.TryGet<ChromaticAberration>(out var chromaticAberration)) {
            chromaOld = chromaticAberration.intensity.value;
        }

        do {
            t = (Time.time - startTime) / transitionTime;
            var f = transitionCurve.Evaluate(t);

            SetParameters(
                Mathf.Lerp(tempOld, newSettings.temperature, f),
                Mathf.Lerp(intensityOld, newSettings.sunIntensity, f),
                Mathf.Lerp(chromaOld, newSettings.chromaticAbberationIntensity, f)
            );

            yield return null;

        } while(t < 1);
    }

    private void SetParameters(float temperature, float sunIntensity, float chromaticAbberationIntensity) {
        main.sunLight.colorTemperature = temperature;
        main.sunLight.intensity = sunIntensity;
        if (main.globalVolume.profile.TryGet<ChromaticAberration>(out var chromaticAberration)) {
            chromaticAberration.intensity.value = chromaticAbberationIntensity;
        }
    }
}
