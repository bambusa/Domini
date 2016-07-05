using System;
using UnityEngine;public class CameraMoveController : MonoBehaviour {

    public float dragSpeed = 5;
    public float keySpeed = 0.5f;
    public float scrollSpeed = 1;
    public float minZoom = 5;
    public float maxZoom = 100;

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
            Vector3 move = new Vector3(Input.GetAxis("Horizontal") * keySpeed, Input.GetAxis("Vertical") * keySpeed, 0);
            Camera.main.transform.Translate(move);
        }
        Vector3 zoom = new Vector3(0, Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * -1);
        if ((Camera.main.transform.position.y > minZoom && Camera.main.transform.position.y < maxZoom) ||
            (Camera.main.transform.position.y <= minZoom && zoom.y > 0) ||
            (Camera.main.transform.position.y >= maxZoom && zoom.y < 0)) {
            // Zoom
            Camera.main.transform.Translate(new Vector3(0, Input.GetAxis("Mouse ScrollWheel") * Camera.main.transform.position.y * scrollSpeed * -1), 0);
        }
    }
}