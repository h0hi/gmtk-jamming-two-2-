using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
        globalVolume = GetComponent<Volume>();
    }

    public static void ApplyLightingAsset(int id) {
        if (id >= main.lightingAssets.Length) {
            Debug.LogError("No lighting asset with id " + id + " was found.");
        }

        var asset = main.lightingAssets[id];
        main.sunLight.colorTemperature = asset.temperature;
    }
}
