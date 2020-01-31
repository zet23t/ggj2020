// simulates a game

using System.Collections.Generic;

public class BlockMapSimulator : IBlockMap {
    public int width, height;
    public BlockRegistry Block;

    public List<BlockPlacement> GetAllBlocks()
    {
        throw new System.NotImplementedException();
    }

    public void PlaceBlock(Block block, BlockOrientation orientation)
    {
        throw new System.NotImplementedException();
    }


    // simulates a single step in the simulation
    public void Tick()
    {

    }
}