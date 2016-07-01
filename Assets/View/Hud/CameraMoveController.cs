using System;
using UnityEngine;using UnityEngine.EventSystems;public class CameraMoveController : MonoBehaviour, IPointerClickHandler {

    public GameObject mapLayer;
    public float dragSpeed = 7;
    public float scrollSpeed = 0.5f;

    Vector3 dragOrigin;
    Collider mapCollider;

    // Use this for initialization
    void Start() {
        mapCollider = mapLayer.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (mapCollider.Raycast(ray, out hit, 100.0F)) {
            if (Input.GetMouseButtonDown(0)) {
                // Left mouse button clicked
                dragOrigin = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0)) {
                // Left mouse button held down
                Vector3 pos = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
                Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);
                transform.Translate(move, Space.World);
                dragOrigin = Input.mousePosition;
            }
            else {
                // WASD
                Vector3 move = new Vector3(Input.GetAxis("Horizontal") * scrollSpeed, 0, Input.GetAxis("Vertical") * scrollSpeed);
                transform.Translate(move, Space.World);
            }
        }
        else {
            // WASD
            Vector3 move = new Vector3(Input.GetAxis("Horizontal") * scrollSpeed, 0, Input.GetAxis("Vertical") * scrollSpeed);
            transform.Translate(move, Space.World);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        throw new NotImplementedException();
    }}