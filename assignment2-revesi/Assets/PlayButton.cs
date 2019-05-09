using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour {
    public static int Difficulty = 1;
    public UnityEngine.UI.Slider DifficultySlider;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnMouseUp() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void SetDifficulty() {
        Difficulty = Mathf.RoundToInt(DifficultySlider.value);
    }
}
