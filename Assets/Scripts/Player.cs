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
	void Update () {
        // Playerのターンでなければ以下の処理不要
        if (!GameManager.instance.playerTurn) return;

        // ユーザ入力を受け取る
        int horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        int vertical = (int)(Input.GetAxisRaw("Vertical"));

        // 斜め移動不可
        if (horizontal != 0) vertical = 0;

        if(horizontal!=0 || vertical != 0)
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
