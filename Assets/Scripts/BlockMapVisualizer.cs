using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public class BlockMapVisualizer : MonoBehaviour
{
    public PhysicMaterial BlockMaterial;
    public Camera WorldCamera;
    public int Width;
    public int Height;
    public float ActivateVeclocity = 1;

    public BlockMaterialRegistry MaterialRegistry;
    public BlockRegistry BlockRegistry;

    public float RotateBackPerSecond = 180;
    public float MoveBackPerSecond = 2;
    public float SnapBackDampening = 100;

    public Vector3 MovePlanePoint => transform.TransformPoint(new Vector3(0, 0, -1.05f));
    public Vector3 PlayPlanePoint => transform.TransformPoint(new Vector3(0, 0, 0));

    // Start is called before the first frame update
    void Start()
    {
        SpawnBlocks();
    }

    private void SpawnBlocks()
    {
        for (int j = 0; j < 10; j += 4)
        {
            int x = 0;

            for (int i = 0; i < BlockRegistry.Blocks.Length; i += 1)
            {
                var block = BlockRegistry.Blocks[i];
                x += block.Width;
                var material = MaterialRegistry.Materials[UnityEngine.Random.Range(0, MaterialRegistry.Materials.Length)];
                InstantiateBlock(block, new Vector3(x, block.Height + j, 0), material);
            }
        }
    }

    private KinematicBlock InstantiateBlock(Block block, Vector3 position, BlockMaterial m)
    {
        var go = Instantiate(block.Prefab, transform.TransformPoint(position), Quaternion.identity, transform);
        var kblock = go.AddComponent<KinematicBlock>();
        kblock.Initialize(this, block, m, BlockMaterial);
        return kblock;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Destroy existing blocks
            foreach (var kinematicBlock in (KinematicBlock[]) FindObjectsOfType(typeof(KinematicBlock)))
            {
                Destroy(kinematicBlock.gameObject);
            }

            // Re-Spawn blocks
            SpawnBlocks();
        }

        var touches = LeanTouch.GetFingers(true, false, 1);
        if (touches == null || touches.Count == 0)
        {
            return;
        }

        if (touches[0].Down)
        {
            var ray = WorldCamera.ScreenPointToRay(touches[0].ScreenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 20, -1) && hit.collider.GetComponent<KinematicBlock>())
            {
                var kb = hit.collider.GetComponent<KinematicBlock>();
                kb.Activate(touches[0], hit);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(new Vector3(Width * .5f, Height * .5f, 0), new Vector3(Width, Height, 0));
    }

    public Vector3 SnapPoint(Vector3 pos)
    {
        pos = transform.InverseTransformPoint(pos);
        pos.x = Mathf.Round(pos.x);
        pos.y = Mathf.Round(pos.y);
        return transform.TransformPoint(pos);
    }

    public bool IsFitting(KinematicBlock kinematicBlock)
    {
        return kinematicBlock.transform.position.y > 1;
    }
}