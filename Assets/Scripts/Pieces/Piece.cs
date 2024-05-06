using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Piece: MonoBehaviour
{

    // structure variables/attributes
    public GameObject[,] tiles;
    public GameObject pieceObj;
    public int width, height;
    public int id, cost;


    // enable to make each tile indicate which piece it came from.
    bool showPieceNumbers = false;

    // struct constructor
    
    /// <summary>
    /// EFFECTS updates the appearance of tiles within the piece to indicate if they
    /// are placed in the correct spot.
    /// REQUIRES that piece is entirely within the bounds of the board.
    /// REQUIRES that anchorPosition is the top-left tile of the piece.
    /// </summary>
    /// <param name="anchorPosition"></param>
    /// <param name="board"></param>
    public void UpdateTiles(Vector2Int anchorPosition, GameObject[,] tileBoard, GameBoard board)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j])
                {
                    Tile tc = tiles[i, j].GetComponent<Tile>();
                    tc.SetSprite(tc.GetStatus(anchorPosition.x + i, anchorPosition.y + j, board));
                }
            }
        }
    }

    /// <summary>
    /// Will add the piece to the board, if possible. 
    /// REQUIRES that the piece is contained within the board.
    /// will return false if it is not able to place the piece (one or more tiles is occupied and/or is a hole)
    /// </summary>
    /// <param name="anchorPosition"></param>
    /// <param name="tileBoard"></param>
    /// <param name="boardTransform"></param>
    public bool AddTilesToBoard(Vector2Int anchorPosition, GameObject[,] tileBoard, GameBoard board, Transform boardTransform)
    {
        List<Vector2Int> coordsToPlace = new List<Vector2Int>();
        List<GameObject> tilesToPlace = new List<GameObject>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j] is not null)
                {
                    // We can't place the piece if at least one tile is obstructed.
                    if (!tiles[i, j].GetComponent<Tile>().CanPlaceAt(anchorPosition.x + i, anchorPosition.y + j, board))
                    {
                        return false;
                    }
                    tilesToPlace.Add(tiles[i, j]);
                    coordsToPlace.Add(new Vector2Int(anchorPosition.x + i, anchorPosition.y + j));
                }
            }
        }
        for(int i = 0; i < coordsToPlace.Count; i++)
        {

            tilesToPlace[i].transform.parent = boardTransform;
            board.PlaceAt(tilesToPlace[i], coordsToPlace[i].x, coordsToPlace[i].y);
            // snap the tile z back onto the board
                
        }
        return true;
    }
    
    
}
