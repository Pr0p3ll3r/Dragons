using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool dragging = false;
    Vector3 mouseStartPos;
    Vector3 objectStartPos;

    private void Update()
    {
        if (dragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 move = mousePos - mouseStartPos;
            transform.root.position = objectStartPos + move;
        }
    }

    private void OnMouseDown()
    {
        dragging = true;
        mouseStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        objectStartPos = transform.root.position;
    }

    private void OnMouseUp()
    {
        dragging = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        mouseStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        objectStartPos = transform.root.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }
}