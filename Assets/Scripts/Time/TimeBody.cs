using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimeBody : MonoBehaviour
{
    public bool isRewinding = false;
    public bool isReplaying = false;

    ShaderEffect_CRT crt;


    List<PointInTime> pointsInTime;
    Animator animator;
    SpriteRenderer spriteRenderer;

    [SerializeField]
    AudioSource BGM;

    PlayerController playerDead;

    // Start is called before the first frame update
    void Start()
    {
        pointsInTime = new List<PointInTime>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        crt = GameObject.Find("Main Camera").GetComponent<ShaderEffect_CRT>();
        playerDead = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDead.rewinding)
        {
            StartRewind();
        }
        if (playerDead.replaying)
        {
            StartReplay();
        }
    }

    private void FixedUpdate()
    {
        if (isRewinding)
        {
            Rewind();
            SineWave.instance.ScreenWave();

        }
        else if (isReplaying)
        {
            Replay();
        }
        else
        {
            if(playerDead.isRecording)
                Record();
        }
    }

    void Rewind()
    {
        if (pointsInTime.Count > 0)
        {
            PointInTime pointInTime = pointsInTime[0];
            this.transform.position = pointInTime.position;
            spriteRenderer.sprite = pointInTime.sprite;
            transform.localScale = pointInTime.scale;
            transform.rotation = pointInTime.rotation;

            pointsInTime.RemoveAt(0);
            if (BGM != null)
            {
                BGM.Stop();
            }

        }
        else
        {
            StopRewind();
        }
    }

    void Replay()
    {
        if (pointsInTime.Count > 0)
        {
            PointInTime pointInTime = pointsInTime[pointsInTime.Count - 1];
            this.transform.position = pointInTime.position;
            spriteRenderer.sprite = pointInTime.sprite;
            transform.localScale = pointInTime.scale;
            transform.rotation = pointInTime.rotation;

            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }
        else
        {
            StopRewind();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            pointsInTime = new List<PointInTime>();

            StopRewind();
        }
    }



    void Record()
    {
        pointsInTime.Insert(0, new PointInTime(transform.position, spriteRenderer.sprite, transform.localScale, transform.rotation));
    }

    public void StartRewind()
    {
        playerDead.isRecording = false;

        crt.enabled = true;

        Time.timeScale = 5f;
        if (animator != null)
        {
            animator.enabled = false;
        }
        isRewinding = true;

    }
    public void StopRewind()
    {
        crt.enabled = false;
        playerDead.rewinding = false;
        playerDead.BC.enabled = true;
        playerDead.replaying = false;

        if (playerDead.clear)
        {
            playerDead.stageMove = true;
        }

        Time.timeScale = 1f;

        SineWave.instance.StopWave();


        if (animator != null)
        {
            animator.enabled = true;
        }
        isRewinding = false;
        isReplaying = false;

        if (BGM != null)
        {
            BGM.Play();
        }
    }

    public void StartReplay()
    {
        crt.enabled = true;
        playerDead.isRecording = false;

        Time.timeScale = 1f;
        CameraController.instance.ColorToGray();

        if (animator != null)
        {
            animator.enabled = false;
        }
        isReplaying = true;
    }



}
