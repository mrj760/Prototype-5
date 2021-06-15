using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ExitReturnButton : MonoBehaviour
{
    private Button button;
    private GameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        button = GetComponent<Button>();
    }

    public void Exit()
    {
        DataManager.Save();
        
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void Return()
    {
        gameManager.PauseSwitch();
        // gameManager.GameOver();
        gameManager.RestartGame();
    }
}
