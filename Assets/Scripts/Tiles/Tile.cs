using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Used by basic pieces to handle logic when placing, breaking, etc.
/// </summary>
public class Tile : MonoBehaviour
{     
    public virtual bool CanPlaceAt(GameObject[,] tileBoard, int i, int j)
    {
        return tileBoard[i, j] is null;
    }


    /// <summary>
    /// Attempts to place the tile at the given location. Returns false if the process fails.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public virtual bool Place(GameObject[,] tileBoard, int i, int j)
    {
        if (!CanPlaceAt(tileBoard, i, j)) return false;
        tileBoard[i, j] = gameObject;
        return true;
    }

    /// <summary>
    /// Handles logic for removing the piece. If the piece cannot be broken, this returns false.
    /// Board is the board of all tile objects, 
    /// </summary>
    /// <param name="tileBoard"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public virtual bool Break(GameObject[,] tileBoard, int i, int j)
    {
        tileBoard[i, j] = null;
        Destroy(gameObject);
        return true;
    }

}
