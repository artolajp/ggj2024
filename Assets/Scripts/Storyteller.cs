using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Storyteller : MonoBehaviour
{
    [SerializeField] private List<GameObject> fotos;
    [SerializeField] private List<float> startAt;
    [SerializeField] private float endAt;

    public static bool isFTUE = true;
    
    private int clickCount = 0;

    [SerializeField]private float elapsedTime;

    private void Start()
    {
        elapsedTime = 0;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        for (int i = 0; i < fotos.Count; i++)
        {
            fotos[i].SetActive(startAt[i]<elapsedTime);   
        }
        if (endAt < elapsedTime)
        {
            SceneManager.LoadScene("Game");
            isFTUE = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;
            if (!isFTUE || clickCount >= 3)
            {
                SceneManager.LoadScene("Game");
            }
        }
    }
}
