using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockMaterial", menuName = "GJ2020/BlockMaterial", order = 0)]
public class BlockMaterial : ScriptableObject {
    
    public Material MaterialPrefab;

    public Material GetMaterial()
    {
        return MaterialPrefab;
    }
}