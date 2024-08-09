using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstTile : Tile
{
    public override bool Break(int i, int j, GameBoard board)
    {
        // break adjacent tiles
        ActionQueue.Instance.QueueAction(new QueuableBreak(new Vector2Int(i - 1, j), gameObject));
        ActionQueue.Instance.QueueAction(new QueuableBreak(new Vector2Int(i + 1, j), gameObject));
        ActionQueue.Instance.QueueAction(new QueuableBreak(new Vector2Int(i, j - 1), gameObject));
        ActionQueue.Instance.QueueAction(new QueuableBreak(new Vector2Int(i, j + 1), gameObject));
        return base.Break(i, j, board);
    }
}
