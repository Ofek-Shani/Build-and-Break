using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class QueuableMove : QueuableBoardAction
{
    public Vector2Int moveVector { get; private set; }

    public QueuableMove(Vector2Int newCoords, Vector2Int newMoveVector, GameObject callerName) : base(newCoords, callerName)
    {
        moveVector = newMoveVector;
        //Debug.Log("Adding new QueuableMove affecting tile " + newCoords.ToString() + " with power " + moveVector.ToString());
    }

    public override void Act(GameBoard board)
    {
        //Debug.Log("Moving from " + coords.ToString() + " to " + (moveVector+coords).ToString());
        board.MoveTo(coords.x, coords.y, coords.x + moveVector.x, coords.y + moveVector.y);
    }

    public override string ToString()
    {
        return "QueuableMove with origin " + coords.ToString()+ " and power" + moveVector.ToString();
    }

    /// <summary>
    /// Looks for a QueuableBreak in the given list and returns true if it finds one.
    /// </summary>
    /// <param name="qActions"></param>
    /// <returns></returns>
    public static bool Contains(List<QueuableBoardAction> qActions)
    {
        return FindAction(qActions) > -1;
    }

    // TODO: This is ugly code duplication. There has to be a way to fix this, but I just don't know how...
    /// <summary>
    /// Looks for a QueuableBreak element in the given list and returns its index.
    /// </summary>
    /// <param name="qActions"></param>
    /// <returns>index of first QueuableBreak in list, and -1 if it does not exist.</returns>
    static int FindAction(List<QueuableBoardAction> qActions)
    {
        // Note: when creating new types, you MUST change the names here!
        for(int i = 0; i < qActions.Count; i++)
        {
            if (qActions[i].GetType() == typeof(QueuableMove)) return i;
        }
        return -1;
    }
    /// <summary>
    /// Returns a list with "stacked" effects based on the stacking rule of Break.
    /// 
    /// The rule: Combine the gust vectors of all move effects for this tile into ONE move effect.
    /// </summary>
    /// <param name="qActions"></param>
    /// <returns></returns>
    public static List<QueuableBoardAction> Stack(List<QueuableBoardAction> qActions) {
        // Debug.Log("Stacking Wind Effects on list of length " + qActions.Count);
        List<QueuableBoardAction> toReturn = new();
        Vector2Int moveVec = new();
        foreach(QueuableBoardAction qAct in qActions)
        {
            if (qAct.GetType() == typeof(QueuableMove)) moveVec += ((QueuableMove)qAct).moveVector;
            else toReturn.Add(qAct);
        }
        if (moveVec != Vector2Int.zero) toReturn.Add(new QueuableMove(qActions[0].coords, moveVec, null));
        //Debug.Log("Post stack list count: " + qActions.Count);
        return toReturn;
    }
}
