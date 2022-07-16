using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DiceRollSpot : MonoBehaviour
{
    public GameObject dicePrefab;

    private void Start() {
        dicePrefab = Resources.Load<GameObject>("die");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            RollDice();
        }
    }

    private void RollDice() {
        var dice = Instantiate(dicePrefab, transform.position, Random.rotation);
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Collider>().enabled = false;
        dice.GetComponent<DiceBehaviour>().onLand.AddListener(PrintSide);
    }

    public void PrintSide(int side) {
        Debug.Log("Die landed on " + side);
    }
}
