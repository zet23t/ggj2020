using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelPattern", menuName = "GJ2020/LevelPattern", order = 0)]
public class LevelPattern : ScriptableObject {
    [Serializable]
    public struct Placement
    {
        public Block Block;
        public Vector2Int Position;
        public BlockOrientation Orientation;

        public Placement(Block block, Vector2Int position, BlockOrientation orientation)
        {
            Block = block;
            Position = position;
            Orientation = orientation;
        }
    }
    public BlockRegistry BlockRegistry;
    public BlockMaterialRegistry BlockMaterialRegistry;
    public List<Placement> Placements;

    public void Add(Block block, BlockOrientation orientation, Vector2Int position)
    {
        block = block.Original;
        Placements.Add(new Placement(block, position, orientation));
        SetDirtyReally();
    }

    public void RemoveAll()
    {
        Placements.Clear();
        SetDirtyReally();
    }

    private void SetDirtyReally()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}