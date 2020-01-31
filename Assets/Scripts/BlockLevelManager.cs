using UnityEngine;

public class BlockLevelManager : MonoBehaviour {
    public BlockRegistry Registry;

    private void Start() {
        foreach (var block in Registry.Blocks)
        {
            print(block);
        }
    }
}