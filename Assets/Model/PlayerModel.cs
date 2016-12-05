using UnityEngine;
using System.Collections;

public class PlayerModel : MonoBehaviour {

    private string username;
    private string email;

    public PlayerModel(string username, string email) {
        this.username = username;
        this.email = email;
    }

    public string GetUsername() {
        return username;
    }

    public string GetEmail() {
        return email;
    }
}
