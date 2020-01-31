using System;
using UnityEngine;

public class KinematicBlock : MonoBehaviour
{
    private Rigidbody body;

    public void Initialize(Block block)
    {
        for (int x = 0; x < block.Width; x += 1)
        {
            for (int y = 0; y < block.Height; y += 1)
            {
                if (block.IsFieldSet(x, y))
                {
                    var box = gameObject.AddComponent<BoxCollider>();
                    box.center = new Vector3(x + .5f - block.Width, -y - .5f, 0);
                    box.size = Vector3.one;
                }
            }
        }
        body = gameObject.AddComponent<Rigidbody>();
        body.isKinematic = true;
    }

    public void Activate()
    {
        body.isKinematic = false;
    }
}