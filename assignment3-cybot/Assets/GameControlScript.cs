using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class GameControlScript : MonoBehaviour {
    public GameObject WinTarget;
    public GameObject Player;
    public GameObject Powerup;
    public GameObject[] CyBots;
    float mPowerupEndTime = -1;
    bool mPowerupActive = false;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (DistBetween(Player, WinTarget) < 3) {
            SceneManager.LoadScene(2);
        }
        if (Powerup != null && DistBetween(Player, Powerup) < 3) {
            Player.GetComponent<FirstPersonController>().m_WalkSpeed = 15;
            Destroy(Powerup);
            Powerup = null;
            mPowerupActive = true;
            mPowerupEndTime = Time.time + 7;
        }
        int closeCys = 0;
        for (int i = 0; i < CyBots.Length; i++) {
            if (DistBetween(Player, CyBots[i]) < 8) {
                closeCys++;
            }
        }
        if (closeCys > 1) {
            SceneManager.LoadScene(3);
        }
        if (mPowerupActive && mPowerupEndTime < Time.time) {
            Player.GetComponent<FirstPersonController>().m_WalkSpeed = 5;
            mPowerupActive = false;
        }
    }

    float DistBetween(Vector3 obj1, Vector3 obj2) {
        return (obj1 - obj2).magnitude;
    }

    float DistBetween(GameObject obj1, GameObject obj2) {
        return DistBetween(obj1.transform.position, obj2.transform.position);
    }
}
