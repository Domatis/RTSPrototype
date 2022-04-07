using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{

    public static GameplayManager instance;

    public Action GameWinAction;
    public Action GameLoseAction;


    private void Awake() 
    {
        instance = this;
    }


    public void GameWin()
    {
        GameWinAction?.Invoke();
        //Reach the ui.
        GameplayUIManager.instance.OpenGameWinPanel();
    }

    public void GameLose()
    {
        GameLoseAction?.Invoke();
        GameplayUIManager.instance.OpenGameLosePanel();
    }
}
