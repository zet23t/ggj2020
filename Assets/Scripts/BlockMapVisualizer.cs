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

    private List<KinematicBlock> kinematicBlocks = new List<KinematicBlock>();

    public Vector3 MovePlanePoint => transform.TransformPoint(new Vector3(0, 0, -1.05f));
    public Vector3 PlayPlanePoint => transform.TransformPoint(new Vector3(0, 0, 0));

    public BlockMapSimulator Simulator => simulator;

    public Text DebugOutput;

    public AudioSource sfxPlaceBlock;

    private BlockLevelGenerator levelGenerator;

    // Start is called before the first frame update
    void Start()
    {
        simulator = new BlockMapSimulator(Width, Height, BlockRegistry);
        levelGenerator = new BlockLevelGenerator(BlockRegistry, MaterialRegistry, InstantiateBlock, CanPlace);

        if (!IsEditor)
        {
            levelGenerator.GenerateLevel();
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
        if (kinematicBlocks.Count == 0)
        {
            return false;
        }

        var kblock = kinematicBlocks[Random.Range(0, kinematicBlocks.Count)];
        var block = kblock.GetOrientedBlock(out Vector2Int point);
        var possiblyExplodedBlocks = Explode(point.x + block.Width / 2, point.y + block.Height / 2, 3.0f);
        foreach (var blockExploded in possiblyExplodedBlocks)
        {
            var currentKineticBlock = kinematicBlocks.FirstOrDefault(b => b.BlockID == blockExploded.BlockId);
            currentKineticBlock.PushOut();
            kinematicBlocks.Remove(currentKineticBlock);
        }

        return true;
    }

    private KinematicBlock InstantiateBlock(Block block, BlockOrientation orientation, int x, int y, BlockMaterial m, bool place = true)
    {
        Debug.Log("InstantiateBlock [" + x + ", " + y + "]");
        Vector3 position = new Vector3(x, y, 0);
        Vector3 worldSpacePos = transform.TransformPoint(position);
        var go = Instantiate(block.Prefab, worldSpacePos, Quaternion.identity, transform);
        var kblock = go.AddComponent<KinematicBlock>();
        kblock.Initialize(this, block, m, BlockMaterial);
        if (!place)
        {
            return kblock;
        }

        if (!CanPlace(kblock))
        {
            Destroy(go);
            return null;
        }

        PlaceBlock(kblock, true);

        var goBackground =
            Instantiate(block.Prefab, worldSpacePos - new Vector3(0, 0, -0.17f),
                Quaternion.identity, transform);
        goBackground.GetComponent<MeshRenderer>().sharedMaterial = m.MaterialPrefab;

        return kblock;
    }

    // Update is called once per frame
    void Update()
    {
        // DebugText = simulator.ToString();
        // if (DebugOutput != null)
        // {
        //     DebugOutput.text = simulator.ToString();
        // }

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
            kinematicBlocks.Clear();
            levelGenerator.GenerateLevel();
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
            Debug.DrawRay(ray.origin, ray.direction * 20);
            if (Physics.Raycast(ray, out RaycastHit hit, 20, ~(1 << 2)) && hit.collider.GetComponent<KinematicBlock>())
            {
                var kb = hit.collider.GetComponent<KinematicBlock>();
                var block = kb.GetOrientedBlock(out Vector2Int pos);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    kb = kb.Clone();
                }
                else
                {
                    for (int x = 0; x < block.Width; x += 1)
                    {
                        for (int y = 0; y < block.Height; y += 1)
                        {
                            if (block.IsFieldSet(x, y))
                            {
                                Explode(pos.x + x, pos.y - y);
                                x += block.Width;
                                break;
                            }
                        }
                    }
                }
                kb.Activate(touches[0], hit);
                kinematicBlocks.Remove(kb);
            }
        }
    }

    public bool InsideBox(KinematicBlock block)
    {
        Block oriented = block.GetOrientedBlock(out Vector2Int position);
        return position.x >= 0 && position.y >= oriented.Height && position.x + oriented.Width <= Width &&
            position.y <= Height;
    }

    public void PlaceBlock(KinematicBlock kinematicBlock, bool initial = false)
    {
        var block = kinematicBlock.GetOrientedBlock(out Vector2Int pos);

        sfxPlaceBlock.Play();

        pos.y = Height - pos.y;
        Debug.Log("Place @ " + pos.x + ", " + pos.y);
        kinematicBlock.BlockID = simulator.PlaceBlock(block, kinematicBlock.CurrentRotationToOrientation(),
            kinematicBlock.GetComponent<MeshRenderer>().sharedMaterial.color, pos.x, pos.y, initial);
        kinematicBlocks.Add(kinematicBlock);
    }

    public List<BlockPlacement> Explode(int x, int y, float rad = 0f)
    {
        return simulator.Explode(x, Height - y, rad);
    }

    private bool CanPlace(Block b, BlockOrientation orientation, int x, int y)
    {
        y = Height - y;
        return simulator.CanPlaceBlock(b, orientation, x, y);
    }

    public bool CanPlace(KinematicBlock block)
    {
        Block oriented = block.GetOrientedBlock(out Vector2Int position);
        position.y = Height - position.y;
        return simulator.CanPlaceBlock(oriented, block.CurrentRotationToOrientation(), position.x, position.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(new Vector3(Width * .5f, Height * .5f, 0), new Vector3(Width, Height, 0));
        if (simulator == null)
        {
            return;
        }
        int[] grid = simulator.BlockGrid;
        int width = simulator.Width;
        int height = simulator.Height;
        for (int y = 0; y < height; y += 1)
        {
            for (int x = 0; x < height; x += 1)
            {
                int index = x + y * width;
                if (grid[index] < 0) continue;
                Gizmos.color = Color.HSVToRGB((grid[index] * .05f) % 1f, 1, 1);
                Gizmos.DrawCube(new Vector3(x + .5f, height - 1 - y + .5f, -.5f), Vector3.one);
            }

        }
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
}