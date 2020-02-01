﻿using System;
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

        int iRotate = (int)orientation;
        
        bool[] fieldsMirrored = new bool[width * height];

        //Mirror Block
        if ((int) orientation > 3)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //Debug.Log($"Mirror: {width}:{height} - {y * width + x} -> {y * width + (width - x - 1)}");
                    fieldsMirrored[y * width + x] = fields[y * width + (width - x - 1)];
                }
            }

            iRotate -= 4;
        }
        else
        {
            fieldsMirrored = fields;
        }

        // //Rotation works in 90°-Steps, determine how often we have to do this
        //
        // if((int)orientation > )
        // switch (orientation)
        // {
        //     case BlockOrientation.O0:
        //     case BlockOrientation.M0:
        //         iRotate = 0;
        //         break;
        //     case BlockOrientation.O90:
        //     case BlockOrientation.M90:
        //         iRotate = 1;
        //         break;
        //     case BlockOrientation.O180:
        //     case BlockOrientation.M180:
        //         iRotate = 2;
        //         break;
        //     case BlockOrientation.O270:
        //     case BlockOrientation.M270:
        //         iRotate = 3;
        //         break;
        // }
        
        for (int i = 0; i < iRotate; i++)
        {
            
        }

        if (iRotate != 0)
        {
            int w = blockR.width;
            blockR.width = blockR.height;
            blockR.height = w;
            
            for (int i = 0; i < fields.Length; i++)
            {
                //Debug.Log($"Rotate Index: {i},{iRotate}->{blockR.RotateIndex(i, iRotate)}");
                blockR.fields[blockR.RotateIndex(i, blockR.height, blockR.width)] = fieldsMirrored[i];
            }
            return blockR.GetRotatedBlock((BlockOrientation)(--iRotate));
        }
        else
        {
            blockR.fields = fieldsMirrored;
        }

        return blockR;
    }

    public int RotateIndex(int index, int widthLocal, int heightLocal)
    {
        int indexDebug = index;

        int x = index % widthLocal;
        int y = index / widthLocal;
        
        //Debug.Log($"");

        int newX = heightLocal - (y + 1);
        int newY = x;
        int newIndex = heightLocal * newY + newX;
        
        //Debug.Log($"INDEX={indexDebug} -> {x}:{y} | NEW-INDEX={newIndex} -> {newX}:{newY} | {widthLocal} ");
        
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