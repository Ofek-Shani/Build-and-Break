using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformLerper : MonoBehaviour
{
    Transform toMove;

    float lerpTime = 0.1f;

    bool isLocal = true;

    bool doLerp = false;

    Vector3 targetPos;

    /// <summary>
    /// Starts a lerp to the given position vector. Set local to true if lerping to local coordinates.
    /// </summary>
    /// <param name="newPos"></param> // position to lerp to
    /// <param name="local"></param> // whether the position is in local coordinates or not
    /// <param name="t"></param> // Lerp time: value between 0 and 1 where 0 is don't move and 1 is "snap to on next Update"
    public void LerpTo(Vector3 newPos, bool local, float t)
    {
        doLerp = true;
        targetPos = local ? newPos - toMove.parent.position : newPos;
    }
    /// <summary>
    /// immediately snaps to the given position vector. Set local to true if lerping to local coordinates.
    /// </summary>
    /// <param name="newPos"></param> // position to lerp to
    /// <param name="local"></param> // whether the position is in local coordinates or not
    public void SnapTo(Vector3 newPos, bool local)
    {
        doLerp = true;
        isLocal = local;
        if(isLocal) toMove.localPosition = targetPos;
        else toMove.position = targetPos;
    }

    private void Awake()
    {
        toMove = transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        targetPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (doLerp)
        {
            if (isLocal) toMove.localPosition = Vector3.Lerp(toMove.localPosition, targetPos, lerpTime);
            else toMove.position = Vector3.Lerp(toMove.position, targetPos, lerpTime);
        }
    }
}
