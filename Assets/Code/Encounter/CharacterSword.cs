using System.Collections;
using UnityEngine;

public class CharacterSword : MonoBehaviour, ITransitionPassenger<float>
{
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private float halfArcDegrees;
    [SerializeField] private float swingRadius;
    [SerializeField] private float swingDuration;

    private GameObject swordInstance;
    private bool swinging;

    public float GetTransitionValue() => -halfArcDegrees;

    public void SetTransitionValue(float angleDegrees)
    {   
        if (swordInstance == null) return;
        if (swinging) {
            var angleRadians = angleDegrees * Mathf.Deg2Rad;
            swordInstance.transform.localPosition = new Vector3(Mathf.Sin(angleRadians), 0, Mathf.Cos(angleRadians)) * swingRadius;
            swordInstance.transform.localEulerAngles = new Vector3(0, angleDegrees, 0);
        }
    }

    public void DoSwing() {
        if (!swinging) {
            swinging = true;
            swordInstance = Instantiate(swordPrefab, transform);
            swordInstance.layer = gameObject.layer;

            foreach (Transform child in swordInstance.transform) {
                child.gameObject.layer = gameObject.layer;
            }

            TransitionDriver.InitiateTransition(
                AnimationCurve.Linear(0, 0, swingDuration, 1),
                halfArcDegrees,
                this,
                SwingEndCallback
            );
        }
    }

    private void SwingEndCallback() {
        Destroy(swordInstance);
        swinging = false;
    }
}
