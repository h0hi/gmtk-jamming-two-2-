using System.Collections;
using UnityEngine;

public class TransitionDriver : MonoBehaviour
{
    private static TransitionDriver main;

    private void Awake() {
        if (main != null) {
            Destroy(this);
        }

        main = this;
    }

    public static void InitiateTransition<T>(AnimationCurve curve, T end, ITransitionPassenger<T> passenger, System.Action callback) {
        main.StartCoroutine(main.TransitionCoroutine(curve, end, passenger, callback));
    }

    private IEnumerator TransitionCoroutine<T>(AnimationCurve curve, T end, ITransitionPassenger<T> passenger, System.Action callback) {
        var startTime = Time.time;
        var duration = curve.keys[curve.length - 1].time;
        
        var start = passenger.GetTransitionValue();
        var flags = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public;
        var lerpMethod = typeof(T).GetMethod("Lerp", flags);

        if (typeof(T) == typeof(float)) {
            lerpMethod = typeof(Mathf).GetMethod("Lerp", flags);
        }
        if (lerpMethod == null) {
            Debug.LogError("No static Lerp method found for type " + typeof(T));
            yield break;
        }

        while (Time.time - startTime < duration) {
            var f = curve.Evaluate(Time.time - startTime);
            T value = (T) lerpMethod.Invoke(null, new object[] { start, end, f });
            passenger.SetTransitionValue(value);
            yield return null;
        }

        passenger.SetTransitionValue(end);
        callback?.Invoke();
    }
}

public interface ITransitionPassenger<T> {
    T GetTransitionValue();
    void SetTransitionValue(T value);
}
