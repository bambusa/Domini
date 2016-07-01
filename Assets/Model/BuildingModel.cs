using System;

public class BuildingModel {

    public delegate void Callback(BuildingModel b);
    protected Callback placedCallback;

    public string name;
    public string color;
    public int posX;
    public int posZ;

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

    public void SetPlacedCallback(Callback callback) {
        placedCallback = callback;
    }

    public void placeBuilding(int posX, int posZ) {
        this.posX = posX;
        this.posZ = posZ;
        placedCallback(this);
    }

    public Boolean isPlaced() {
        return (posX > -1 && posZ > -1);
    }

}
