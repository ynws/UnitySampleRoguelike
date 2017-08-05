using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム全体の要素管理を行うシングルトンのクラス。
/// </summary>
public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public BoardManager boardScript;

    // 初期ゲームレベル。デバック用に敵が出てくるレベルを初期レベルにしている。リリース時には1に。
    private int level = 3;

	// Use this for initialization
	void Awake () {
        if(instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            // ありえそうにないけど、GameManagerが複数生存するのを防止する
            Destroy(gameObject);
        }

        // シーンをまたがって状態を維持するための非消去設定
        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
        InitGame();
	}

    void InitGame()
    {
        boardScript.SetupScene(level);
    }
}
