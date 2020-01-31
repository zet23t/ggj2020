using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Releasio : MonoBehaviour
{
    public Material ReleasioMaterial;
    private bool materialsExchanged = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !materialsExchanged)
        {
            Renderer[] allRenderers = (Renderer[]) FindObjectsOfType(typeof(Renderer));

            foreach (var renderer in allRenderers)
            {
                renderer.material = ReleasioMaterial;
            }

            materialsExchanged = true;
        }
    }
}