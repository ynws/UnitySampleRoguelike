﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int playerDamage;    // 攻撃力。要リネーム

    private Animator animator;
    private Transform target;
    private bool skipMove;

    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    // Use this for initialization
    protected override void Start () {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    protected override void AttemptMove<T> (int xDir, int yDir)
    {
        // 2ターンに1度だけ動く
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);
        skipMove = true;
    }

    /// <summary>
    /// 敵の動きの制御
    /// </summary>
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        // PlayerとX座標が一致していればY方向に近寄る。どうでなければX方向に近寄る
        if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        AttemptMove<Player>(xDir, yDir);
    }

    /// <summary>
    /// Playerにぶつかった場合の処理
    /// </summary>
    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        hitPlayer.LoseFood(playerDamage);
        animator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
}
