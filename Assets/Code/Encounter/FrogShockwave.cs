using UnityEngine;

public class FrogShockwave : MonoBehaviour
{
    [SerializeField] private float secondsLife;
    [SerializeField] private Vector3 endScale;
    private float secondsAlive;

    private void Update() {
        secondsAlive += Time.deltaTime;
        var t = secondsAlive / secondsLife;
        if (t > 1) Destroy(gameObject);

        transform.localScale = Vector3.Lerp(Vector3.one, endScale, t);
    }
}
