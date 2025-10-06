using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // The core function to preserve the object
        }
        else
        {
            Destroy(gameObject); // Destroy this duplicate object
        }
    }
}
