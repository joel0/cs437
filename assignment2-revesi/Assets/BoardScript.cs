using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour {
    public GameController GameController;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnMouseUp() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 100);

        if (hit.rigidbody.gameObject == gameObject) {
            Vector3 location = hit.point;
            int row;
            int col;
            row = Mathf.RoundToInt(location.z + 3.5f);
            col = Mathf.RoundToInt(location.x + 3.5f);
            GameController.PlayerMove(row, col);
        }
    }
}
