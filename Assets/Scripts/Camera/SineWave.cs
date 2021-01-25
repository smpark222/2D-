/*
Creates and Manages the SineWave
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class SineWave : MonoBehaviour
{

    #region Variables

    //this is the material that will store the SineWave Shader
    public Shader SineWaveShader;
    private Material mat;
    public static SineWave instance;


    private bool _XAxis;
    public bool XAxis
    {
        get { return _XAxis; }
        set
        {
            _XAxis = value;
            float X = (XAxis) ? 1f : 0f;
            mat.SetFloat("_XAxis", X);
        }
    }


    private float _HorizontalOffset;
    public float HorizontalOffset
    {
        get { return _HorizontalOffset; }
        set
        {
            _HorizontalOffset = value;
            mat.SetFloat("_HorizontalOffset", HorizontalOffset);
        }
    }


    private float _VerticalOffset;
    public float VerticalOffset
    {
        get { return _VerticalOffset; }
        set
        {
            _VerticalOffset = value;
            mat.SetFloat("_VerticalOffset", VerticalOffset);
        }
    }


    private float _Amplitude;
    public float Amplitude
    {
        get { return _Amplitude; }
        set
        {
            _Amplitude = value;
            mat.SetFloat("_Amplitude", _Amplitude);
        }
    }


    private float _Frequency;
    public float Frequency
    {
        get { return _Frequency; }
        set
        {
            _Frequency = value;
            mat.SetFloat("_Frequency", _Frequency);
        }
    }

    #endregion

    #region Methods
    void Start()
    {
        mat = new Material(SineWaveShader);
        instance = this;
        //default values
        XAxis = false;
        HorizontalOffset = 0f;
        VerticalOffset = 0;
        Amplitude = 0f;
        Frequency = 60f;


    }



    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (mat == null)
        {
            return;
        }

        if (mat != null)
        {

        }

        Graphics.Blit(src, dest, mat);
    }

    public void ScreenWave()
    {
        float a = 0.05f;
        VerticalOffset = VerticalOffset + a;

        Amplitude = Mathf.Lerp(Amplitude, 0.05f, Time.deltaTime * 0.1f);

        if (VerticalOffset >= 0.01f || VerticalOffset <= -0.01f)
        {
            a = -a;
        }

    }

    public void StopWave()
    {
        VerticalOffset = 0f;
        Amplitude = 0f;
    }

    #endregion
}
