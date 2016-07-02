using System;
using UnityEngine;using UnityEngine.EventSystems;public class MapMoveController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

    public float dragSpeed = 7;
    public float scrollSpeed = 0.5f;

    Vector3 dragOrigin;
    Boolean dragging;

    // Use this for initialization
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (!dragging) {
            // WASD
            Vector3 move = new Vector3(Input.GetAxis("Horizontal") * scrollSpeed * -1, 0, Input.GetAxis("Vertical") * scrollSpeed * -1);
            transform.Translate(move, Space.World);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Debug.Log("OnBeginDrag");
        dragging = true;
        dragOrigin = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("OnBeginDrag");
        dragging = false;
    }

    public void OnDrag(PointerEventData eventData) {
        Debug.Log("OnBeginDrag");
        Vector3 pos = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
        Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);
        transform.Translate(move, Space.World);
        dragOrigin = Input.mousePosition;
    }
}