using UnityEngine;
using System.Collections;

public class HudButtonListener : MonoBehaviour {

    public GameObject buildingLayer;
    public GameObject buildingMenuPanel;

    enum STATE { WORLD, BUILDING_MENU };
    STATE state = STATE.WORLD;
    BuildingLayerController buildingController;
    Renderer buildingMenuPanelRenderer;

    void onStart() {
        buildingController = buildingLayer.GetComponent<BuildingLayerController>();
    }

    public void onClickBuildButton() {
        Debug.Log("BuildButton clicked");
        if (state == STATE.WORLD) {
            state = STATE.BUILDING_MENU;
            buildingMenuPanel.SetActive(true);
        }
    }

    public void onClickBuildingButton() {
        if (state == STATE.BUILDING_MENU) {
            state = STATE.WORLD;
            buildingMenuPanel.SetActive(false);
        }        
    }
}
