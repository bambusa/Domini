using System;

public class BuildingModel {

    // Callbacks
    Action<BuildingModel> cbPositionChanged;

    string name;
    string color;
    int posX;
    int posZ;

    public BuildingModel(string name, string color) {
        this.name = name;
        this.color = color;
        this.posX = -100;
        this.posZ = -100;
    }

    public BuildingModel(string name, string color, int posX, int posZ) {
        this.name = name;
        this.color = color;
        this.posX = posX;
        this.posZ = posZ;
    }

    public void CbRegisterPositionChanged(Action<BuildingModel> callback) {
        cbPositionChanged += callback;
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
}
