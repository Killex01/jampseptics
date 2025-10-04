using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General : MonoBehaviour
{
    [SerializeField] private Canvas test;
    [SerializeField] private Canvas levelOne;
    [SerializeField] private Canvas levelTwo;

    // Start is called before the first frame update
    void Start()
    {
        levelOne.enabled = false;
        levelTwo.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchScene(); //constantly check if h is pressed
    }

    public void SwitchScene()
    {
        if (Input.GetKeyDown(KeyCode.H)) //if h is pressed
        {
            test.enabled = false;
            levelOne.enabled = true;
            Debug.Log("Switch to level one");
        }

    }
}
