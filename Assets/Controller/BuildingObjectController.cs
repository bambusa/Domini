using UnityEngine;

/// <summary>
/// Controller for an individual GameData building object,
/// manage interaction and rendering
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class BuildingObjectController : MonoBehaviour {

    private BuildingModel buildingModel;

	//// Use this for initialization
	//void Start () {

	//}
	
	//// Update is called once per frame
	//void Update () {
        
 //   }
 
    /// <summary>
    /// Callback for changed position data of a building,
    /// render new location
    /// </summary>
    /// <param name="buildingModel">BuildingModel which position changed</param>
    private void OnBuildingPositionChanged(BuildingModel buildingModel) {
        Debug.Log("Building was moved");
        gameObject.transform.position = new Vector3(this.buildingModel.GetPositionX() + 0.5f, 0.5f, this.buildingModel.GetPositionZ() + 0.5f);
    }

    public void SetReferences(BuildingModel buildingModel, bool placeInstantly) {
        this.buildingModel = buildingModel;
        this.buildingModel.CbRegisterPositionChanged(OnBuildingPositionChanged);
        if (placeInstantly) OnBuildingPositionChanged(this.buildingModel);
    }

    public BuildingModel GetBuildingModel() {
        return buildingModel;
    }
}
