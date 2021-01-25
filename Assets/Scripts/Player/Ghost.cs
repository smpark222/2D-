using System.Collections;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    Animator ghostAni;

    GameObject player;
    Transform playerPos;

    SpriteRenderer playerSprite;
    SpriteRenderer GhostSprite;
    PlayerController playerController;

    ColorManager colorManager;

    public Color[] colors =
    {
        Color.magenta,
        Color.cyan,
    };



    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Player");
        playerPos = player.GetComponent<Transform>();
        playerSprite = player.GetComponent<SpriteRenderer>();
        playerController = player.GetComponent<PlayerController>();

        colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();
        GhostSprite = this.GetComponent<SpriteRenderer>();
        ghostAni = this.GetComponent<Animator>();
        //GhostSprite.color = colors[Random.Range(0, colors.Length)];

    }
    private void Update()
    {
        if (playerController.isEvading ||playerController.wallJump || colorManager.speedAttack)
        {
            ghostAni.speed = 3f;
        }
        else
        {
            ghostAni.speed = 10f;
        }

    }


    public IEnumerator PlayAni()
    {
        GhostSprite.sprite = playerSprite.sprite;
        this.transform.position = playerPos.position;
        ghostAni.SetTrigger("Ghost");
        this.transform.localScale = playerPos.localScale * 1.04f;
        yield return null;
    }
}
