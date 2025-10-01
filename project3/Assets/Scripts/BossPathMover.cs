using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossPathMover : MonoBehaviour
{
    public AttackPath[] patterns;
    public bool loopPatterns = true;
    public float restBetweenPatterns = 0.5f;

    public Transform spaceReference;

    public bool freezeRotation = true;

    public enum ApproachMode { Snap, GlideByDuration, GlideBySpeed, ContinueFromCurrent }
    public ApproachMode approachMode = ApproachMode.GlideByDuration;

    public float approachDuration = 0.25f;
    public float approachSpeed = 12f;
    public float approachSmooth = 0.35f;
    public float approachWaitAfter = 0f;
    public bool damageDuringApproach = true;
    public float startSnapEpsilon = 0.01f;

    Rigidbody2D rb;
    int patternIndex;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (freezeRotation) rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void OnEnable()
    {
        StopAllCoroutines();
        if (patterns != null && patterns.Length > 0)
            StartCoroutine(RunPatterns());
    }

    IEnumerator RunPatterns()
    {
        if (patterns == null || patterns.Length == 0) yield break;

        do
        {
            var p = patterns[patternIndex];
            if (p != null)
                yield return StartCoroutine(PlayPath(p));

            patternIndex = (patternIndex + 1) % patterns.Length;

            if (restBetweenPatterns > 0f)
                yield return new WaitForSeconds(restBetweenPatterns);

        } while (loopPatterns || patternIndex != 0);
    }

    IEnumerator PlayPath(AttackPath path)
    {
        if (path.segments == null || path.segments.Length == 0) yield break;

        if (path.preTelegraph > 0f)
            yield return new WaitForSeconds(path.preTelegraph);

        Vector2 current = rb.position;

        for (int i = 0; i < path.segments.Length; i++)
        {
            var seg = path.segments[i];
            Vector2 segStart = Resolve(seg.startPos, path.space);
            Vector2 segEnd   = Resolve(seg.endPos,   path.space);

            float distToStart = Vector2.Distance(current, segStart);
            if (distToStart > startSnapEpsilon)
            {
                switch (approachMode)
                {
                    case ApproachMode.Snap:
                        rb.position = segStart;
                        break;

                    case ApproachMode.ContinueFromCurrent:
                        segStart = current;
                        break;

                    case ApproachMode.GlideByDuration:
                        yield return StartCoroutine(Glide(rb, current, segStart, approachDuration, approachSmooth, damageDuringApproach));
                        break;

                    case ApproachMode.GlideBySpeed:
                        float dur = distToStart / Mathf.Max(0.0001f, approachSpeed);
                        yield return StartCoroutine(Glide(rb, current, segStart, dur, approachSmooth, damageDuringApproach));
                        break;
                }

                if (approachWaitAfter > 0f && approachMode != ApproachMode.ContinueFromCurrent)
                    yield return new WaitForSeconds(approachWaitAfter);

                current = rb.position;
            }

            float distance = Vector2.Distance(segStart, segEnd);
            float duration = seg.duration > 0f
                ? seg.duration
                : (seg.speed > 0f ? distance / seg.speed : 0.5f);

            if (duration <= 0f)
            {
                rb.position = segEnd;
            }
            else
            {
                yield return StartCoroutine(Glide(rb, segStart, segEnd, duration, seg.smoothstep, true));
            }

            current = rb.position;

            if (seg.postWait > 0f)
                yield return new WaitForSeconds(seg.postWait);
        }
    }

    Vector2 Resolve(Vector2 p, AttackPath.SpaceMode space)
    {
        if (space == AttackPath.SpaceMode.World || spaceReference == null) return p;
        return (Vector2)spaceReference.TransformPoint(p);
    }

    IEnumerator Glide(Rigidbody2D body, Vector2 a, Vector2 b, float duration, float smooth, bool damageOn)
    {
        float elapsed = 0f;
        var wait = new WaitForFixedUpdate();

        while (elapsed < duration)
        {
            elapsed += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = AttackPath.Ease(t, smooth);
            Vector2 pos = Vector2.LerpUnclamped(a, b, t);
            body.MovePosition(pos);
            yield return wait;
        }

        body.MovePosition(b);
    }

    void OnDrawGizmosSelected()
    {
        if (patterns == null) return;
        Gizmos.color = Color.cyan;
        foreach (var path in patterns)
        {
            if (path == null || path.segments == null) continue;
            foreach (var seg in path.segments)
            {
                Vector3 a = Resolve(seg.startPos, path.space);
                Vector3 b = Resolve(seg.endPos,   path.space);
                a.z = b.z = 0f;
                Gizmos.DrawLine(a, b);
                Gizmos.DrawSphere(a, 0.05f);
                Gizmos.DrawSphere(b, 0.05f);
            }
        }
    }
}
