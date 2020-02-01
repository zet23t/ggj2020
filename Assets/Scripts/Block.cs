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
        block.width = width;
        block.height = height;
        block.Prefab = Prefab;
        block.fields = new bool[width * height];
        Array.Copy(fields, block.fields, block.fields.Length);
        return block;
    }

    public void Rotate(BlockOrientation orientation)
    {
        int rotate90 = (int) orientation;
        bool[] fieldsMirrored = new bool[fields.Length];

        //Mirror block
        if ((int) orientation > 3)
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
                Rotate((BlockOrientation) (rotate90));
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
                str += fields[y * width + x] ? "1" : "0";
            }

            str += "\n";
        }

        return str;
    }
}