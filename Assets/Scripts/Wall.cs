using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 壊せる壁
/// </summary>
public class Wall : MonoBehaviour {

    public Sprite dmgSprite;
    public int hp = 4;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	public void DamageWall (int loss) {
        spriteRenderer.sprite = dmgSprite;
        hp -= loss;
        if(hp <= 0)
        {
            gameObject.SetActive(false);
        }
	}
}
