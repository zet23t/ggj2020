using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Lean.Touch;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class BlockMapVisualizer : MonoBehaviour
{
    public bool IsEditor;
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

    private HashSet<KinematicBlock> kinematicBlocks;

    public Vector3 MovePlanePoint => transform.TransformPoint(new Vector3(0, 0, -1.05f));
    public Vector3 PlayPlanePoint => transform.TransformPoint(new Vector3(0, 0, 0));

    public Text DebugOutput;

    private BlockLevelGenerator levelGenerator;

    // Start is called before the first frame update
    void Start()
    {
        simulator = new BlockMapSimulator(Width, Height, BlockRegistry);
        levelGenerator = new BlockLevelGenerator(BlockRegistry, MaterialRegistry, InstantiateBlock, simulator.CanPlaceBlock);

        if (!IsEditor)
        {
            kinematicBlocks = levelGenerator.GenerateLevel();
        }
        else
        {
            int x = 0;
            int y = 4;
            foreach (var block in BlockRegistry.Blocks)
            {
                x += block.Width + 1;
                if (x > Width)
                {
                    x = 0;
                    y += 5;
                }
                var kblock = InstantiateBlock(block, BlockOrientation.O0, x, y, MaterialRegistry.Materials[0], false);
                var pos = kblock.transform.position;
                pos.z = -.5f;
                kblock.transform.position = pos;
                kblock.PushOut(0f);
            }
        }
    }

    public bool ExplodeRandomBlock()
    {
        var amountOfTries = 0;
        while (true)
        {
            if (++amountOfTries > 50)
            {
                return !simulator.IsEmpty();
            }

            var possiblyExplodedBlocks = simulator.Explode(Random.Range(0, Width), Random.Range(0, Height), 1.0f);
            if (possiblyExplodedBlocks.Count != 0)
            {
                foreach (var blockExploded in possiblyExplodedBlocks)
                {
                    var currentKineticBlock = kinematicBlocks.FirstOrDefault(block => block.BlockID == blockExploded.BlockId);

                    currentKineticBlock?.PushOut();
                }

                return true;
            }
        }
    }

    private KinematicBlock InstantiateBlock(Block block, BlockOrientation orientation, int x, int y, BlockMaterial m, bool place = true)
    {
        Debug.Log("InstantiateBlock [" + x + ", " + y + "]");
        
        Vector3 position = new Vector3(x, y, 0);
        var go = Instantiate(block.Prefab, transform.TransformPoint(position), Quaternion.identity, transform);
        var kblock = go.AddComponent<KinematicBlock>();
        kblock.Initialize(this, block, m, BlockMaterial);
        Vector2Int simPos = kblock.GetSimulatorPosition();
        Debug.Log("SimPos: [" + simPos.x + "," + simPos.y + "]");
        if (!place)
        {
            return kblock;
        }

        if (!simulator.CanPlaceBlock(block, orientation, simPos.x, simPos.y))
        {
            Destroy(go);
            return null;
        }

        kblock.BlockID = simulator.PlaceBlock(block, orientation, simPos.x, simPos.y);

        var goBackground =
            Instantiate(block.Prefab, transform.TransformPoint(position - new Vector3(0, 0, -0.87f)),
                Quaternion.identity, transform);
        goBackground.GetComponent<MeshRenderer>().sharedMaterial = m.MaterialPrefab;

        return kblock;
    }

    // Update is called once per frame
    void Update()
    {
        if (DebugOutput != null)
        {
            DebugOutput.text = simulator.ToString();
        }
        
        if (IsEditor)
        {
            HandleInput();
            return;
        }

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.R))
        {
            // Destroy existing blocks
            foreach (var kinematicBlock in (KinematicBlock[]) FindObjectsOfType(typeof(KinematicBlock)))
            {
                Destroy(kinematicBlock.gameObject);
            }

            // Re-Spawn blocks
            kinematicBlocks = levelGenerator.GenerateLevel();
        }

        HandleInput();
    }

    private void HandleInput()
    {
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
                var block = kb.GetOrientedBlock(out Vector2Int pos);
                for (int x = 0; x < block.Width; x += 1)
                {
                    for (int y = 0; y < block.Height; y += 1)
                    {
                        if (block.IsFieldSet(x, y))
                        {
                            simulator.Explode(pos.x + x, pos.y - y, 0);
                            x += block.Width;
                            break;
                        }
                    }
                }
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    kb = kb.Clone();
                }
                kb.Activate(touches[0], hit);
            }
        }
    }

    public bool InsideBox(KinematicBlock block)
    {
        Block oriented = block.GetOrientedBlock(out Vector2Int position);
        return position.x >= 0 && position.y >= oriented.Height && position.x + oriented.Width <= Width &&
            position.y <= Height;
    }

    public bool CanPlace(KinematicBlock block)
    {
        Block oriented = block.GetOrientedBlock(out Vector2Int position);
        return simulator.CanPlaceBlock(oriented, block.CurrentRotationToOrientation(), position.x, position.y);
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
        var block = kinematicBlock.GetOrientedBlock(out Vector2Int pos);
        kinematicBlock.BlockID = simulator.PlaceBlock(block, kinematicBlock.CurrentRotationToOrientation(), pos.x, pos.y);
    }
}