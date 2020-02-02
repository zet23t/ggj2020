using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockOrientation{
    O0, O90, O180, O270,
    M0, M90, M180, M270
}

public class BlockPlacement
{
    public int BlockId;
    public Block Block;
    public BlockOrientation Orientation;
}

public class BlockPoint
{
    public bool Valid;
    public Color Color;
}