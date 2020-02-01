using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockOrientation{
    O0, O270, O180, O90,
    M0, M270, M180, M90
}

public class BlockPlacement
{
    public int BlockId;
    public Block Block;
    public BlockOrientation Orientation;
}