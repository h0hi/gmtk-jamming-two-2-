using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PortalCollider : MonoBehaviour, IEncounterEventListener
{
    private readonly static HashSet<PortalCollider> portals = new ();
    private readonly List<Collider> ignoredColliders = new ();

    public void OnEncounterEvent(EncounterEventType eventType)
    {
        if (eventType == EncounterEventType.Load) {
            portals.Add(this);
        }
        else if (eventType == EncounterEventType.End) {
            portals.Remove(this);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponentInParent<Rigidbody>() != null && !ignoredColliders.Contains(other)) {

            if (portals.Count == 1) return;

            var randomlyChosenPortal = this;
            while (randomlyChosenPortal == this) {
                Random.InitState(System.DateTime.Now.GetHashCode());
                randomlyChosenPortal = portals.ElementAt(Random.Range(0, portals.Count));
            }
            
            other.transform.parent.position = randomlyChosenPortal.transform.position;
            foreach (var item in other.transform.parent.GetComponentsInChildren<Collider>()) {
                randomlyChosenPortal.ignoredColliders.Add(item);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        ignoredColliders.Remove(other);
    }

    /*
    private void SpawnDuplicate(GameObject original) {
        var duplicate = Instantiate(original, transform.position, original.transform.rotation, original.transform.parent);
        foreach (var item in duplicate.GetComponentsInChildren<Collider>()) {
            ignoredColliders.Add(item);
        }
    }

    private void Consume(GameObject original) {
        Destroy(original);
    }
    */
}
