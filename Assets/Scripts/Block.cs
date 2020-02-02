using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "GJ2020/Block", order = 0)]
public class Block : ScriptableObject {
    [SerializeField]
    private int width, height;
    [SerializeField]
    private bool[] fields;

    public int Width => width;
    public int Height => height;
    
    public GameObject Prefab;

    private bool[] originalFields;
    private int originalWidth;
    private int originalHeight;
    private bool isClone;

    private void OnValidate()
    {
        originalFields = new bool[fields.Length];
        Array.Copy(fields, originalFields, fields.Length);
        originalWidth = width;
        originalHeight = height;
    }

    public bool IsFieldSet(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            return false;
        }
        return fields[x + y * width];
    }

    public Block Clone()
    {
        Block block = CreateInstance<Block>();
        block.isClone = true;
        block.width = width;
        block.height = height;
        block.Prefab = Prefab;
        block.fields = new bool[width * height];
        Array.Copy(fields, block.fields, block.fields.Length);
        block.OnValidate();
        return block;
    }

    public void Rotate(BlockOrientation orientation)
    {
        CheckModifyable();
        Array.Copy(originalFields, fields, fields.Length);
        width = originalWidth;
        height = originalHeight;
        Rotate((int)orientation);
    }

    private void CheckModifyable()
    {
        if (!isClone)
        {
            throw new Exception("Don't modify an original!");
        }
    }

    private void Rotate(int rotate90)
    {
        bool[] fieldsMirrored = new bool[fields.Length];

        //Mirror block
        if (rotate90 > 3)
        {
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                fieldsMirrored[y * width + x] = fields[y * width + (width - x - 1)];
            rotate90 -= 4;
        }
        else
        {
            Array.Copy(fields, fieldsMirrored, fields.Length);
        }

        //Rotate block
        if (rotate90 != 0)
        {
            int w = width;
            width = height;
            height = w;
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = fieldsMirrored[RotateIndex(i, width, height)];
            }
            
            if (--rotate90 != 0)
            {
                Rotate(rotate90);
            }
        }
        else
        {
            fields = fieldsMirrored;
        }
    }

    private int RotateIndex(int index, int widthLocal, int heightLocal)
    {
        int x = index % widthLocal;
        int y = index / widthLocal;

        int newX = heightLocal - (y + 1);
        int newIndex = heightLocal * x + newX;

        return newIndex;
    }

    public override string ToString()
    {
        string str = "";
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                str += fields[y * width + x] ? "#" : "_";
            }

            str += "\n";
        }

        return str;
    }
}