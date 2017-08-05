using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// セルに沿った動きをするオブジェクトのabstract class
/// </summary>
public abstract class MovingObject : MonoBehaviour {

    public float moveTiime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

	// Use this for initialization
	protected virtual void Start () {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTiime;

	}

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        // 当たり判定
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if(hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }

    /// <summary>
    /// セル間の移動をスムーズに行うための関数
    /// </summary>
    /// <param name="end"></param>
    /// <returns></returns>
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        // 残り移動距離
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        // 残り距離が微小になるまでちょっとずつ動かす
        while(sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        // 移動先に何もないなら以下の処理は不要
        if(hit.transform == null)
        {
            return;
        }

        // 移動先にあるものを取得
        T hitComponent = hit.transform.GetComponent<T>();

        // 移動先に何かあって移動できない場合の処理
        if(!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
