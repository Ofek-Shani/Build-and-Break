using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardController : MonoBehaviour
{
    [SerializeField]
    GameObject keyTextObject, costTextObject, grayOutOverlayObject;
    public GameObject anchorObject { get; private set; }
    [SerializeField]
    float extendDistance = 1; // how much the card moves when it is selected.
    bool isOut;
    TextMeshPro keyText, costText;
    SpriteRenderer overlaySpr;

    void Awake()
    {
        anchorObject = transform.GetChild(0).gameObject;
        keyText = keyTextObject.GetComponent<TextMeshPro>();
        costText = costTextObject.GetComponent<TextMeshPro>();
        overlaySpr = grayOutOverlayObject.GetComponent<SpriteRenderer>();
    }

    public void SetKeyText(string text_in)
    {
        keyText.text = text_in;
    }
    public void SetCostText(string text_in)
    {
        costText.text = text_in;
    }

    /// <summary>
    /// Moves the card out by extendDistance (private param)
    /// only moves out if the card is "in" (move the card in using MoveIn)
    /// </summary>
    public void MoveOut()
    {
        if (!isOut)
        {
            anchorObject.transform.position += new Vector3(extendDistance, 0, 0);
            isOut = true;
        }
    }

    /// <summary>
    /// Moves the card in by extendDistance (private param)
    /// only moves out if the card is "out" (move the card out using MoveIn)
    /// </summary>
    public void MoveIn() 
    {
        if (isOut)
        {
            anchorObject.transform.position -= new Vector3(extendDistance, 0, 0);
            isOut = false;
        }
    }

    /// <summary>
    /// Sets the transparency of the overlay object based on grayout. If grayout is true, the object becomes grayed out.
    /// Repeated uses of SetGrayOut(true) do not stack.
    /// </summary>
    /// <param name="grayout"></param>
    public void SetGrayOut(bool grayout)
    {
        Color bgColor = Camera.main.backgroundColor;
        Color toSet = grayout ? new Color(bgColor.r, bgColor.g, bgColor.b, .5f) : new Color(0, 0, 0, 0);
        overlaySpr.color = toSet;
    }


}
