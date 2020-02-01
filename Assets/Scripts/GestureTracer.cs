using System.Collections.Generic;
using UnityEngine;

public class GestureTracer {
    private float sampleDistance;
    private List<Vector2> points = new List<Vector2>();
    public GestureTracer(float sampleDistance)
    {
        this.sampleDistance = sampleDistance;
    }

    public void Sampling(Vector2 point)
    {

    }
}