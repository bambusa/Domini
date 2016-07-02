using UnityEngine;
using System.Collections;

public class PlaceBuildingController : MonoBehaviour {

    public BuildingModel buildingModel;
    public GameObject mapLayer;

    Collider mapCollider;
    int posX = -100;
    int posZ = -100;

    // Use this for initialization
    void Start () {
        mapCollider = mapLayer.GetComponent<Collider>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && posX >= 0 && posZ >= 0) {
            Debug.Log("Place at " + posX + "/" + posZ);
            buildingModel.placeBuilding(posX, posZ);
            Destroy(this);
        }
        else if (mapLayer != null && !buildingModel.isPlaced()) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (mapCollider.Raycast(ray, out hit, 100.0F)) {
                int x = (int)Mathf.Floor(hit.point.x);
                int z = (int)Mathf.Floor(hit.point.z);
                if (posX != x || posZ != z) {
                    posX = x;
                    posZ = z;
                    Debug.Log("Map coordinate " + posX + "/" + posZ);
                    transform.position = new Vector3(posX + 0.5f, 0.5f, posZ + 0.5f);
                }
            }
                
        }
       
    }
}
