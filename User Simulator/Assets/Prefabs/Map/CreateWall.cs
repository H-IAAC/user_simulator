using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateWall : MonoBehaviour
{
    [SerializeField] float meter_per_pixel;
    [SerializeField] float height;
    [SerializeField] bool floor;
    [SerializeField] Vector4 box_img;
    
    
    public void Create()
    {
        Vector4 box_real = new();

        for(int i = 0; i<4; i++)
        {
            box_real[i] = box_img[i];
        }

        if (box_real[0] > box_real[2])
        {
            float temp = box_real[0];
            box_real[0] = box_real[2];
            box_real[2] = temp;
        }

        if (box_real[1] > box_real[3])
        {
            float temp = box_real[1];
            box_real[1] = box_real[3];
            box_real[3] = temp;
        }

        for(int i = 0; i<4; i++)
        {
            box_real[i] = Mathf.Round(box_real[i]);
 
        }

        box_real *= meter_per_pixel;

        Vector3 size = new(box_real[0]-box_real[2], height, box_real[1]-box_real[3]);

        for (int i = 0; i < 3; i++)
        {
            size[i] = Mathf.Abs(size[i]);
        }

        Vector3 position = new(box_real[0], 0, -box_real[1]);

        position[0] += size[0]/2;

        if(floor)
        {
            position[1] -= size[1]/2;
        }
        else //Wall
        {
            position[1] += size[1]/2;
        }

        
        position[2] -= size[2]/2;

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = size;

        Undo.RegisterCreatedObjectUndo(cube, "Create Wall");
    }
}
