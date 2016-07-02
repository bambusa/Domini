using System;
using UnityEngine;public class CameraMoveController : MonoBehaviour {

    public float dragSpeed = 7;
    public float scrollSpeed = 0.5f;

    Vector3 dragOrigin;
    Boolean dragging;

    // Use this for initialization
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            dragOrigin = Input.mousePosition;
            return;
        }
        else if (Input.GetMouseButton(0)) {
            Vector3 drag = Input.mousePosition;
            Vector3 pos = Camera.main.ScreenToViewportPoint(drag - dragOrigin);
            Vector3 move = new Vector3(pos.x * dragSpeed * -1, 0, pos.y * dragSpeed * -1);
            Camera.main.transform.Translate(move, Space.World);
            dragOrigin = drag;
        }
        else {
            // WASD
            Vector3 move = new Vector3(Input.GetAxis("Horizontal") * scrollSpeed * -1, 0, Input.GetAxis("Vertical") * scrollSpeed * -1);
            Camera.main.transform.Translate(new Vector3(Input.GetAxis("Horizontal") * scrollSpeed, Input.GetAxis("Vertical") * scrollSpeed, 0));
        }
    }
}