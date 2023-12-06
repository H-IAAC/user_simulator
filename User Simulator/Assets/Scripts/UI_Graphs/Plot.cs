using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public struct PlotData
{
    public List<float> values;
    public Color color;
}

public class Plot : VisualElement
{
    public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlBoolAttributeDescription centerZero = new UxmlBoolAttributeDescription()
            {
                name = "center-zero"
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                (ve as Plot).centerZero = centerZero.GetValueFromBag(bag, cc);
            }
        }
    public new class UxmlFactory : UxmlFactory<Plot, UxmlTraits> { }

    public List<PlotData> data = new();

    public bool centerZero;

    public Plot()
    {
        generateVisualContent += GenerateVisualContent;
    } 

    void GenerateVisualContent(MeshGenerationContext context)
    {
        foreach(PlotData curveData in data)
        {
            List<float> values = curveData.values;
            if(values == null || values.Count == 0)
            {
                return;
            }

            float width = contentRect.width;
            float height = contentRect.height;

            float xStep = width / (values.Count-1);

            float minValue = values[0], maxValue = values[0];

            foreach(float value in values)
            {
                if(value < minValue)
                {
                    minValue = value;
                }
                if(value > maxValue)
                {
                    maxValue = value;
                }
            }

            if(centerZero)
            {
                float maxAbs = Mathf.Max(Mathf.Abs(minValue), Mathf.Abs(maxValue));
                minValue = -maxAbs;
                maxValue = maxAbs;
            }

            Painter2D painter = context.painter2D;

            painter.strokeColor = curveData.color;
            painter.lineJoin = LineJoin.Miter;
            painter.lineCap = LineCap.Round;
            painter.lineWidth = 10.0f;
            painter.BeginPath();

            Vector2 position = Vector2.zero;
            for(int i = 0; i<values.Count; i++)
            {
                float value = values[i];

                position.y = Mathf.InverseLerp(minValue, maxValue, value);
                position.y = 1 - position.y;
                position.y = Mathf.Lerp(0, height, position.y);
                
            
                if(i == 0)
                {
                    painter.MoveTo(position);
                }
                else
                {
                    painter.LineTo(position);
                }

                position.x += xStep;
            }

            painter.Stroke();

        }

        
    }
}