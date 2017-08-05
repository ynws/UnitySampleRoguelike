using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームマネージャをロードするためのローダークラス。
/// カメラに張り付けて使う。
/// </summary>
public class Loader : MonoBehaviour {

    public GameObject gameManager;

	void Awake()
    {
        if(GameManager.instance == null)
        {
            Instantiate(gameManager);
        }
    }
}
