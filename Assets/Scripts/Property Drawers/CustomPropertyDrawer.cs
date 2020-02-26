using System;
using System.Collections;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class HidePropertyDrawer : PropertyAttribute
{
    public string HideAttributes = "";
    public bool HideInInspector = false;

    public HidePropertyDrawer(string hideAttributes)
    {
        this.HideAttributes = hideAttributes;
        this.HideInInspector = false;
    }
    public HidePropertyDrawer(string hideAttributes, bool hideInInspector)
    {
        this.HideAttributes = hideAttributes;
        this.HideInInspector = hideInInspector;
    }
}
