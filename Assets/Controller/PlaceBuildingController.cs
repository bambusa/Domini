using UnityEngine;
using System.Collections;

public class PlaceBuildingController : MonoBehaviour {

    public BuildingModel buildingModel;
    public GameObject mapLayer;

    Collider mapCollider;

    // Use this for initialization
    void Start () {
        mapCollider = mapLayer.GetComponent<Collider>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Place Building here");
            Destroy(this);
        }
        else if (mapLayer != null) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (mapCollider.Raycast(ray, out hit, 100.0F)) {
                int x = (int)Mathf.Floor(hit.point.x);
                int z = (int)Mathf.Floor(hit.point.z);
                buildingModel.SetPosition(x, z);
            }
                
        }
       
    }
}
