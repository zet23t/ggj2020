using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Releasio : MonoBehaviour
{
    public Material ReleasioMaterial;
    private bool materialsExchanged = false;
    private readonly Dictionary<Renderer, Material> savedMaterials = new Dictionary<Renderer, Material>();

    void Update()
    {
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.E))
        {
            GetComponent<AudioSource>()?.Play();

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

        if (materialsExchanged)
        {
            foreach (var renderer in savedMaterials.Keys)
            {
                float offset = Time.time * 0.5f;
                renderer.material.mainTextureOffset = new Vector2(offset, 0);
                renderer.material.SetVector("_EmissionColor", new Vector4(1.0f, 0.0f, 0.0f) * 
                    (Mathf.Sin(Time.realtimeSinceStartup) + 1) * 4
                );
            }
        }
    }
}