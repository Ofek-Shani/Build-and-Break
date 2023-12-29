using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public class PieceStruct
    {
        // struct constructor
        public PieceStruct(PieceSO p_in, int id_in, GameObject pieceObj_in, GameObject tile)
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
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (p.data[i, j])
                    {
                        Vector3 pos = new Vector3(i, j - (p.height - 1), 0);
                        GameObject temp = Instantiate(tile, pieceObj.transform, false);
                        temp.transform.localPosition = pos;
                        temp.GetComponentInChildren<TextMeshPro>().text = id_in.ToString();
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
        public void UpdateTiles(Vector2Int anchorPosition, bool[,] board)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (tiles[i, j] is not null)
                    {
                        bool correct = board[anchorPosition.x + i, anchorPosition.y + j];
                        tiles[i, j].GetComponent<TileController>().SetSprite(correct);
                    }
                }
            }
        }

        /// <summary>
        /// adds child tiles to the tileBoard matrix and parents them to boardTransform.
        /// Any duplicate tiles are deleted.
        /// REQUIRES piece is completely within board -- no bits sticking out.
        /// </summary>
        /// <param name="anchorPosition"></param>
        /// <param name="tileBoard"></param>
        /// <param name="boardTransform"></param>
        public void AddTilesToBoard(Vector2Int anchorPosition, GameObject[,] tileBoard, Transform boardTransform)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (tiles[i, j] is not null)
                    {
                        if (tileBoard[anchorPosition.x + i, anchorPosition.y + j] is not null)
                        {
                            GameObject.Destroy(tileBoard[anchorPosition.x + i, anchorPosition.y + j]);
                        }
                        tileBoard[anchorPosition.x + i, anchorPosition.y + j] = tiles[i, j];
                        tiles[i, j].transform.parent = boardTransform;
                    }
                }
            }
        }
        // structure variables/attributes
        PieceSO p;
        GameObject[,] tiles;
        public GameObject pieceObj;
        public int width, height;
        public int id, cost;
    }
}
