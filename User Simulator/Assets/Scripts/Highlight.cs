using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Hightlight : MonoBehaviour
{
    [SerializeField]
    private Color color = Color.white;
    List<Material> materials;

    Dictionary<Material, Color> oldColors;

    void Awake()
    {
        oldColors = new Dictionary<Material, Color>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        materials = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            materials.AddRange(new List<Material>(renderer.materials));
        }
    }

    void OnEnable()
    {
        foreach (Material material in materials)
        {
            oldColors[material] = material.GetColor("_BaseColor");

            material.SetColor("_BaseColor", color);
        }

    }
    
    void OnDisable()
    {
        foreach (Material material in materials)
        {
            if(oldColors.ContainsKey(material))
            {
                material.SetColor("_BaseColor", oldColors[material]);
            }
        }
    }
}