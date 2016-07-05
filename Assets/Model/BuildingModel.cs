using System;

/// <summary>
/// Data model of a building
/// </summary>
public class BuildingModel {

    // Callbacks
    Action<BuildingModel> cbPositionChanged;
    Action<BuildingModel> cbResourcesChanged;

    string name;
    string color;
    int posX;
    int posZ;
    float production;

    public BuildingModel(string name, string color) {
        this.name = name;
        this.color = color;
        this.posX = -100;
        this.posZ = -100;
        this.production = 10f;
    }

    public BuildingModel(string name, string color, int posX, int posZ, float production) {
        this.name = name;
        this.color = color;
        this.posX = posX;
        this.posZ = posZ;
        this.production = production;
    }

    public void SetName(string name) {
        this.name = name;
    }
    public string GetName() {
        return name;
    }

    public void SetPosition(int posX, int posZ) {
        if (this.posX != posX || this.posZ != posZ) {
            this.posX = posX;
            this.posZ = posZ;
            if (cbPositionChanged != null)
                cbPositionChanged(this);
        }
    }
    public int GetPositionX() {
        return posX;
    }
    public int GetPositionZ() {
        return posZ;
    }

    public void SetProduction(float production) {
        if (this.production != production) {
            this.production = production;
            cbResourcesChanged(this);
        }
    }
    public float GetProduction() {
        return production;
    }

    public void NotifyPlaced() {
        cbResourcesChanged(this);
    }

    /// <summary>
    /// Callback function for a change of position data
    /// </summary>
    /// <param name="callback">Callback function</param>
    public void CbRegisterPositionChanged(Action<BuildingModel> callback) {
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
