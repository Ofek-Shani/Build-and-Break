using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class QueuableBoardAction
{
    /// <summary>
    /// coordinates of the gameobject on the board which we are acting on.
    /// </summary>
    public Vector2Int coords;
    public GameObject caller;

    // TODO: find a way to automatically populate this list with all derived types of this class.
    static List<Type> stackableTypes = new() { typeof(QueuableBreak), typeof(QueuableMove)};

    /// <summary>
    /// Does something to the game board (depends on subtype)
    /// </summary>
    public virtual void Act(GameBoard board) { }

    public QueuableBoardAction(Vector2Int newCoords, GameObject callerName)
    {
        coords = newCoords;
        caller = callerName;
    }


    /// <summary>
    /// Checks to see if 2 actions affect the same space
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool CanStack(QueuableBoardAction a, QueuableBoardAction b)
    {
        return (a.coords == b.coords);
    }

    /// <summary>
    /// Attempts to shrink a list of queuable actions by stacking them using the stack rules of each 
    /// </summary>
    /// <param name="qActions"></param>
    /// <returns></returns>
    public static List<QueuableBoardAction> CombineActions(List<QueuableBoardAction> qActions)
    {
        // separate into a list of lists of actions that all affect the same space
        Dictionary<Vector2Int, List<QueuableBoardAction>> sortedActions = new Dictionary<Vector2Int, List<QueuableBoardAction>>();
        foreach (QueuableBoardAction qAct in qActions)
        {
            if (!sortedActions.ContainsKey(qAct.coords)) sortedActions.Add(qAct.coords, new List<QueuableBoardAction>());
            sortedActions[qAct.coords].Add(qAct);
        }
        // ACTION STACKING
        var stackedSortedActions = new Dictionary<Vector2Int, List<QueuableBoardAction>>(sortedActions);
        //Debug.Log("Tiles affected by this turn: " + sortedActions.Count);
        // this is done on a per-tile basis.
        // For each tile's action set, we cycle through all 
        foreach (KeyValuePair<Vector2Int, List<QueuableBoardAction>> entry in sortedActions)
        {
            object[] stackParams = new object[] { (object)entry.Value };
            foreach (Type t in stackableTypes)
            {
                MethodInfo stackFunction = t.GetMethod("Stack");
                stackedSortedActions[entry.Key] = (List<QueuableBoardAction>)stackFunction.Invoke(null, stackParams);
            }
        }
        // now we need to convert back from dictionary into a list
        List<QueuableBoardAction> toReturn = new();
        foreach (KeyValuePair<Vector2Int, List<QueuableBoardAction>> entry in stackedSortedActions)
        {
            foreach (QueuableBoardAction qAct in entry.Value)
            {
                toReturn.Add(qAct);
            }
        }
        return toReturn;
    }
}
