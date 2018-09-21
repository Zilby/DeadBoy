using UnityEngine;
using System;
using System.Collections;

//Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
//Modified by: Alex Zilbersher

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute
{
    public struct Range
    {
        public float min;
        public float max;

        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    public string ConditionalSourceField = "";
    public bool HideInInspector = false;
    public bool Inverse = false;
    public Range range;

    public ConditionalHideAttribute(string conditionalSourceField = "", bool hideInInspector = false, bool inverse = false, float min = 0, float max = 0)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = hideInInspector;
        this.Inverse = inverse;
        this.range = new Range(min, max);
    }
}



