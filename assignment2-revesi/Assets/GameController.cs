using ReversiTournament;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject PieceModel;

    List<GameObject> mActivePieces = new List<GameObject>(64);
    GameObject[,] mBoardGrid = new GameObject[8, 8];
    bool?[,] mBoardWhite = new bool?[8, 8];
    int mPiecesOnBoard = 0;
    List<AnimationSteps> mPendingAnimations = new List<AnimationSteps>();
    JoelAI mAI = new JoelAI(3);
    bool mGridNeedsSync = false;

    // Start is called before the first frame update
    void Start() {
        for (int row = 0; row < 8; row++) {
            for (int col = 0; col < 7; col++) {
                GameObject newPiece = Instantiate(PieceModel);
                newPiece.transform.position = CalculateCoordStock(row, col);
                mActivePieces.Add(newPiece);
            }
        }

        PlacePiece(3, 3, true);
        PlacePiece(3, 4, false);
        PlacePiece(4, 4, true);
        PlacePiece(4, 3, false);

        mAI.Reset(ReversiCommon.TokenColor.WHITE, ReversiCommon.TokenColor.WHITE);
        Move m = mAI.GetMove();
        mAI.MakeMove(m);
        PlacePiece(m.Row, m.Col, true);
    }

    // Update is called once per frame
    void Update() {
        if (mPendingAnimations.Count > 0 &&
            mPendingAnimations[0].AnimationObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Static")) {
            GameObject obj = mPendingAnimations[0].AnimationObj;
            object step = mPendingAnimations[0].Steps[0];
            if (step is Vector3 target) {
                Vector3 direction = target - obj.transform.position;
                if (direction.magnitude < 0.1f) {
                    obj.transform.position = target;
                    mPendingAnimations[0].Steps.RemoveAt(0);
                } else {
                    direction.Normalize();
                    obj.transform.position += direction * 0.05f;
                }
            } else if (step is string) {
                Animator animator = obj.GetComponent<Animator>();
                if (!animator.IsInTransition(0)) {
                    animator.Play("flip");
                }
                mPendingAnimations[0].Steps.RemoveAt(0);
            }
            if (mPendingAnimations[0].Steps.Count == 0) {
                mPendingAnimations.RemoveAt(0);
                if (mPendingAnimations.Count == 0 && mGridNeedsSync) {
                    SyncBoard();
                }
            }
        }
    }

    void PlacePiece(int row, int col, bool white) {
        AnimationSteps anim;
        anim.AnimationObj = mActivePieces[mPiecesOnBoard];
        mBoardWhite[row, col] = white;
        mBoardGrid[row, col] = mActivePieces[mPiecesOnBoard];
        anim.Steps = new List<object>
        {
            CalculateCoord(row, col, true)
        };
        if (white) {
            anim.Steps.Add("flip");
        }
        anim.Steps.Add(CalculateCoord(row, col));
        mPendingAnimations.Add(anim);

        mGridNeedsSync = true;
        mPiecesOnBoard++;
    }

    void FlipPiece(GameObject piece) {
        AnimationSteps anim;
        anim.AnimationObj = piece;
        anim.Steps = new List<object>
        {
            piece.transform.position + Vector3.up * 2,
            "flip",
            piece.transform.position
        };
        mPendingAnimations.Add(anim);
    }

    void SyncBoard() {
        for (int row = 0; row < 8; row++) {
            for (int col = 0; col < 8; col++) {
                if (mBoardWhite[row, col] == true &&
                    mAI.Board[col, row] != JoelAI.BoardToken.WHITE) {
                    FlipPiece(mBoardGrid[row, col]);
                    mBoardWhite[row, col] = !mBoardWhite[row, col];
                } else if (mBoardWhite[row, col] == false &&
                    mAI.Board[col, row] != JoelAI.BoardToken.BLACK) {
                    FlipPiece(mBoardGrid[row, col]);
                    mBoardWhite[row, col] = !mBoardWhite[row, col];
                }
            }
        }
        mGridNeedsSync = false;
    }

    Vector3 CalculateCoord(int row, int col, bool hover = false) {
        float y = 1.12f;
        float z = row / 7f * 7 - 3.5f;
        float x = col / 7f * 7 - 3.5f;
        if (hover) {
            y += 2;
        }
        return new Vector3(x, y, z);
    }

    Vector3 CalculateCoordStock(int row, int col) {
        Vector3 location = CalculateCoord(row, col);
        location.y = 2.2f;
        if (col < 4) {
            location.x -= 5.75f;
        } else {
            location.x += 5.75f;
        }
        return location;
    }

    struct AnimationSteps {
        public GameObject AnimationObj;
        public List<object> Steps;
    }
}
