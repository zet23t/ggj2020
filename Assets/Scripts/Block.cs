using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "GJ2020/Block", order = 0)]
public class Block : ScriptableObject {
    public int width, height;
    [SerializeField]
    private bool[] fields;

    public bool IsFieldSet(int x, int y)
    {
        return fields[x];
    }

    public GameObject Prefab;
    
    private void OnValidate() {
        if (fields == null || width * height != fields.Length)
        {
            fields = new bool[width * height];
        }
    }
}