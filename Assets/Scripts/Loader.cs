using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラに張り付けてマネージャをロードするためのローダー
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
