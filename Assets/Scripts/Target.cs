using UnityEngine;

public class Target : MonoBehaviour
{
    private GameManager gm; 
    
    [SerializeField] private ParticleSystem part;
    [SerializeField] private AudioClip aud, tripleMeat;
    [SerializeField] private bool isMeat;

    private static int meatCount = 0;

    private enum GoodOrBad
    {
        Good,
        Bad
    }

    [SerializeField] private GoodOrBad goodOrBad;
    
    // Add to tracked list
    void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        gm.AddToList(this);
    }

    private void OnDestroy()
    {
        // Remove from Tracked list
        gm.RemoveFromList(this);
    }

    // Play particles, update score, destroy g.o.
    private void OnMouseDown()
    {
        if (gm.IsGameActive())
        {
            DestroyTarget();
        }
    }

    public void DestroyTarget()
    {
        Transform tx = transform;
        Instantiate(part, tx.position, tx.rotation);
            
        if (isMeat)
        {
            if (++meatCount > 2)
            {
                gm.PlayOneShot(tripleMeat);
                meatCount = 0;
                gm.TripleMeat();
                gm.lives++;
                gm.livesText.text = "Lives: " + gm.lives;
            }
            else
            {
                gm.PlayOneShot(aud);
            }
        }
        else
        {
            meatCount = 0;
            gm.PlayOneShot(aud);
        }

        switch (goodOrBad)
        {
            case (GoodOrBad.Good):
                gm.UpdateScore(10);
                break;
            case (GoodOrBad.Bad):
                gm.UpdateScore(-30);
                gm.lives--;
                gm.livesText.text = "Lives: " + gm.lives;
                if (gm.lives < 1)
                    gm.GameOver();
                break;
            default:
                Debug.Log("Invalid Good or Bad");
                break;
        }

        Destroy(gameObject);  
    }

    public bool IsGood()
    {
        return goodOrBad == GoodOrBad.Good;
    }
}
