using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム全体の要素管理を行うシングルトンのクラス。
/// </summary>
public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    // 各種遷移にかかる時間
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;

    public BoardManager boardScript;

    public int playerFoodPoint = 100;
    [HideInInspector] public bool playerTurn = true;

    private bool doingSetup;

    private int level = 1;
    private Text levelText;
    private GameObject levelImage;

    private List<Enemy> enemies;
    private bool enemiesMoving;

    /// <summary>
    /// シーンが切り替わるごとに呼ばれる関数
    /// </summary>
    private void OnLevelWasLoaded(int index)
    {
        level++;
        InitGame();
    }

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
        if(playerTurn || enemiesMoving || doingSetup)
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
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        enabled = false;
    }

    void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
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
