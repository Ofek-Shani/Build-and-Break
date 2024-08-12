using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This class serves to create Piece object instances. It creates an empty gameobject and gives it all necessary components.
/// </summary>
public class PieceFactory : MonoBehaviour
{
    [SerializeField]
    static GameObject normalTile = Resources.Load<GameObject>("Prefabs/Tiles/Tile");
    static GameObject unbreakableTile = Resources.Load<GameObject>("Prefabs/Tiles/Tile_Unbreakable");
    static GameObject burstTile = Resources.Load<GameObject>("Prefabs/Tiles/Tile_Burst");
    static GameObject gustTile = Resources.Load<GameObject>("Prefabs/Tiles/Tile_Gust");

    static Dictionary<Color32, GameObject> pieceColors = new Dictionary<Color32, GameObject>()
    {
        {new Color32(0,0,0,255), normalTile}, // Normal
        {new Color32(255,128,0,255), burstTile}, // Burst
        {new Color32(128,128,255,255), gustTile}, // Gust
        {new Color32(128,128,128,255), unbreakableTile} // Unbreakable
    };

    /// <summary>
    /// Creates a piece component with all necessary values given the inputted PieceData
    /// </summary>
    /// <param name="p_in"></param>
    /// <param name="id_in"></param>
    /// <param name="pieceObj_in"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    public static Piece MakePiece(PieceData p_in, int id_in, GameObject pieceObj_in)
    {

        // then create the piece
        Piece piece = pieceObj_in.AddComponent<Piece>();
        piece.width = p_in.width;
        piece.height = p_in.height;
        piece.id = id_in;
        piece.cost = p_in.cost;
        piece.pieceObj = pieceObj_in;
        piece.tiles = new GameObject[piece.width, piece.height];

        // Set up the GameObject
        // adding collider
        BoxCollider2D coll = piece.pieceObj.AddComponent<BoxCollider2D>();
        coll.offset = new Vector2(piece.width / 2, -piece.height / 2);
        coll.offset += new Vector2((piece.width % 2 == 0) ? -.5f : 0, (piece.height % 2 == 0) ? .5f : 0);
        coll.size = new Vector2(piece.width, piece.height);
        // adding lerpabletransform
        TransformLerper tlerp = piece.pieceObj.AddComponent<TransformLerper>();
        // fill it up with tiles
        //Debug.Log(p.level.ToString() + p.name + " ISNULL " + (p is null) + " DATA IS NULL " + (p.data is null));
        for (int i = 0; i < piece.width; i++)
        {
            for (int j = 0; j < piece.height; j++)
            {
                if (p_in.data[i, j])
                {
                    // which tile are we making?
                    GameObject tile = pieceColors[p_in.texture.GetPixel(i,j)];
                    // where are we putting it?
                    Vector3 pos = new Vector3(i, j - (piece.height - 1), 0);
                    GameObject temp = Instantiate(tile, piece.pieceObj.transform, false);
                    temp.transform.localPosition = pos;
                    //if (showPieceNumbers) temp.GetComponentInChildren<TextMeshPro>().text = id_in.ToString();
                    // hook it up to the piece
                    piece.tiles[i, j] = temp;
                }
            }
        }

        return piece;
    }
}
