using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuUIManager : MonoBehaviour
{
    
    public void PressPlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void PressExitButton()
    {
        Application.Quit();
    }

}
