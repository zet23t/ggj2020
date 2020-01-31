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
        int x = 0;
        for (int i=0;i<5;i+=1)
        {
            var block = BlockRegistry.Blocks[Random.Range(0, BlockRegistry.Blocks.Length)];
            Instantiate(block.Prefab, transform.TransformPoint(new Vector3(x, block.Height, 0)), Quaternion.identity, transform);
            x += block.Width;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(new Vector3(Width * .5f, Height * .5f, 0), new Vector3(Width, Height, 0));
    }
}
