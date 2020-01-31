using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class KinematicBlock : MonoBehaviour
{
    private Rigidbody body;
    private BlockMapVisualizer visualizer;

    public void Initialize(BlockMapVisualizer visualizer,  Block block)
    {
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
                }
            }
        }
        body = gameObject.AddComponent<Rigidbody>();
        body.isKinematic = true;
    }

    public void Activate()
    {
        if (!body.isKinematic)
        {
            return;
        }
        body.isKinematic = false;
        body.AddForceAtPosition(Vector3.forward * visualizer.ActivateVeclocity, transform.position + Random.insideUnitSphere * .04f, ForceMode.VelocityChange);

    }
}