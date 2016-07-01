using UnityEngine;
using System.Collections;

public class CameraInteractionController : MonoBehaviour {

    public GameObject mapLayer;
    Vector3 dragOrigin;

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            // Left mouse button clicked
            dragOrigin = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0)) {
            // Left mouse button held down
            Vector3 dragEnd = Input.mousePosition;
            if (dragOrigin == dragEnd) {
                //Debug.Log("not dragged");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit)) {
                    if (hit.transform.name == "Cube") {
                        //Debug.Log("This is a Cube");
                    }
                    else {
                        //Debug.Log("This isn't a Cube");
                    }
                }
            }
        }
    }

    bool V3Equal(Vector3 a, Vector3 b) {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }
}
