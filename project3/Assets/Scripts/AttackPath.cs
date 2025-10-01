// AttackPath.cs (world/local coordinates)
using UnityEngine;

[CreateAssetMenu(fileName = "AttackPath", menuName = "Boss/Attack Path (World)")]
public class AttackPath : ScriptableObject
{
    public enum SpaceMode { World, LocalToReference }

    [System.Serializable]
    public struct Segment
    {
        [Tooltip("Start position in chosen space")]
        public Vector2 startPos;
        [Tooltip("End position in chosen space")]
        public Vector2 endPos;

        [Tooltip("If >0, use exact duration (sec). Otherwise uses speed.")]
        public float duration;
        [Tooltip("Units/sec if duration == 0.")]
        public float speed;

        [Tooltip("Wait after reaching end (sec).")]
        public float postWait;

        public float smoothstep;
    }

    [Header("Playback")]
    public float preTelegraph = 0.25f;
    public SpaceMode space = SpaceMode.World;

    public Segment[] segments;

    public static float Ease(float t, float power)
    {
        // power = 2 → quadratic ease, 3 → cubic ease, etc.
        // Big powers exaggerate acceleration/deceleration.
        t = Mathf.Clamp01(t);
        return Mathf.Pow(t, power) / (Mathf.Pow(t, power) + Mathf.Pow(1f - t, power));
    }
}
