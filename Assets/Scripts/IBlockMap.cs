using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BlockOrientation{

}

public class BlockPlacement
{
    public Block Block;
    BlockOrientation Orientation;
}

public interface IBlockMap
{ 
    void PlaceBlock(Block block, BlockOrientation orientation);
    List<BlockPlacement> GetAllBlocks();
}