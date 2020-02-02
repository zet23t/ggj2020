using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CreateBlockFunctionType = System.Func<Block, BlockOrientation, int, int, BlockMaterial, bool, KinematicBlock>;
using Random = UnityEngine.Random;
using TestBlockFunctionType = System.Func<Block, BlockOrientation, int, int, bool>;

public class BlockLevelGenerator
{
    private BlockRegistry BlockRegistry;
    private BlockMaterialRegistry MaterialRegistry;

    private CreateBlockFunctionType CreateBlockCb;
    private TestBlockFunctionType TestBlockCb;

    public BlockLevelGenerator(
        BlockRegistry BlockRegistry, BlockMaterialRegistry MaterialRegistry,
        CreateBlockFunctionType CreateBlockCb, TestBlockFunctionType TestBlockCb)
    {
        this.BlockRegistry = BlockRegistry;
        this.MaterialRegistry = MaterialRegistry;

        this.CreateBlockCb = CreateBlockCb;
        this.TestBlockCb = TestBlockCb;
    }

    public HashSet<KinematicBlock> GenerateLevel()
    {
        //return GenerateOrdered();
        return GenerateCircle();
    }

    private HashSet<KinematicBlock> GenerateCircle()
    {
        var kinematicBlocks = new HashSet<KinematicBlock>();
        int sizeX = 12;
        int sizeY = 12;

        // Get midpoint
        var gridSize = new Vector2(sizeX, sizeY);
        var midpoint = gridSize * 0.5f;

        List<Block> blocksBySize = new List<Block>(BlockRegistry.Blocks);
        blocksBySize.Sort((b1, b2) => -(b1.Width * b1.Height).CompareTo(b2.Width * b2.Height));
        foreach (var block in blocksBySize)
        {
            for (int i = 0; i < 50; i += 1)
            {
                int x = Random.Range(0, sizeX);
                int y = Random.Range(0, sizeY);
                BlockOrientation orientation = (BlockOrientation) (Random.Range(0, 8) * 0);
                var blockCopy = block.Clone();
                var blockKine = CreateBlockCb(blockCopy, orientation, x, y, GetRandomMaterial(), true);
                if (blockKine)
                {
                    kinematicBlocks.Add(blockKine);
                }

            }
        }

        return kinematicBlocks;
    }

    private Block GetRandomBlock()
    {
        return BlockRegistry.Blocks[UnityEngine.Random.Range(0, BlockRegistry.Blocks.Length)].Clone();
    }

    private BlockMaterial GetRandomMaterial()
    {
        return MaterialRegistry.Materials[UnityEngine.Random.Range(0, MaterialRegistry.Materials.Length)];
    }

    private HashSet<KinematicBlock> GenerateOrdered()
    {
        var kinematicBlocks = new HashSet<KinematicBlock>();

        for (int j = 0; j < 10; j += 4)
        {
            int x = 0;

            for (int i = 0; i < BlockRegistry.Blocks.Length; i += 1)
            {
                var block = GetRandomBlock();
                x += block.Width + 1;
                var blockKinematic = CreateBlockCb(block, BlockOrientation.O0, x, block.Height + j, GetRandomMaterial(), true);
                if (blockKinematic)
                {
                    kinematicBlocks.Add(blockKinematic);
                }
            }
        }

        return kinematicBlocks;
    }
}