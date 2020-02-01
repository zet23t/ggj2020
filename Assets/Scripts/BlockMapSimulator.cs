// simulates a game

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Simulates the 2D-Grid of our game.
/// </summary>
public class BlockMapSimulator
{
    private int _idCounter;

    /// <summary>
    /// The width of the grid
    /// </summary>
    public int Width;
    
    /// <summary>
    /// The height of the grid
    /// </summary>
    public int Height;
    
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
        this.Width = width;
        this.Height = height;
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
    /// Lets an explosion occur on the given position withhin a given radius. All Blocks withhin thatradius will be
    /// removed from the game field and returned in a list.
    public List<BlockPlacement> Explode(int x, int y, float fRadius)
    {
        y = InvertY(y);
        
        
        List<BlockPlacement> explodedBlocks = new List<BlockPlacement>();
        HashSet<int> explodedBlocksIds = new HashSet<int>();

        for (int iX = 0; iX < Width; iX++)
        {
            for (int iY = 0; iY < Height; iY++)
            {
                if (Math.Pow(iX - x, 2) + Math.Pow(iY - y, 2) <= Math.Pow(fRadius, 2))
                {
                    int id = _blockGrid[iY * this.Width + iX];
                    if (id >= 0)
                    {
                        explodedBlocksIds.Add(id);
                    }
                    _blockGrid[iY * this.Width + iX] = -2;
                }
            }
        }
        for (int iX = 0; iX < Width; iX++)
        {
            for (int iY = 0; iY < Height; iY++)
            {
                int id = _blockGrid[iY * this.Width + iX];
                if (explodedBlocksIds.Contains(id))
                {
                    _blockGrid[iY * this.Width + iX] = -1;
                }
            }
        }

        foreach (int id in explodedBlocksIds)
        {
            explodedBlocks.Add(_blocks[id]);
            _blocks.Remove(id);
        }

        return explodedBlocks;
    }

    /// <summary>
    /// Places a block at the given position and orientation.
    /// </summary>
    public void PlaceBlock(Block block, BlockOrientation orientation, int x, int y)
    {
        y = InvertY(y);
        
        block = block.GetRotatedBlock(orientation);
        
        if (!CanPlaceBlock(block, orientation, x, y, false))
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
                    _blockGrid[(y + iY) * Width + (x + iX)] = blockId;
                }
            }
        }
    }

    /// <summary>
    /// Checks if a block can be placed at a given position and orientation.
    /// </summary>
    public bool CanPlaceBlock(Block block, BlockOrientation orientation, int x, int y, bool invertYAxis = true)
    {
        if (invertYAxis)
            y = InvertY(y);

        //if (x < 0 || y < 0 || x >= Width - 1 || y >= Height - 1)
        //    return false;

        block = block.GetRotatedBlock(orientation);

        for (int iX = 0; iX < block.GetWidth(); iX++)
        {
            for (int iY = 0; iY < block.GetHeight(); iY++)
            {
                int target = (y + iY) * Width + (x + iX);
                if (target < 0 || target >= _blockGrid.Length)
                {
                    return false;
                }
                if (block.IsFieldSet(iX, iY) && _blockGrid[(y + iY) * Width + (x + iX)] > 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private int InvertY(int y)
    {
        return Height - y;
    }

    /// <summary>
    /// Simulates a single step in the simulation.
    /// </summary>
    public void Tick()
    {
        
    }

    public override string ToString()
    {
        string str = "";
        for (int iY = 0; iY < Height; iY++)
        {
            for (int iX = 0; iX < Width; iX++)
            {
                int v = _blockGrid[iY * Width + iX];
                if (v == -2) str += "XX";
                else str += v != -1 ? v.ToString("D2") : "__";
            }
        
            str += "\n";
        }

        return str;
    }
}