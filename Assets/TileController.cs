using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controller Implementation used by the tiles placed by the player.
/// DOES NOT apply to the background tiles!
/// </summary>
public class TileController : MonoBehaviour
{
    SpriteRenderer spr;
    [SerializeField]
    Sprite correct, incorrect;
    // Start is called before the first frame update
    void Awake()
    {
        spr = GetComponentInChildren<SpriteRenderer>();   
        SetSprite( true );
    }

    public void SetSprite(bool iscorrect)
    {
        spr.sprite = iscorrect ? correct : incorrect;
    }
}
