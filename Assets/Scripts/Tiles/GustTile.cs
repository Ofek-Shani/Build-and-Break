using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GustTile : Tile
{
    public override bool Break(int i, int j, GameBoard board)
    {
        ActionQueue.Instance.QueueAction(new QueuableMove(new Vector2Int(i - 1, j), new Vector2Int(-1, 0), gameObject));
        ActionQueue.Instance.QueueAction(new QueuableMove(new Vector2Int(i + 1, j), new Vector2Int(1, 0), gameObject));
        ActionQueue.Instance.QueueAction(new QueuableMove(new Vector2Int(i, j - 1), new Vector2Int(0, -1), gameObject));
        ActionQueue.Instance.QueueAction(new QueuableMove(new Vector2Int(i, j + 1), new Vector2Int(0, 1), gameObject));
        return base.Break(i, j, board);
    }
}
