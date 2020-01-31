using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMapVisualizer : MonoBehaviour
{
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

    private void InstantiateBlock(Block block, Vector3 position)
    {
        var go = Instantiate(block.Prefab, transform.TransformPoint(position), Quaternion.identity, transform);
        for (int x = 0; x < block.Width; x += 1)
        {
            for (int y = 0; y < block.Height; y += 1)
            {
                if (block.IsFieldSet(x, y))
                {
                    var box = go.AddComponent<BoxCollider>();
                    box.center = new Vector3(x + .5f - block.Width, -y - .5f, 0);
                    box.size = Vector3.one;
                }
            }
        }
        go.AddComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(new Vector3(Width * .5f, Height * .5f, 0), new Vector3(Width, Height, 0));
    }
}