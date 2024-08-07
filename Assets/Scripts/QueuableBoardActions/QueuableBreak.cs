using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class QueuableBreak : QueuableBoardAction
{
    public QueuableBreak(Vector2Int newCoords, GameObject callerName) : base(newCoords, callerName)
    {
        Debug.Log("Adding new QueuableBreak affecting tile " + newCoords.ToString());
    }

    public override void Act(GameBoard board)
    {
        board.BreakAt(coords.x, coords.y);
    }

    public override string ToString()
    {
        return "QueuableBreak with origin " + coords.ToString();
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

    /// <summary>
    /// Looks for a QueuableBreak element in the given list and returns its index.
    /// </summary>
    /// <param name="qActions"></param>
    /// <returns>index of first QueuableBreak in list, and -1 if it does not exist.</returns>
    static int FindAction(List<QueuableBoardAction> qActions)
    {
        for(int i = 0; i < qActions.Count; i++)
        {
            if (qActions[i].GetType() == typeof(QueuableBreak)) return i;
        }
        return -1;
    }
    /// <summary>
    /// Returns a list with "stacked" effects based on the stacking rule of Break.
    /// 
    /// The rule: if the list contains a Break, delete all other actions and keep only this one.
    /// </summary>
    /// <param name="qActions"></param>
    /// <returns></returns>
    public static List<QueuableBoardAction> Stack(List<QueuableBoardAction> qActions) {
        int indexOfBreak = FindAction(qActions);
        if (indexOfBreak == -1) return qActions;
        else return new List<QueuableBoardAction> { qActions[indexOfBreak] };
    }
}
