using System;
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

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
    
    public bool IsFieldSet(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            return false;
        }
        return fields[x + y * width];
    }

    public Block GetRotatedBlock(BlockOrientation orientation)
    {
        Block blockR = CreateInstance<Block>();
        blockR.fields = new bool[width * height];
        blockR.width = width;
        blockR.height = height;
        blockR.Prefab = Prefab;

        int rotate90 = (int) orientation;
        bool[] fieldsMirrored = new bool[width * height];

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
            fieldsMirrored = fields;
        }

        //Rotate block
        if (rotate90 != 0)
        {
            int w = blockR.width;
            blockR.width = blockR.height;
            blockR.height = w;
            for (int i = 0; i < fields.Length; i++)
            {
                blockR.fields[blockR.RotateIndex(i, blockR.height, blockR.width)] = fieldsMirrored[i];
            }

            return blockR.GetRotatedBlock((BlockOrientation) (--rotate90));
        }

        blockR.fields = fieldsMirrored;
        return blockR;
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