using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PortalCollider : MonoBehaviour, IEncounterEventListener
{
    [SerializeField] private int gatePairId;
    private readonly static Dictionary<int, HashSet<PortalCollider>> portals = new ();
    private readonly List<Collider> ignoredColliders = new ();
    private static Dictionary<int, Color> portalColors;

    public void OnEncounterEvent(EncounterEventType eventType)
    { 
        switch (eventType) {
            case EncounterEventType.Load:
                if (!portals.TryGetValue(gatePairId, out var portalSet)) {
                    portalSet = new HashSet<PortalCollider>();
                    portals.Add(gatePairId, portalSet);
                }

                portalSet.Add(this);
                break;
            case EncounterEventType.Begin:

                if (portalColors == null) {
                    GeneratePortalColorsArray();
                }

                var portalmat = GetComponent<MeshRenderer>().material;
                portalmat.color = portalColors[gatePairId];
                portalmat.SetColor("_EmissionColor", portalColors[gatePairId]);
                break;
            case EncounterEventType.End:
                if (portals.TryGetValue(gatePairId, out portalSet)) {
                    portalSet.Remove(this);
                    if (portalSet.Count == 0) {
                        portals.Remove(gatePairId);
                    }
                }
                portalColors = null;
                break;
        }
    }

    private void OnTriggerEnter(Collider other) {
        var rb = other.GetComponentInParent<Rigidbody>();
        if (rb != null && !ignoredColliders.Contains(other)) {

            if (portals[gatePairId].Count == 1) return;

            var randomlyChosenPortal = this;
            while (randomlyChosenPortal == this) {
                Random.InitState(System.DateTime.Now.GetHashCode());
                randomlyChosenPortal = portals[gatePairId].ElementAt(Random.Range(0, portals[gatePairId].Count));
            }
            
            var objectParent = other.transform.parent;
            objectParent.position = randomlyChosenPortal.transform.position;

            // adjust rotation 
            var parentForwardProjected = new Vector3(transform.parent.forward.x, 0, transform.parent.forward.z).normalized;
            var objectForwardProjected = new Vector3(objectParent.forward.x, 0, objectParent.forward.z).normalized;
            var destinationForwardProjected = new Vector3(randomlyChosenPortal.transform.parent.forward.x, 0, randomlyChosenPortal.transform.parent.forward.z).normalized;
            var angleObjectBefore = Mathf.Atan2(objectForwardProjected.x, objectForwardProjected.z);
            var deltaOriginal = angleObjectBefore - Mathf.Atan2(parentForwardProjected.x, parentForwardProjected.z);
            var angleObject = deltaOriginal + Mathf.Atan2(destinationForwardProjected.x, destinationForwardProjected.z) + Mathf.PI;
            objectParent.eulerAngles = new Vector3(0, angleObject * Mathf.Rad2Deg, 0);
            rb.velocity = Quaternion.Euler(0, angleObject * Mathf.Rad2Deg, 0) * Quaternion.Inverse(Quaternion.Euler(0, angleObjectBefore * Mathf.Rad2Deg, 0)) * rb.velocity;

            foreach (var item in other.transform.parent.GetComponentsInChildren<Collider>()) {
                randomlyChosenPortal.ignoredColliders.Add(item);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        ignoredColliders.Remove(other);
    }

    private static void GeneratePortalColorsArray() {
        var len = portals.Count;
        float i = 0;
        portalColors = new ();
        
        foreach (var key in portals.Keys) {
            portalColors[key] = Color.HSVToRGB(i / len, 0.8f, 0.6f);
            i++;
        }
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
