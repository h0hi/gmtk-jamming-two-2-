using UnityEngine;

public class BoardGraph : MonoBehaviour
{
    [SerializeField] private bool graphLooping;
    public int Length { get { return transform.childCount; } }

    public Vector3 GetStartingPosition() => transform.GetChild(0).position;

    public Transform GetTransformAtDistance(int i) {
        if (!graphLooping && i >= Length) throw new System.ArgumentOutOfRangeException();
        return transform.GetChild(graphLooping ? (i % Length) : Mathf.Min(i, Length - 1));
    }
    public Vector3 GetPointAtDistance(int i) => GetTransformAtDistance(i).position;
}
