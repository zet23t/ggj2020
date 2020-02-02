using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Gesture
{
    Nothing,
    RotateLeft,
    RotateRight,
    MirrorHorizontal,
    MirrorVertical
}

public class GestureTracer
{
    private float sampleDistance;
    private float maxTraceDistance;
    private List<Vector2> points = new List<Vector2>();

    public GestureTracer(float sampleDistance, float maxTraceDistance)
    {
        this.maxTraceDistance = maxTraceDistance;
        this.sampleDistance = sampleDistance;
    }

    public Gesture GetGesture()
    {
        var byKey = DetectByKey();
        if (byKey != Gesture.Nothing)
        {
            return byKey;
        }

        return DetectByTouch();
    }

    private Gesture DetectByKey()
    {
        var rotateClockwiseKeys = new List<KeyCode> {KeyCode.D, KeyCode.RightArrow};
        var rotateAntiClockwiseKey = new List<KeyCode> {KeyCode.A, KeyCode.LeftArrow};
        var mirrorKey = new List<KeyCode> {KeyCode.Space};

        bool anyClockwiseKeyDown = rotateClockwiseKeys.Any(Input.GetKeyUp);
        bool anyAntiClockwiseKeyDown = rotateAntiClockwiseKey.Any(Input.GetKeyUp);
        bool anyMirrorKeyDown = mirrorKey.Any(Input.GetKeyUp);

        if (anyClockwiseKeyDown)
        {
            return Gesture.RotateRight;
        }
        else if (anyAntiClockwiseKeyDown)
        {
            return Gesture.RotateLeft;
        }
        else if (anyMirrorKeyDown)
        {
            return Gesture.MirrorHorizontal;
        }

        return Gesture.Nothing;
    }

    private Gesture DetectByTouch()
    {
        if (points.Count < 5)
        {
            return Gesture.Nothing;
        }

        Vector2 centerOfMass = Vector3.zero;
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        for (int i = 1; i < points.Count; i += 1)
        {
            min = Vector2.Min(min, points[i]);
            max = Vector2.Max(max, points[i]);
            centerOfMass += points[i];
        }

        centerOfMass /= points.Count;
        Vector2 center = (min + max) * .5f;
        // is circle ?
        float prevDist = 0f;
        float circleWalkDist = 0f;
        float circleWalkAngle = 0f;
        float avgCenterDist = 0f;
        int circleCount = 0;
        for (int i = points.Count - 1; i >= 0; i -= 1)
        {
            Vector2 p1 = points[i];
            var centerDist = Vector3.Distance(p1, center);
            var diff = centerDist - prevDist;
            prevDist = centerDist;
            if (i == points.Count - 1)
            {
                continue;
            }

            if (diff > sampleDistance * .95f)
            {
                break;
            }

            avgCenterDist += centerDist;
            circleCount += 1;
            Vector2 p2 = points[i + 1];
            var pdist = Vector3.Distance(p1, p2);
            var angle = Vector2.SignedAngle(p1 - center, p2 - center);
            circleWalkAngle += angle;
            circleWalkDist += pdist;

            var a = p2;
            var b = p1;
            Debug.DrawLine(a, b);
            Debug.DrawLine(b, center, new Color(1, 0, 0, .25f));
        }

        if (circleCount > 0)
        {
            avgCenterDist /= circleCount;
            // Debug.Log($"{avgCenterDist} : {circleWalkAngle}");
            if (avgCenterDist > sampleDistance * 2)
            {
                if (circleWalkAngle > 270)
                {
                    return Gesture.RotateLeft;
                }

                if (circleWalkAngle < -270)
                {
                    return Gesture.RotateRight;
                }
            }
        }
        float width = max.x - min.x;
        float height = max.y - min.y;
        float totalLength = CalcTotalLength();
        if (width > height * 3 && totalLength > width * 3)
        {
            return Gesture.MirrorHorizontal;
        }
        if (width * 3 < height && totalLength > height * 3)
        {
            return Gesture.MirrorVertical;
        }

        return Gesture.Nothing;
    }

    public void Reset()
    {
        points.Clear();
    }

    private float CalcTotalLength()
    {
        float len = 0;
        for (int i = 1; i < points.Count; i += 1)
        {
            var a = points[i - 1];
            var b = points[i];
            len += Vector3.Distance(a, b);
        }

        return len;
    }

    public void Sampling(Vector2 point)
    {
        if (points.Count == 0 || Vector2.Distance(point, points[points.Count - 1]) > sampleDistance)
        {
            points.Add(point);
        }

        while (CalcTotalLength() > maxTraceDistance)
        {
            points.RemoveAt(0);
        }
    }
}