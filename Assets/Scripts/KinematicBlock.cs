using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class KinematicBlock : MonoBehaviour
{
    private Rigidbody body;
    private BlockMapVisualizer visualizer;
    private List<Collider> colliders = new List<Collider>();

    public void Initialize(BlockMapVisualizer visualizer,  Block block, BlockMaterial m)
    {
        GetComponent<MeshRenderer>().sharedMaterial = m.MaterialPrefab;
        this.visualizer = visualizer;
        for (int x = 0; x < block.Width; x += 1)
        {
            for (int y = 0; y < block.Height; y += 1)
            {
                if (block.IsFieldSet(x, y))
                {
                    var box = gameObject.AddComponent<BoxCollider>();
                    box.center = new Vector3(x + .5f - block.Width, -y - .5f, 0);
                    box.size = Vector3.one * 0.98f;
                    colliders.Add(box);
                }
            }
        }
        body = gameObject.AddComponent<Rigidbody>();
        body.isKinematic = true;
    }

    public void Activate(Lean.Touch.LeanFinger leanFinger, RaycastHit hit)
    {
        if (!body.isKinematic)
        {
            StartCoroutine(HandleFinger(leanFinger, hit));
            return;
        }
        body.isKinematic = false;
        body.AddForceAtPosition(Vector3.forward * visualizer.ActivateVeclocity, transform.position + Random.insideUnitSphere * .04f, ForceMode.VelocityChange);
        StartCoroutine(PushOutRoutine());
    }

    private IEnumerator HandleFinger(Lean.Touch.LeanFinger leanFinger, RaycastHit hit)
    {
        Plane plane = new Plane(Vector3.forward, hit.point);
        var localPos = transform.InverseTransformPoint(hit.point);
        body.isKinematic = true;
        while (!leanFinger.Up)
        {
            Ray ray = visualizer.WorldCamera.ScreenPointToRay(leanFinger.ScreenPosition);
            if (plane.Raycast(ray, out float distance))
            {
                var proj = ray.GetPoint(distance);
                var currentAttach = transform.TransformPoint(localPos);
                body.MovePosition((proj - currentAttach) + transform.position);
            }

            yield return new WaitForEndOfFrame();
        }
        body.isKinematic = false;
    }

    private IEnumerator PushOutRoutine()
    {
        foreach (var collider in colliders) collider.enabled = false;
        for (int i=0;i<5;i+=1)
            yield return new WaitForFixedUpdate();
        foreach (var collider in colliders) collider.enabled = true;

    }
}