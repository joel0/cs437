using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStartScript : MonoBehaviour {
    public float xSize;
    public float zSize;

    // Start is called before the first frame update
    void Start() {
        float x = (Random.value - 0.5f) * xSize;
        float z = (Random.value - 0.5f) * zSize;
        Vector3 position = new Vector3(x, transform.position.y, z);
        transform.position = position;
        CharacterController c = GetComponent<CharacterController>();
        if (c != null) {
            c.Move(position);
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
