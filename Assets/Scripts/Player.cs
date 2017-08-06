using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MovingObject {

    public int wallDamege = 1;  // 壁に与えるダメージ
    public int pointsPerFood = 10;  // アイテム取得で回復するfood量。本来はItemが持つべきだろう
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;

    private Vector2 touchOrigin = -Vector2.one;

    // Use this for initialization
    protected override void Start () {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoint;
        foodText.text = "Food: " + food;
        base.Start();
	}

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoint = food;
    }

    // Update is called once per frame
    void Update()
    {
        // Playerのターンでなければ以下の処理不要
        if (!GameManager.instance.playerTurn) return;

        // ユーザ入力を受け取る
        int horizontal = 0;
        int vertical = 0;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        // PCなど、カーソルキーがある場合の入力処理
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));
        // 斜め移動不可
        if (horizontal != 0) vertical = 0;
#else
        // スマホなど、キー入力できない場合、スワイプで操作
        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];
            if (myTouch.phase == TouchPhase.Began)
            {
                // タッチ開始位置を記録
                touchOrigin = myTouch.position;
            }
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                // タッチ終了かつ、開始位置が設定済みなら、移動処理。
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;
                // X, Yのうち、よりスワイプされているほうに進む
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    horizontal = x > 0 ? 1 : -1;
                else
                    vertical = y > 0 ? 1 : -1;
            }
        }
#endif

        if (horizontal != 0 || vertical != 0)
        {
            // 移動先に壁を想定しておく
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T> (int xDir, int yDir)
    {
        food--;
        foodText.text = "Food: " + food;

        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        if(Move(xDir, yDir, out hit)){
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();
        GameManager.instance.playerTurn = false;
    }

    /// <summary>
    /// 移動できない場合の処理。現時点では壁がある時だけ呼ばれる
    /// </summary>
    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamege);
        animator.SetTrigger("playerChop");
    }

    /// <summary>
    /// 衝突処理
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if(other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if(other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
    }

    private void Restart()
    {
        // 警告出てるけど、チュートリアルなので許してください
        Application.LoadLevel(Application.loadedLevel);
    }

    /// <summary>
    /// Foodを失う処理。このゲームでは、ダメージをFoodで表すのでなんとなく感覚とずれるけど。
    /// </summary>
    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if(food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }
}
