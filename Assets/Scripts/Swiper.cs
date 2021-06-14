using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(TrailRenderer), typeof(BoxCollider))]
public class Swiper : MonoBehaviour
{
    private GameManager gm;
    private Camera cam;
    private Vector3 pos;
    private TrailRenderer tr;
    private BoxCollider box;
    private ParticleSystem part;
    private bool swiping = false;

    // Singleton ...
    private static Swiper swiper;
    public static Swiper Instance()
    {
        return swiper;
    }
    

    // Initialize variables and set trail/collider to false ...
    private void Awake()
    {
        // Singleton
        if (swiper == null)
        {
            swiper = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // GM, Cam, Trail Renderer, and Collider ...
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        (tr = GetComponent<TrailRenderer>()).enabled = false;
        (box = GetComponent<BoxCollider>()).enabled = false;
    }

    private void Start()
    {
        part = GetComponent<ParticleSystem>();
        swiping = true;
        UpdateComponents();
    }

    private void Update()
    {
        // as long as spawning, track mouse ...
        if (gm.spawning)
        {
            UpdateMousePosition();
            // if (Input.GetMouseButtonDown(0))
            // {
            //     swiping = true;
            //     UpdateComponents();
            // }
            // else if (Input.GetMouseButtonUp(0))
            // {
            //     swiping = false;
            //     UpdateComponents();
            // }
            // if (swiping)
            // {
            //     UpdateMousePosition();
            // }
        }

        // Game Over, Destroy ...
        if (gm.gameOver)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    ///   <para>Disables the Player Indicator if true</para>
    /// </summary>
    /// <param name="pause">Whether to be paused</param>
    public void PauseTR (/* whether the game is paused*/bool pause)
    {
        // Swiper Trail Renderer and particles Active when Not paused
        tr.enabled = !pause;
        part.gameObject.SetActive(!pause);
    }

    private void UpdateMousePosition()
    {
        pos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9.286747f));     // cam z-pos is: -9.286747f .. so offset where to spawn swiper
        transform.position = pos;
    }

    private void UpdateComponents()
    {
        tr.enabled = swiping;
        box.enabled = swiping;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Target>())
        {
            other.gameObject.GetComponent<Target>().DestroyTarget();
        }
    }
}
