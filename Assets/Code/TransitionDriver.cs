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

    public static void InitiateTransition<T>(float duration, AnimationCurve curve, T end, ITransitionPassenger<T> passenger) {
        main.StartCoroutine(main.TransitionCoroutine(duration, curve, end, passenger));
    }

    private IEnumerator TransitionCoroutine<T>(float duration, AnimationCurve curve, T end, ITransitionPassenger<T> passenger) {
        var startTime = Time.time;
        var t = 0f;
        
        var start = passenger.GetTransitionValue();
        var flags = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public;
        var lerpMethod = typeof(T).GetMethod("Lerp", flags);

        if (lerpMethod == null) {
            Debug.LogError("No static Lerp method found for type " + typeof(T));
            yield break;
        }

        while (t < 1) {
            t = (Time.time - startTime) / duration;
            var f = curve.Evaluate(t);
            T value = (T) lerpMethod.Invoke(null, new object[] { start, end, f });
            passenger.SetTransitionValue(value);
            yield return null;
        }
    }
}

public interface ITransitionPassenger<T> {
    T GetTransitionValue();
    void SetTransitionValue(T value);
}
