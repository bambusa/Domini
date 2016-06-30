using UnityEngine;using System.Collections;public class StaticCamera : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {        transform.Translate(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") * Mathf.Sin(transform.rotation.eulerAngles.x * (3.14f / 180)), Input.GetAxis("Vertical") * Mathf.Cos(transform.rotation.eulerAngles.x * (3.14f / 180)), Space.Self);
    }}