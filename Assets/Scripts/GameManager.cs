using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // all possible spawnable targets
    [SerializeField] private GameObject[] targets, volumeSliders;
    // values determining how the targets will be spawned
    [SerializeField] private float ySpawnPos, xSpawnBounds, minSpawnRate, maxSpawnRate, minForce, maxForce, minTorq, maxTorq;
    // stores all targets currently on screen
    private List<Target> onScreenTargets = new List<Target>();
    // player score
    public int score = 0, lives;
    // Text elements for player score and whether the game is over
    public TextMeshProUGUI scoreText, livesText, tripleMeatText, oldScoreText, newScoreText;
    [SerializeField] private GameObject gameoverUI, titleScreenUI, pauseUI, highScoreUI;
    [SerializeField] private TMP_InputField nameInput;
    public bool gameOver = false, spawning = false, paused = false;
    private AudioSource audsc;
    public AudioClip gameOverAud;
    
    // Difficulty determined by which button was pressed at the title screen ...
    public void SetDifficulty(Difficulty difficulty)
    {
        diff = difficulty;
        switch (difficulty)
        {
            case Difficulty.Easy:
                minSpawnRate = .5f;
                maxSpawnRate = 1.5f;
                lives = 5;
                break;
            case Difficulty.Medium:
                minSpawnRate = .25f;
                maxSpawnRate = 1.2f;
                lives = 3;
                break;
            case Difficulty.Hard:
                minSpawnRate = .1f;
                maxSpawnRate = 1f;
                lives = 2;
                break;
            default:
                Debug.Log("Invalid difficulty");
                break;
        }
    }

    private Difficulty diff;

    private void Awake()
    {
        audsc = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        Cursor.visible = true;
        float vol = DataManager.GetVolume();
        audsc.volume = vol;
        foreach (var vs in volumeSliders)
        {
            vs.GetComponent<Slider>().value = vol;
        }
    }

    private void Update()
    {
        // Pause the game when pressing space ...
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PauseSwitch();
        }
    }
    
    // Pauses / Unpauses the game
    /**
     *  1st call : pauseUI not active .
     *  Time->0 when paused .
     *  UI shows when paused .
     *  Targets hidden when paused .
     *  Cursor visible when paused .
     */
    public void PauseSwitch()
    {
        if (!spawning) return;
        paused = !pauseUI.activeInHierarchy ;
        Time.timeScale = !paused? 1 : 0;
        pauseUI.SetActive(paused);
        foreach (Target t in onScreenTargets)
        {
            t.gameObject.SetActive(!paused);
        }
        Cursor.visible = paused;
        Swiper.Instance().PauseTR(paused);
        if (paused)
        {
            DataManager.Save();
        }
    }

    // Difficulty settings ...
    public enum Difficulty
    {
        Easy, Medium, Hard
    }

    // Start spawning Targets and Check whether they're in bounds every half second...
    public void StartGame()
    {
        titleScreenUI.SetActive(false);

        scoreText.gameObject.SetActive(true);
        livesText.gameObject.SetActive(true);
        livesText.text = "Lives: " + lives;

        StartCoroutine(nameof(SpawnTarget));
        spawning = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        
        InvokeRepeating(nameof(CheckBounds), 0f, .5f);
        
        UpdateScore(0);
        
        DataManager.Save();
    }
    
    // Checks all on screen targets for if they're in bounds on Y pos. Game over if good target falls... 
    private void CheckBounds()
    {
        if (onScreenTargets.Count == 0) return;
        
        foreach (Target t in onScreenTargets)
        {
            if (t.transform.position.y < ySpawnPos)
            {
                
                if (IsGameActive() && t.IsGood() && --lives < 1)
                {
                    GameOver();
                }

                livesText.text = "Lives: " + lives;
                Destroy(t.gameObject);
            }
        }
    }

    // Adds to / Removes from the list of on screen targets... 
    public void AddToList(Target t)
    {
        onScreenTargets.Add(t);
    }
    public void RemoveFromList(Target t)
    {
        onScreenTargets.Remove(t);
    }
    
    // Spawn a new target at ranged random intervals, apply ranged random upward force, apply ranged random torque... As long as game is still active ...
    private IEnumerator SpawnTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnRate, maxSpawnRate));
            if (gameOver) break;    // break here to stop the next spawn in queue ...
            int i = Random.Range(0, targets.Length);
            // if low on lives, give a chance for meat ...
            if (lives < 2 && Random.value < .25)
            {
                i = 3;
            }
            var t = Instantiate(targets[i]);
            t.transform.position = SpawnPos(t.transform.position);
            RandForceAndTorque(t.GetComponent<Rigidbody>());
        }
    }

    // return position at set y, ranged random x ...
    private Vector3 SpawnPos(Vector3 pos)
    {
        pos.y = ySpawnPos;
        pos.x = Random.Range(-xSpawnBounds, xSpawnBounds);
        return pos;
    }

    // add ranged random upward force, ranged random torque ...
    private void RandForceAndTorque(Rigidbody rb)
    {
        rb.AddForce(Vector3.up * Random.Range(minForce, maxForce), ForceMode.Impulse);
        rb.AddTorque(Random.Range(minTorq,maxTorq), Random.Range(minTorq,maxTorq), Random.Range(minTorq,maxTorq));
    }

    // Change the current score of the player by amount 'inc' . Change the on-screen text to reflect new score ...
    public void UpdateScore(int inc)
    {
        score += inc;
        scoreText.text = "Score: " + score;
    }

    // Whether the game is not over ... 
    public bool IsGameActive()
    {
        return !gameOver;
    }
    
    // Brings up the Game Over UI. Game manager stops running its ...
    public void GameOver()
    {
        if (score > DataManager.GetHighScore(diff))
        {
            highScoreUI.SetActive(true);
            oldScoreText.text = "Previous Score: " + DataManager.GetHighScoreHolder(diff) + " -> " +
                                DataManager.GetHighScore(diff);
            newScoreText.text = "Your Score: " + score;
        }
        else
        {
            gameoverUI.SetActive(true);
        }
        audsc.PlayOneShot(gameOverAud);
        gameOver = true;
        spawning = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        DataManager.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TripleMeat()
    {
        StartCoroutine(nameof(TripleMeatCR));
    }

    private IEnumerator TripleMeatCR()
    {
        tripleMeatText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        tripleMeatText.gameObject.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        audsc.volume = volume;
        foreach (GameObject go in volumeSliders)
        {
            Slider s = go.GetComponent<Slider>();
            s.value = audsc.volume;
        }

        DataManager.UpdateVolume(volume);
    }

    public void PlayOneShot(AudioClip clip)
    {
        audsc.PlayOneShot(clip);
    }

    public void SetHighScore()
    {
        DataManager.UpdateScore(diff, score, nameInput.text);
        RestartGame();
    }
}
