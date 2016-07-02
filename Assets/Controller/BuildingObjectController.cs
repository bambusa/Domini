using UnityEngine;

[RequireComponent(typeof(MeshFilter))][RequireComponent(typeof(MeshRenderer))][RequireComponent(typeof(MeshCollider))]
public class BuildingObjectController : MonoBehaviour {

    public BuildingModel buildingModel;

	// Use this for initialization
	void Start () {
        Debug.Log("BuildingObjectController started");
        if (buildingModel != null) {
            buildingModel.CbRegisterPositionChanged(OnBuildingPositionChanged);
        }
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    void OnBuildingPositionChanged(BuildingModel b) {
        Debug.Log("Building was moved");
        gameObject.transform.position = new Vector3(b.GetPositionX() + 0.5f, 0.5f, b.GetPositionZ() + 0.5f);
    }
}
