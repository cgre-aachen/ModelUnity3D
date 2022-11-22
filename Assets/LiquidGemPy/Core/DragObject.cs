using System;
using System.Collections;

using System.Collections.Generic;

using UnityEngine;


public class DragObject : MonoBehaviour

{
    private Vector3 mOffset;
    private float mZCoord;
    private Vector3 closerAnchor = new Vector3(1, 1, 0);
    private Vector3 mousePosition;

    private bool snapped = false;
    private bool isDragged = false;
    private Vector3 mouseStartPosition;
    private Vector3 pileDragStartPosition;
    

    void OnMouseDown()

    {
        mZCoord = Camera.main.WorldToScreenPoint(
            gameObject.transform.position).z;
        isDragged = true;
        mouseStartPosition = GetMouseAsWorldPoint();
        pileDragStartPosition = gameObject.transform.position;
    }


    private Vector3 GetMouseAsWorldPoint()

    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }


    void OnMouseDrag()
    {
        if (isDragged)
        {
            mousePosition = pileDragStartPosition + (GetMouseAsWorldPoint() - mouseStartPosition);
            gameObject.transform.position = mousePosition;
        }
    }

    private void OnMouseUp()
    {
        isDragged = false;
        var gameLoc = gameObject.transform.localPosition;
        var snappy = Snapping.Snap(gameLoc, closerAnchor);
        if (snappy.x < 0)
        {
            snappy.x = 0;
            gameObject.transform.localPosition = snappy;
        }
        else
        {
            gameObject.transform.localPosition = snappy;
        }
    }
}