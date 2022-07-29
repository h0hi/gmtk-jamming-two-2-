using System.Collections.Generic;
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
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponentInParent<Rigidbody>() != null && !ignoredColliders.Contains(other)) {

            if (portals.Count == 1) return;

            foreach (var item in portals) {
                if (item != this) 
                    item.SpawnDuplicate(other.transform.parent.gameObject);
            }
            Consume(other.transform.parent.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        ignoredColliders.Remove(other);
    }

    private void SpawnDuplicate(GameObject original) {
        var duplicate = Instantiate(original, transform.position, original.transform.rotation, original.transform.parent);
        foreach (var item in duplicate.GetComponentsInChildren<Collider>()) {
            ignoredColliders.Add(item);
        }
    }

    private void Consume(GameObject original) {
        Destroy(original);
    }
}
