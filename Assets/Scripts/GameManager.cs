using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム全体の要素管理を行うシングルトンのクラス。
/// </summary>
public class GameManager : MonoBehaviour {

    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoint = 100;
    [HideInInspector] public bool playerTurn = true;

    // 初期ゲームレベル。デバック用に敵が出てくるレベルを初期レベルにしている。リリース時には1に。
    private int level = 3;
    private List<Enemy> enemies;
    private bool enemiesMoving;

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

        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
	}

    void Update()
    {
        if(playerTurn || enemiesMoving)
        {
            return;
        }

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    public void GameOver()
    {
        enabled = false;
    }

    void InitGame()
    {
        enemies.Clear();
        boardScript.SetupScene(level);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        if(enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playerTurn = true;
        enemiesMoving = false;
    }
}
