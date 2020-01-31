// simulates a game

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Simulates the 2D-Grid of our game.
/// </summary>
public class BlockMapSimulator : IBlockMap
{
    private int _idCounter;

    /// <summary>
    /// The width of the grid
    /// </summary>
    private readonly int _width;
    
    /// <summary>
    /// The height of the grid
    /// </summary>
    private readonly int _height;
    
    /// <summary>
    /// Information about all available Block types
    /// </summary>
    public BlockRegistry Registry;

    /// <summary>
    /// A two-dimensional grid that represents the game field.
    /// The block id refers to a BlockPlacement-Object from <see cref="_blocks"/>.
    /// If the block id equals -1, there is no block at that position.
    /// </summary>
    private readonly int[] _blockGrid;
    
    /// <summary>
    /// A Dicitonary with all Blocks in the game field.
    /// The index is an unique ID of the Block.
    /// </summary>
    private readonly Dictionary<int, BlockPlacement> _blocks;

    public BlockMapSimulator(int width, int height, BlockRegistry blockRegistry)
    {
        this._width = width;
        this._height = height;
        this.Registry = blockRegistry;

        _blockGrid = new int[width * height];
        _blocks = new Dictionary<int, BlockPlacement>();
        _idCounter = 0;

        for (int i = 0; i < _blockGrid.Length; i++)
        {
            _blockGrid[i] = -1;
        }
    }

    /// <summary>
    /// Returns a list with all blocks in the gamefield.
    /// </summary>
    public List<BlockPlacement> GetAllBlocks()
    {
        return _blocks.Values.ToList();
    }

    /// <summary>
    /// Places a block at the given position.
    /// </summary>
    public void PlaceBlock(Block block, BlockOrientation orientation, int x, int y)
    {
        if (!CanPlaceBlock(block, orientation, x, y))
        {
            throw new InvalidOperationException("No block can be placed here! Use CanPlaceBlock() first!");
        }
        
        int blockId = _idCounter++;
        
        _blocks.Add(blockId, new BlockPlacement()
        {
            BlockId = blockId,
            Block = block,
            Orientation = orientation
        });
        
        for (int iX = 0; iX < block.GetWidth(); iX++)
        {
            for (int iY = 0; iY < block.GetHeight(); iY++)
            {
                if (block.IsFieldSet(iX, iY))
                {
                    //TODO: Rotation
                    _blockGrid[(y + iY) * this._height + (x + iX)] = blockId;
                }
            }
        }
    }

    /// <summary>
    /// Checks if a block can be placed at a given position.
    /// </summary>
    public bool CanPlaceBlock(Block block, BlockOrientation orientation, int x, int y)
    {
        for (int iX = 0; iX < block.GetWidth(); iX++)
        {
            for (int iY = 0; iY < block.GetHeight(); iY++)
            {
                if (block.IsFieldSet(iX, iY) && _blockGrid[y * this._height + x] > 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // simulates a single step in the simulation
    public void Tick()
    {

    }
}