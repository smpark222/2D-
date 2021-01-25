using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public Image[] images;
    Coroutine routine = null;

    public static Fade instance;

    PlayerController player;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        images = GetComponentsInChildren<Image>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }


    public void SquareFadeOut()
    {
        if(routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
        routine = StartCoroutine(CoSquareFadeOut());
    }

    public void SquareFadeIn()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
        routine = StartCoroutine(CoSquareFadeIn());
    }
    public void AlphaFadeOut()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
        routine = StartCoroutine(CoAlphaFadeOut());
    }
    public void AlphaFadeIn()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
        routine = StartCoroutine(CoAlphaFadeIn());
    }


    IEnumerator CoSquareFadeIn()
    {
        Color[] colors = new Color[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
                images[i].fillAmount = 0;
            
            colors[i] = images[i].color;
            colors[i].a = 1f;
            images[i].color = colors[i];

        }

        List<Image> imageList = new List<Image>();
        for(int i = 0; i<images.Length;i++)
        {
            imageList.Insert(i, images[i]);
            imageList[i].fillMethod = (Image.FillMethod)Random.Range(0, 2);
        }
        while(imageList.Count != 0)
        {
            int randomNum = Random.Range(0, imageList.Count);
            StartCoroutine(ImageFade(imageList[randomNum], 1f));
            imageList.RemoveAt(randomNum);
            yield return new WaitForSeconds(0.01f);

        }
        player.fade = false;
    }



    IEnumerator CoSquareFadeOut()
    {
        Color[] colors = new Color[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            colors[i] = images[i].color;
            colors[i].a = 1f;
            images[i].color = colors[i];
        }
        List<Image> imageList = new List<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            imageList.Insert(i, images[i]);
            imageList[i].fillMethod = (Image.FillMethod)Random.Range(0, 2);
            imageList[i].fillOrigin = Random.Range(0, 2);
        }
        while (imageList.Count != 0)
        {
            int randomNum = Random.Range(0, imageList.Count);
            StartCoroutine(ImageFade(imageList[randomNum],0));
            imageList.RemoveAt(randomNum);
            yield return null;
        }
        player.fade = false;
    }

    IEnumerator ImageFade(Image image, float target)
    {
        while(Mathf.Abs(image.fillAmount - target) > 0.01f)
        {
            image.fillAmount = Mathf.Lerp(image.fillAmount, target, Time.deltaTime * 6f);
            yield return Yields.FixedUpdate; 
        }
        image.fillAmount = target;
    }


    IEnumerator CoAlphaFadeIn()
    {
        Color[] colors = new Color[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            colors[i] = images[i].color;

            colors[i].a = 0f;
            images[i].color = colors[i];
            images[i].fillAmount = 1f;

        }
        while (colors[0].a<1f)
        {
            for(int i = 0; i< images.Length;i++)
            {
                colors[i].a = Mathf.Lerp(colors[i].a, 1f, Time.deltaTime * 3f);

                if (colors[i].a >= 1f) colors[i].a = 1f;
                images[i].color = colors[i];

            }
            yield return null;

        }
        player.fade = false;
    }
    IEnumerator CoAlphaFadeOut()
    {
        Color[] colors = new Color[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            colors[i] = images[i].color;
            images[i].fillAmount = 1f;

        }
        while (colors[0].a > 0f)
        {
            for (int i = 0; i < images.Length; i++)
            {
                colors[i].a = Mathf.Lerp(colors[i].a, 0f, Time.deltaTime * 3f);

                if (colors[i].a <= 0f) colors[i].a = 0f;
                images[i].color = colors[i];

            }
            yield return null;

        }
        player.fade = false;
    }
}
