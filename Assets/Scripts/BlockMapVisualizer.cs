using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;
using UnityEngine.UI;

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
    private BlockMapSimulator simulator;

    public Vector3 MovePlanePoint => transform.TransformPoint(new Vector3(0, 0, -1.05f));
    public Vector3 PlayPlanePoint => transform.TransformPoint(new Vector3(0, 0, 0));

    public Text DebugOutput;

    // Start is called before the first frame update
    void Start()
    {
        simulator = new BlockMapSimulator(Width, Height, BlockRegistry);
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
                InstantiateBlock(block, x, block.Height + j, material);
            }
        }
    }

    private KinematicBlock InstantiateBlock(Block block, int x, int y, BlockMaterial m)
    {
        Vector3 position = new Vector3(x, y, 0);
        var go = Instantiate(block.Prefab, transform.TransformPoint(position), Quaternion.identity, transform);
        var kblock = go.AddComponent<KinematicBlock>();
        kblock.Initialize(this, block, m, BlockMaterial);
        if (!simulator.CanPlaceBlock(block, BlockOrientation.O0, x, y))
        {
            Destroy(go);
            return null;
        }
        simulator.PlaceBlock(block, BlockOrientation.O0, x, y);

        // MapSimulator.PlaceBlock(block, BlockOrientation.O0, (int)position.x - block.Width, (int)position.y - block.Height);
        return kblock;
    }

    // Update is called once per frame
    void Update()
    {
        DebugOutput.text = simulator.ToString();
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ) && Input.GetKeyDown(KeyCode.R))
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
            if (Physics.Raycast(ray, out RaycastHit hit, 20, ~(1 << 2)) && hit.collider.GetComponent<KinematicBlock>())
            {
                var kb = hit.collider.GetComponent<KinematicBlock>();
                Vector2Int pos = kb.GetTopLeftPoint();
                simulator.Explode(pos.x, pos.y, 0);
                kb.Activate(touches[0], hit);
            }
        }
    }

    public bool InsideBox(KinematicBlock block)
    {
        Block oriented = block.GetOrientedBlock(out Vector2Int position);
        return position.x >= 0 && position.y >= oriented.Height && position.x + oriented.Width <= Width && position.y <= Height;
    }

    public bool CanPlace(KinematicBlock block)
    {
        Block oriented = block.GetOrientedBlock(out Vector2Int position);
        // TODO: simulator check
        if (!(position.x >= 0 && position.y >= oriented.Height && position.x + oriented.Width <= Width && position.y <= Height))
        {
            return false;
        }
        var b = block.GetOrientedBlock(out Vector2Int pos);
        return simulator.CanPlaceBlock(b, block.CurrentRotationToOrientation(), pos.x, pos.y);
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

    public float GetBottom()
    {
        return transform.position.y;
    }

    public void PlaceBlock(KinematicBlock kinematicBlock)
    {
        var orientation = kinematicBlock.GetOrientedBlock(out Vector2Int pos);
        simulator.PlaceBlock(orientation, kinematicBlock.CurrentRotationToOrientation(), pos.x, pos.y);
    }
}