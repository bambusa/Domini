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
        Debug.Log("Hud");
        if (buildingLayer == null) {
            Debug.LogError("BuildingLayer not set");
        }
        buildingController = buildingLayer.GetComponent<BuildingLayerController>();
        if (buildingController == null) {
            Debug.LogError("BuildingController not found");
        }
    }

    public void onClickBuildButton() {
        Debug.Log("BuildButton clicked");
        if (state == STATE.WORLD) {
            state = STATE.BUILDING_MENU;
            buildingMenuPanel.SetActive(true);
        }
    }
}
