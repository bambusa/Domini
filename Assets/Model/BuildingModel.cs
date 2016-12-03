using System;
using System.Collections.Generic;
using UnityEngine; // TODO: Delete

/// <summary>
/// Data model of a building
/// </summary>
public class BuildingModel {

    // Callbacks
    private Action<BuildingModel> cbPositionChanged;
    private Action<BuildingModel> cbResourcesChanged;

    public BuildingTypesModel buildingType;
    private int posX;
    private int posZ;

    public BuildingModel(BuildingTypesModel buildingTypesModel) {
        this.buildingType = buildingTypesModel;
    }

    public void SetPosition(int posX, int posZ) {
        if (this.posX != posX || this.posZ != posZ) {
            //Debug.Log("Changed Position: " + posX + "/" + posZ);
            this.posX = posX;
            this.posZ = posZ;
            if (cbPositionChanged != null)
                //Debug.Log("cbPositionChanged");
                cbPositionChanged(this);
        }
    }
    public int GetPositionX() {
        return posX;
    }
    public int GetPositionZ() {
        return posZ;
    }

    public void NotifyPlaced() {
        cbResourcesChanged(this);
    }

    /// <summary>
    /// Callback function for a change of position data
    /// </summary>
    /// <param name="callback">Callback function</param>
    public void CbRegisterPositionChanged(Action<BuildingModel> callback) {
        //Debug.Log("CbRegisterPositionChanged");
        cbPositionChanged += callback;
    }

    /// <summary>
    /// Callback function for a change of resource input, output or storage data
    /// </summary>
    /// <param name="callback">Callback function</param>
    public void CbRegisterResourcesChanged(Action<BuildingModel> callback) {
        cbResourcesChanged += callback;
    }
}
