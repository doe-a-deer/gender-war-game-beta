using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject containing character data and expressions
/// </summary>
[CreateAssetMenu(fileName = "New Character", menuName = "Gender War/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Basic Info")]
    public string CharacterName;
    public int Age;
    public string Bio;
    public RouteType AssociatedRoute;

    [Header("Visual")]
    public Sprite PortraitNeutral;
    public Sprite PortraitAlt;
    public Color CharacterColor = Color.white;
    public Mesh Character3DMesh;
    public Material Character3DMaterial;

    [Header("Expressions")]
    public List<CharacterExpression> Expressions = new List<CharacterExpression>();

    [Header("Phone Profile")]
    public PhoneProfile Profile;

    public Sprite GetExpression(string expressionName)
    {
        foreach (var expr in Expressions)
        {
            if (expr.Name.ToLower() == expressionName.ToLower())
                return expr.Sprite;
        }
        return PortraitNeutral;
    }

    public Material GetExpressionMaterial(string expressionName)
    {
        foreach (var expr in Expressions)
        {
            if (expr.Name.ToLower() == expressionName.ToLower())
                return expr.Material3D;
        }
        return Character3DMaterial;
    }
}

[Serializable]
public class CharacterExpression
{
    public string Name;
    public Sprite Sprite;
    public Material Material3D;
}

[Serializable]
public class PhoneProfile
{
    public Sprite Photo;
    public string DisplayName;
    public int DisplayAge;
    public string[] BioLines;
    public string[] Tags;
}
