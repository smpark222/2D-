using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;


    public float m_force = 0f;
    public Vector3 m_offset = Vector3.zero;
    Quaternion m_originRot;
    public PostProcessVolume grayScale;

    // Start is called before the first frame update
    void Start()
    {
        m_originRot = this.transform.rotation;
        instance = this;
    }


    public void ShakeScreen()
    {
        StartCoroutine(Shake());
    }

    public void ColorToGray()
    {
        grayScale.isGlobal = true;
    }
    public void GrayToColor()
    {
        grayScale.isGlobal = false;
    }

    IEnumerator Shake()
    {
        for(int i = 0; i < 5;i++)
        {
            StopCoroutine(ShakeCoroutine());
            StopCoroutine(Reset());
            StartCoroutine(ShakeCoroutine());
            yield return new WaitForSeconds(0.01f);
            StopCoroutine(ShakeCoroutine());
            StopCoroutine(Reset());
            StartCoroutine(Reset());
            yield return new WaitForSeconds(0.01f);
        }

        StopCoroutine(ShakeCoroutine());
        StopCoroutine(Reset());

        transform.rotation = m_originRot;
        StopAllCoroutines();
        
    }



    IEnumerator ShakeCoroutine()
    {
        Vector3 t_originEuler = transform.eulerAngles;
        while(true)
        {
            float t_rotX = Random.Range(-m_offset.x, m_offset.x);
            float t_rotY = Random.Range(-m_offset.y, m_offset.y);
            float t_rotZ = Random.Range(-m_offset.z, m_offset.z);



            Vector3 t_randomRot = t_originEuler + new Vector3(t_rotX, t_rotY,t_rotZ);
            Quaternion t_rot = Quaternion.Euler(t_randomRot);

            while (Quaternion.Angle(transform.rotation, t_rot) > -0.1f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, t_rot, m_force * Time.fixedDeltaTime/4f);
                yield return null;
            }
            yield return null;
        }
    }

    IEnumerator Reset()
    {
        while(Quaternion.Angle(transform.rotation, m_originRot)>2f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, m_originRot, m_force * Time.fixedDeltaTime/25f);
            yield return null;
        }
        transform.rotation = m_originRot;

        StopCoroutine(Reset());
    }


}
