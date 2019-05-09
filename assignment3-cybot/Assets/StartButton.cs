using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour {
    public static int cyCount = 2;
    public Slider CySlider;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnClick() {
        SceneManager.LoadScene(1);
    }

    public void OnSlider() {
        cyCount = Mathf.RoundToInt(CySlider.value);
    }
}
