using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Piece : MonoBehaviour
{
  
    // enable to make each tile indicate which piece it came from.
    bool showPieceNumbers = false;

    // struct constructor
    public Piece(PieceData p_in, int id_in, GameObject pieceObj_in, GameObject tile)
    {
        p = p_in;
        width = p.width;
        height = p.height;
        id = id_in;
        cost = p.cost;
        pieceObj = pieceObj_in;
        tiles = new GameObject[width, height];

        // Set up the GameObject
        BoxCollider2D coll = pieceObj.AddComponent<BoxCollider2D>();
        coll.offset = new Vector2(width / 2, -height / 2);
        coll.offset += new Vector2((width % 2 == 0) ? -.5f : 0, (height % 2 == 0) ? .5f : 0);
        coll.size = new Vector2(width, height);
        // fill it up with tiles
        //Debug.Log(p.level.ToString() + p.name + " ISNULL " + (p is null) + " DATA IS NULL " + (p.data is null));
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (p.data[i, j])
                {
                    Vector3 pos = new Vector3(i, j - (p.height - 1), 0);
                    GameObject temp = Instantiate(tile, pieceObj.transform, false);
                    temp.transform.localPosition = pos;
                    if(showPieceNumbers) temp.GetComponentInChildren<TextMeshPro>().text = id_in.ToString();
                    tiles[i, j] = temp;
                }
            }
        }
    }
    /// <summary>
    /// EFFECTS updates the appearance of tiles within the piece to indicate if they
    /// are placed in the correct spot.
    /// REQUIRES that piece is entirely within the bounds of the board.
    /// REQUIRES that anchorPosition is the top-left tile of the piece.
    /// </summary>
    /// <param name="anchorPosition"></param>
    /// <param name="board"></param>
    public void UpdateTiles(Vector2Int anchorPosition, GameObject[,] tileBoard, BoardController bc)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j])
                {
                    TileController tc = tiles[i, j].GetComponent<TileController>();
                    tc.SetSprite(tc.GetStatus(anchorPosition.x + i, anchorPosition.y + j, tileBoard, bc));
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
    public bool AddTilesToBoard(Vector2Int anchorPosition, GameObject[,] tileBoard, BoardController bc, Transform boardTransform)
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
                    if (!tiles[i, j].GetComponent<TileController>().CanPlaceAt(anchorPosition.x + i, anchorPosition.y + j, tileBoard, bc))
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

            tileBoard[coordsToPlace[i].x, coordsToPlace[i].y] = tilesToPlace[i];
            tilesToPlace[i].transform.parent = boardTransform;
            tilesToPlace[i].GetComponent<TileController>().Place();
            // snap the tile z back onto the board
                
        }
        return true;
    }
    // structure variables/attributes
    PieceData p;
    GameObject[,] tiles;
    public GameObject pieceObj;
    public int width, height;
    public int id, cost;
    
}
