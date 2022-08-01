using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DieThrower : MonoBehaviour
{
    public static DieThrower main;
    
    [SerializeField] private float dieThrowForce;
    [SerializeField] private float dieThrowMaxInitialTorque;
    
    private GameObject d6prefab;

    private void Awake() {
        if (main != null) {
            Destroy(this);
        }

        main = this;
    }

    private void Start() {
        d6prefab = AssetLoader.LoadAsset<GameObject>("Assets/Prefabs/die.prefab");
    }

    public void ThrowD6(Vector3 position, Vector3 throwDirection, UnityAction<int> callback, Color color) {
        var dice = Instantiate(d6prefab, position, Quaternion.FromToRotation(Vector3.forward, throwDirection), null);
        dice.GetComponent<DiceBehaviour>().onLand.AddListener(callback);
        dice.GetComponent<Rigidbody>().AddRelativeTorque(UnityEngine.Random.onUnitSphere * dieThrowMaxInitialTorque, ForceMode.Impulse);
        dice.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * dieThrowForce);
        dice.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", color);
    }
}
