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

    public void Initialize(BlockMapVisualizer visualizer, Block block, BlockMaterial m, PhysicMaterial blocksMaterial)
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
                    box.size = Vector3.one * 0.95f;
                    colliders.Add(box);
                    box.sharedMaterial = blocksMaterial;
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
        GestureTracer tracer = new GestureTracer(.1f, 6);

        SetCollidersEnabled(false);
        Plane plane = new Plane(Vector3.forward, hit.point);
        Plane projPlane = new Plane(Vector3.forward, visualizer.MovePlanePoint);
        var localPos = transform.InverseTransformPoint(hit.point);
        body.isKinematic = true;
        Quaternion targetRotation = Quaternion.identity;
        while (!leanFinger.Up)
        {
if (Input.GetKeyDown(KeyCode.A)) print("A");
            Ray rayTouch = visualizer.WorldCamera.ScreenPointToRay(leanFinger.ScreenPosition);
            if (plane.Raycast(rayTouch, out float distancePlane) && projPlane.Raycast(rayTouch, out float distanceProjPlane))
            {
                float damp = Lean.Touch.LeanTouch.GetDampenFactor(visualizer.SnapBackDampening, Time.deltaTime);
                var projA = rayTouch.GetPoint(distancePlane);
                var projB = rayTouch.GetPoint(distanceProjPlane);
                tracer.Sampling(projB);
                switch (tracer.GetGesture())
                {
                    case Gesture.RotateRight:
                        targetRotation = targetRotation * Quaternion.Euler(0,0,90);
                        tracer.Reset();
                        break;
                    case Gesture.RotateLeft:
                        targetRotation = targetRotation * Quaternion.Euler(0,0,-90);
                        tracer.Reset();
                        break;
                    case Gesture.MirrorHorizontal:
                        targetRotation = targetRotation * Quaternion.Euler(0,180,0);
                        tracer.Reset();
                        break;
                    case Gesture.MirrorVertical:
                        targetRotation = targetRotation * Quaternion.Euler(180,0,0);
                        tracer.Reset();
                        break;

                }

                var factor = Mathf.InverseLerp(0.5f, 1.5f, projB.y);
                var proj = Vector3.Lerp(projA, projB, factor);
                var currentAttach = Vector3.Lerp(transform.TransformPoint(localPos), body.worldCenterOfMass, factor);
                Vector3 nextPosition = Vector3.MoveTowards(body.position, (proj - currentAttach) + body.position, visualizer.MoveBackPerSecond * Time.deltaTime);
                Vector3 snapPosition = Vector3.Lerp(nextPosition, visualizer.SnapPoint(nextPosition), factor * damp);
                body.MovePosition(snapPosition);
                body.MoveRotation(Quaternion.RotateTowards(body.rotation, targetRotation, visualizer.RotateBackPerSecond * Time.deltaTime * factor));

            }

            yield return null;
        }
        SetCollidersEnabled(true);

        if (visualizer.IsFitting(this))
        {
            Plane playPlane = new Plane(Vector3.forward, visualizer.PlayPlanePoint);
            do
            {
                float damp = Lean.Touch.LeanTouch.GetDampenFactor(visualizer.SnapBackDampening, Time.deltaTime);
                var planePos = playPlane.ClosestPointOnPlane(body.position);
                Vector3 position = Vector3.Lerp(body.position, planePos, damp);
                Vector3 snapPosition = Vector3.Lerp(position, visualizer.SnapPoint(position), damp);

                body.MovePosition(snapPosition);
                body.MoveRotation(Quaternion.RotateTowards(body.rotation, targetRotation, visualizer.RotateBackPerSecond * damp));

                var dist = playPlane.GetDistanceToPoint(body.position);
                yield return null;
            } while (Mathf.Abs(playPlane.GetDistanceToPoint(body.position)) > 0.05f);
            yield break;
        }
        body.isKinematic = false;
    }

    private IEnumerator PushOutRoutine()
    {
        SetCollidersEnabled(false);
        for (int i = 0; i < 5; i += 1)
            yield return new WaitForFixedUpdate();
        SetCollidersEnabled(true);

    }

    private void SetCollidersEnabled(bool b)
    {
        foreach (var collider in colliders) collider.enabled = b;
    }
}