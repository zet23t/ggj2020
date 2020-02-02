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
    public static int BLOCK_ID_EMPTY = -1;
    public static int BLOCK_ID_EXPLODED = -2;

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
    /// If the block id equals BLOCK_ID_EMPTY, there is no block at that position.
    /// </summary>
    private readonly int[] _blockGrid;

    /// <summary>
    /// 
    /// </summary>
    private readonly BlockPoint[] _blockGridBackground;

    /// <summary>
    /// A Dicitonary with all Blocks in the game field.
    /// The index is an unique ID of the Block.
    /// </summary>
    private readonly Dictionary<int, BlockPlacement> _blocks;

    private ScoreHandler scoreHandler;

    public int[] BlockGrid => _blockGrid;

    public BlockMapSimulator(int width, int height, BlockRegistry blockRegistry)
    {
        this.Width = width;
        this.Height = height;
        this.Registry = blockRegistry;

        _blockGrid = new int[width * height];
        _blockGridBackground = new BlockPoint[width * height];
        _blocks = new Dictionary<int, BlockPlacement>();
        _idCounter = 0;

        for (int i = 0; i < _blockGrid.Length; i++)
        {
            _blockGrid[i] = BLOCK_ID_EMPTY;
            _blockGridBackground[i] = new BlockPoint() {Valid = false};
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
    /// Returns true if the gamefield is empty
    /// </summary>
    public bool IsEmpty()
    {
        return _blocks.Values.ToList().Count == 0;
    }

    /// <summary>
    /// Lets an explosion occur on the given position withhin a given radius. All Blocks withhin thatradius will be
    /// removed from the game field and returned in a list.
    public List<BlockPlacement> Explode(int x, int y, float fRadius)
    {
        List<BlockPlacement> explodedBlocks = new List<BlockPlacement>();
        HashSet<int> explodedBlocksIds = new HashSet<int>();

        for (int iX = 0; iX < Width; iX++)
        {
            for (int iY = 0; iY < Height; iY++)
            {
                if (Math.Pow(iX - x, 2) + Math.Pow(iY - y, 2) <= Math.Pow(fRadius, 2))
                {
                    int index = iY * this.Width + iX;
                    int id = _blockGrid[index];
                    if (id >= 0)
                    {
                        explodedBlocksIds.Add(id);
                    }
                    _blockGrid[index] = BLOCK_ID_EXPLODED;
                }
            }
        }
        for (int iX = 0; iX < Width; iX++)
        {
            for (int iY = 0; iY < Height; iY++)
            {
                int index = iY * this.Width + iX;
                int id = _blockGrid[index];
                if (explodedBlocksIds.Contains(id))
                {
                    _blockGrid[index] = BLOCK_ID_EMPTY;
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
    public int PlaceBlock(Block block, BlockOrientation orientation, Color color, int x, int y, bool initial = false)
    {
        block.Rotate(orientation);
        
        if (!CanPlaceBlock(block, x, y))
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
        
        for (int iX = 0; iX < block.Width; iX++)
        {
            for (int iY = 0; iY < block.Height; iY++)
            {
                if (block.IsFieldSet(iX, iY))
                {
                    Debug.Log("Place Block [" + x + ", " + y + "] + [" + iX + ", " + iY + "] @ " + ((y + iY) * Width + (x + iX)));
                    _blockGrid[(y + iY) * Width + (x + iX)] = blockId;
                    if (initial || !scoreHandler)
                    {
                        _blockGridBackground[(y + iY) * Width + (x + iX)] =
                            new BlockPoint() {Valid = true, Color = color};
                    }
                    else
                    {
                        BlockPoint bp = _blockGridBackground[y * Width + x];

                        if (bp.Valid)
                        {
                            if (bp.Color == color)
                            {
                                scoreHandler.BlockScore += 25;
                            }
                            else
                            {
                                scoreHandler.BlockScore += 5;
                            }
                        }
                        else
                        {
                            scoreHandler.BlockScore -= 2;
                        }
                    }
                }
            }
        }
        return blockId;
    }
    
    public bool CanPlaceBlock(Block block, BlockOrientation orientation, int x, int y)
    {
        block.Rotate(orientation);
        return CanPlaceBlock(block, x, y);
    }

    /// <summary>
    /// Checks if a block can be placed at a given position and orientation.
    /// </summary>
    private bool CanPlaceBlock(Block block, int x, int y)
    {
        for (int iX = 0; iX < block.Width; iX++)
        {
            for (int iY = 0; iY < block.Height; iY++)
            {
                int iYGrid = (y + iY);
                int iXGrid = (x + iX);
                if (iYGrid >= Height || iXGrid >= Width || iXGrid < 0 || iYGrid < 0)
                {
                    return false;
                }
                if (block.IsFieldSet(iX, iY) && _blockGrid[iYGrid * Width + iXGrid] > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public override string ToString()
    {
        string str = "";
        for (int iY = Height-1; iY >= 0; iY--)
        {
            for (int iX = 0; iX < Width; iX++)
            {
                int v = _blockGrid[iY * Width + iX];
                
                bool isExploded = v == BLOCK_ID_EXPLODED;
                bool isEmpty = v == BLOCK_ID_EMPTY;
                bool canPlace = CanPlaceBlock(Registry.Blocks[0], BlockOrientation.O0, iX, iY);
                str += v < 0 ? "00" : v.ToString("D2");
                str += ";";
                // str += canPlace ? "_" : "P";
                //str += isExploded ? "_" : "X";
                //str += isEmpty ? "_" : "F";
                // str += "   ";
            }
        
            str += "\n";
        }

        return str;
    }

    public void SetScoreHandler(ScoreHandler scoreHandler)
    {
        this.scoreHandler = scoreHandler;
    }
}