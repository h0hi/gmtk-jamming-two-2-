using UnityEngine;

public class Shield : MonoBehaviour, IEncounterEventListener
{
    [SerializeField] private CharacterHealth[] disableConditions;

    private Material mat;
    private const float scrollSpeed = 1;

    public void OnEncounterEvent(EncounterEventType eventType)
    {
        if (eventType == EncounterEventType.Load) {
            foreach (var item in disableConditions) {
                item.onDeath.AddListener(CheckDisable);
            }

            if (disableConditions.Length == 0) {
                Disable();
            }

            mat = GetComponentInChildren<MeshRenderer>().material;
        }
    }

    private void Update() {
        mat.mainTextureOffset = Time.time * scrollSpeed * Vector2.up;
    }

    private void CheckDisable() {
        foreach (var item in disableConditions) {
            if (item != null && item.HitPoints > 0) return;
        }

        Disable();
    }

    private void Disable() => Destroy(gameObject);
}
