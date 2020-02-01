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
        blockR.fields = fields;
        blockR.width = width;
        blockR.height = height;
        blockR.Prefab = Prefab;

        int iRotate = 0;
        
        bool[] fieldsThis = fields;

        //Mirror Block
        if ((int) orientation >= 3)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fieldsThis[y * height + x] = fields[y * height + (width - x)];
                }
            }
        }

        //Rotation works in 90°-Steps, determine how often we have to do this
        switch (orientation)
        {
            case BlockOrientation.O0:
            case BlockOrientation.M0:
                iRotate = 0;
                break;
            case BlockOrientation.O90:
            case BlockOrientation.M90:
                iRotate = 1;
                break;
            case BlockOrientation.O180:
            case BlockOrientation.M180:
                iRotate = 2;
                break;
            case BlockOrientation.O270:
            case BlockOrientation.M270:
                iRotate = 3;
                break;
        }
        
        //Rotate block
        for (int i = 0; i < iRotate; i++)
        {
            blockR.width = height;
            blockR.height = width;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    blockR.fields[y * height + x] = fieldsThis[(width - y - 1) * height + x];
                }
            }

            fieldsThis = blockR.fields;
        }

        return blockR;
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