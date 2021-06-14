using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{

    private Button button;
    private GameManager gameManager;
    [SerializeField] private GameManager.Difficulty difficulty;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(SetDifficulty);
    }

    public void SetDifficulty()
    {
        gameManager.SetDifficulty(difficulty);
    }

    public void StartGame()
    {
        gameManager.StartGame();
    }
}
