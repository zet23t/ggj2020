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
    public Block block;
    private BlockOrientation orientation;

    public int BlockID;

    public static Quaternion OrientationToRotation(BlockOrientation block)
    {
        switch (block)
        {
            case BlockOrientation.O0:
                return Quaternion.Euler(0, 0, 0);
            case BlockOrientation.O90:
                return Quaternion.Euler(0, 0, -270);
            case BlockOrientation.O180:
                return Quaternion.Euler(0, 0, -180);
            case BlockOrientation.O270:
                return Quaternion.Euler(0, 0, -90);
            case BlockOrientation.M0:
                return Quaternion.Euler(0, 180, 0);
            case BlockOrientation.M90:
                return Quaternion.Euler(0, 180, 270);
            case BlockOrientation.M180:
                return Quaternion.Euler(0, 180, 180);
            case BlockOrientation.M270:
                return Quaternion.Euler(0, 180, 90);
        }
        return default;
    }

    public BlockOrientation CurrentRotationToOrientation()
    {
        var rightV3 = transform.TransformDirection(Vector3.right);
        var upV3 = transform.TransformDirection(Vector3.up);
        var right = Vector2Int.RoundToInt(rightV3);
        var up = Vector2Int.RoundToInt(upV3);
        // print($"{right} - {up}");
        if (right.x == 1 && up.y == 1) return BlockOrientation.O0;
        if (right.y == -1 && up.x == 1) return BlockOrientation.O270;
        if (right.x == -1 && up.y == -1) return BlockOrientation.O180;
        if (right.y == 1 && up.x == -1) return BlockOrientation.O90;

        if (right.x == -1 && up.y == 1) return BlockOrientation.M0;
        if (right.y == 1 && up.x == 1) return BlockOrientation.M270;
        if (right.x == 1 && up.y == -1) return BlockOrientation.M180;
        if (right.y == -1 && up.x == -1) return BlockOrientation.M90;

        return BlockOrientation.M0;
    }

    public Block GetOrientedBlock(out Vector2Int position)
    {
        position = GetTopLeftPoint();
        Debug.Log(position);

        BlockOrientation orientation = CurrentRotationToOrientation();
        // print(orientation);
        Block copy = block.Clone();
        copy.Rotate(orientation);
        return copy;
    }

    public Vector2Int GetTopLeftPoint()
    {
        Vector3 minPos = visualizer.SnapPoint(body.position);
        for (int x = 0; x < block.Width; x += 1)
        {
            for (int y = 0; y < block.Height; y += 1)
            {
                var testPos = visualizer.SnapPoint(transform.TransformPoint(-x - 1f, -y - 1f, 0));
                minPos.x = Mathf.Min(testPos.x, minPos.x);
                minPos.y = Mathf.Max(testPos.y, minPos.y);
            }
        }
        Vector3 space = minPos * 4; //visualizer.transform.InverseTransformPoint(minPos);
        var rounded = Vector2Int.RoundToInt(space);
        return rounded;
    }

    public Vector2Int GetSimulatorPosition()
    {
        /*
        return new Vector2Int((int) transform.localPosition.x - block.Width,
        (int) transform.localPosition.y);
        */
        var p2 = GetTopLeftPoint();
        p2.x = p2.x + block.Width;
        //p2.y = p2.y + block.Height;
        return p2;
        
    }

    public KinematicBlock Clone()
    {
        var cp = Instantiate(gameObject, transform.position, transform.rotation, transform.parent);
        var kb = cp.GetComponent<KinematicBlock>();
        kb.body = kb.GetComponent<Rigidbody>();
        kb.visualizer = visualizer;
        kb.colliders = new List<Collider>(kb.GetComponents<BoxCollider>());
        kb.block = block;
        kb.orientation = orientation;
        return kb;
    }

    public void Initialize(BlockMapVisualizer visualizer, Block block, BlockMaterial m, PhysicMaterial blocksMaterial)
    {
        this.block = block;
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
        StartCoroutine(HandleFinger(leanFinger, hit));
    }

    public void PushOut(float forceMultiplier = 1)
    {
        if (!body.isKinematic)
        {
            return;
        }
        body.isKinematic = false;
        body.AddForceAtPosition(Vector3.forward * visualizer.ActivateVeclocity * forceMultiplier, body.centerOfMass + Random.insideUnitSphere * .04f, ForceMode.VelocityChange);
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
        Quaternion targetRotation = OrientationToRotation(CurrentRotationToOrientation());
        while (!leanFinger.Up)
        {
            Ray rayTouch = visualizer.WorldCamera.ScreenPointToRay(leanFinger.ScreenPosition);
            if (plane.Raycast(rayTouch, out float distancePlane) && projPlane.Raycast(rayTouch, out float distanceProjPlane))
            {
                float damp = Lean.Touch.LeanTouch.GetDampenFactor(visualizer.SnapBackDampening, Time.deltaTime);
                var projA = rayTouch.GetPoint(distancePlane);
                var projB = rayTouch.GetPoint(distanceProjPlane);
                tracer.Sampling(projB);
                
                switch (tracer.GetGesture())
                {
                    case Gesture.RotateLeft:
                        targetRotation = Quaternion.Euler(0, 0, 90) * targetRotation;
                        tracer.Reset();
                        break;
                    case Gesture.RotateRight:
                        targetRotation = Quaternion.Euler(0, 0, -90) * targetRotation;
                        tracer.Reset();
                        break;
                    case Gesture.MirrorHorizontal:
                        targetRotation = Quaternion.Euler(0, 180, 0) * targetRotation;
                        tracer.Reset();
                        break;
                    case Gesture.MirrorVertical:
                        targetRotation = Quaternion.Euler(180, 0, 0) * targetRotation;
                        tracer.Reset();
                        break;

                }

                var factor = Mathf.InverseLerp(visualizer.GetBottom() - 1f, visualizer.GetBottom(), projB.y);
                var proj = Vector3.Lerp(projA, projB, factor);
                var currentAttach = Vector3.Lerp(transform.TransformPoint(localPos), body.worldCenterOfMass, factor);
                Vector3 nextPosition = Vector3.MoveTowards(body.position, (proj - currentAttach) + body.position, visualizer.MoveBackPerSecond * Time.deltaTime);
                Vector3 snapPosition = Vector3.Lerp(nextPosition, visualizer.SnapPoint(nextPosition), factor * damp);
                Quaternion rot = Quaternion.RotateTowards(body.rotation, targetRotation, visualizer.RotateBackPerSecond * Time.deltaTime * factor);
                if (!visualizer.CanPlace(this))
                {
                    snapPosition += Random.onUnitSphere * .05f;
                }
                body.MovePosition(snapPosition);
                body.MoveRotation(rot);
            }

            yield return null;
        }
        SetCollidersEnabled(true);

        if (visualizer.CanPlace(this))
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
            visualizer.PlaceBlock(this);

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

    private void OnDrawGizmosSelected()
    {
        var block = GetOrientedBlock(out Vector2Int p2d);
        var pos = visualizer.transform.TransformPoint(new Vector3(p2d.x, p2d.y, 0));
        Gizmos.DrawWireSphere(new Vector3(pos.x, pos.y, 0), .125f);
        Vector3 size = transform.TransformVector(Vector3.one);
        size.x = Mathf.Abs(size.x);
        size.y = Mathf.Abs(size.y);
        for (int x = 0; x < block.Width; x += 1)
        {
            for (int y = 0; y < block.Height; y += 1)
            {
                if (!block.IsFieldSet(x, y))
                {
                    continue;
                }
                var p3d = new Vector3(pos.x + (x + .5f) * size.x, pos.y + (-1 - y + .5f) * size.y);
                Gizmos.DrawWireCube(p3d, size);
            }
        }
    }
}