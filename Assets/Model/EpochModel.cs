using UnityEngine;

public class EpochModel : MonoBehaviour {

	private long epoch_id;
    private string name;
    private string description;

    public EpochModel(long epoch_id, string name, string description) {
        this.epoch_id = epoch_id;
        this.name = name;
        this.description = description;
    }

    private long GetId() {
        return epoch_id;
    }

    private string GetName() {
        return name;
    }

    private string GetDescription() {
        return description;
    }
}
