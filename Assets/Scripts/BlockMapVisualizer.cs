using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public class BlockMapVisualizer : MonoBehaviour
{
    public Camera WorldCamera;
    public int Width;
    public int Height;

    public BlockRegistry BlockRegistry;

    // Start is called before the first frame update
    void Start()
    {
        for (int j = 0; j < 10; j += 4)
        {
            int x = 0;

            for (int i = 0; i < BlockRegistry.Blocks.Length; i += 1)
            {
                var block = BlockRegistry.Blocks[i];
                InstantiateBlock(block, new Vector3(x, block.Height + j, 0));
                x += block.Width;
            }
        }
    }

    private KinematicBlock InstantiateBlock(Block block, Vector3 position)
    {
        var go = Instantiate(block.Prefab, transform.TransformPoint(position), Quaternion.identity, transform);
        var kblock = go.AddComponent<KinematicBlock>();
        kblock.Initialize(block);
        return kblock;
    }

    // Update is called once per frame
    void Update()
    {
        var touches = LeanTouch.GetFingers(true, false, 1);
        if (touches == null || touches.Count == 0)
        {
            return;
        }

        var ray = WorldCamera.ScreenPointToRay(touches[0].ScreenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 20, -1) && hit.collider.GetComponent<KinematicBlock>())
        {
            var kb = hit.collider.GetComponent<KinematicBlock>();
            kb.Activate();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(new Vector3(Width * .5f, Height * .5f, 0), new Vector3(Width, Height, 0));
    }
}