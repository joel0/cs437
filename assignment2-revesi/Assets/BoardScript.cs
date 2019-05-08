using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour {
    public GameObject Piece;

    // Start is called before the first frame update
    void Start() {
        //Piece.GetComponent<Rigidbody>().isKinematic = true;
        //Animator anim = Piece.GetComponent<Animator>();
        //anim.Play("flip");

        //foreach (AnimationState state in anim) {
        //    state.speed = 0.5f;
        //}
        //anim.Play();
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnMouseOver() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, 100);

        if (hit.rigidbody.gameObject == gameObject) {
            Vector3 location = hit.point;
            location.y = 3;
            Piece.transform.position = location;
        }
    }
}
