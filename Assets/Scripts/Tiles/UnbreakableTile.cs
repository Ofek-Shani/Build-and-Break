using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnbreakableTile : Tile
{
    public override bool Break(int i, int j, GameBoard board)
    {
        return false;
    }
}
