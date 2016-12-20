using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Controller for interaction with game objects
/// </summary>
public class CameraInteractionController : MonoBehaviour {

    public GameObject mapLayer;
    public GameObject infoPanel;
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
            // Left mouse button released
            Vector3 dragEnd = Input.mousePosition;

            // clicked, not dragged
            if (dragOrigin == dragEnd) {
                if (infoPanel.activeSelf) {
                    infoPanel.SetActive(false);
                }
                else { 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                    if (Physics.Raycast(ray, out hit)) {
                        Debug.Log("This is a " + hit.transform.name);
                        BuildingObjectController controller = hit.collider.GetComponent<BuildingObjectController>();
                        if (controller != null) {
                            BuildingModel building = controller.GetBuildingModel();
                            if (building != null) {
                                Debug.Log(building.buildingType.GetName());
                                foreach (Text text in infoPanel.GetComponentsInChildren<Text>()) {
                                    if (text.name.Equals("Title")) {
                                        text.text = building.buildingType.GetName();
                                    }
                                    else if (text.name.Equals("Description Content")) {
                                        text.text = building.buildingType.GetDescription();
                                    }
                                    else if (text.name.Equals("Level Content")) {
                                        text.text = building.GetLevel().ToString();
                                    }
                                    else if (text.name.Equals("Position Content")) {
                                        text.text = building.GetPositionX() + "/" + building.GetPositionZ();
                                    }
                                }
                                infoPanel.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
    }

    bool V3Equal(Vector3 a, Vector3 b) {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }
}
