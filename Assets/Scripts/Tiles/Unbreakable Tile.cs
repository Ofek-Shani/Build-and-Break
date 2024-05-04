using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnbreakableTile : Tile
{
    public override bool Break(GameObject[,] tileBoard, int i, int j)
    {
        return false;
    }
}
