using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitEffect : MonoBehaviour
{
    public Vector2 dir;
    SpriteRenderer color;

    // Start is called before the first frame update
    void Start()
    {
        AniManager.instance.EffectDirection(this.gameObject, dir, this.transform.localScale);

        color = GetComponent<SpriteRenderer>();
        Destroy(gameObject, 20f);
        StartCoroutine(effectColor());

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Time.fixedDeltaTime * 400f);
    }
    
    IEnumerator effectColor()
    {
        while (true)
        {
            color.color = new Color(0f, 1f, 0.970f,0.5f);
            yield return new WaitForSeconds(0.03f);
            color.color = new Color(1f, 0f, 0.943f,0.5f);
            yield return new WaitForSeconds(0.03f);
        }

    }
}
