using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hightlight the object when enabled
/// </summary>
public class Hightlight : MonoBehaviour
{
    [Tooltip("Color to hightlight the object.")]
    [SerializeField]
    private Color color = Color.white;
    List<Material> materials;

    Dictionary<Material, Color> oldColors;

    void Awake()
    {
        //Search for materials in the object
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
        //Changes the object color to the hightlight color
        foreach (Material material in materials)
        {
            oldColors[material] = material.GetColor("_BaseColor");

            material.SetColor("_BaseColor", color);
        }

    }
    
    void OnDisable()
    {
        //Returns the object color to the original color
        foreach (Material material in materials)
        {
            if(oldColors.ContainsKey(material))
            {
                material.SetColor("_BaseColor", oldColors[material]);
            }
        }
    }
}