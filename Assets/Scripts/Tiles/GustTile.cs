using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GustTile : Tile
{
    public override bool Break(int i, int j, GameBoard board)
    {
        Destroy(gameObject);
        return true;
    }
}
