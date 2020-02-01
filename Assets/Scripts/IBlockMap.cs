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

public interface IBlockMap
{ 
    /// <summary>
    /// Places a block at the given position and orientation.
    /// </summary>
    void PlaceBlock(Block block, BlockOrientation orientation, int x, int y);

    /// <summary>
    /// Checks if a block can be placed at a given position and orientation.
    /// </summary>
    bool CanPlaceBlock(Block block, BlockOrientation orientation, int x, int y, bool invertYAxis);
    
    /// <summary>
    /// Returns a list with all blocks in the gamefield.
    /// </summary>
    List<BlockPlacement> GetAllBlocks();
    
    /// <summary>
    /// Lets an explosion occur on the given position withhin a given radius. All Blocks withhin thatradius will be
    /// removed  from the game field and returned in a list.
    List<BlockPlacement> Explode(int x, int y, float fRadius);
    
    /// <summary>
    /// Simulates a single step in the simulation.
    /// </summary>
    void Tick();
}