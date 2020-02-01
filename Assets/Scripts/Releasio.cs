using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Releasio : MonoBehaviour
{
    public Material ReleasioMaterial;
    private bool materialsExchanged = false;
    private Dictionary<Renderer, Material> savedMaterials = new Dictionary<Renderer, Material>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Renderer[] allRenderers = (Renderer[]) FindObjectsOfType(typeof(Renderer));

            foreach (var renderer in allRenderers)
            {
                if (materialsExchanged)
                {
                    if (savedMaterials.ContainsKey(renderer))
                    {
                        renderer.material = savedMaterials[renderer];
                    }
                }
                else
                {
                    savedMaterials[renderer] = renderer.material;
                    renderer.material = ReleasioMaterial;
                }
            }

            materialsExchanged = !materialsExchanged;
        }
    }
}