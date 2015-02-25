﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Reference: http://forum.unity3d.com/threads/web-radio-streaming-with-bass-dll.168046/

public class AudioStream : MonoBehaviour
{
    [SerializeField] private string url;

    public float[] spectrum;
    public float sampleRate;

    private int stream;
    private static bool bassInitialized = false; // Only initialize BASS once between all instances.
    private bool paused;
    [HideInInspector] public bool streamInitialized;

    private static int maxRetryAttempts = 5;

    public enum flags
    {
        BASS_DEFAULT
    }

    public enum configs
    {
        BASS_CONFIG_NET_PLAYLIST = 21
    }

    public enum attribs
    {
        BASS_ATTRIB_FREQ = 1,
        BASS_ATTRIB_VOL = 2
    }

    public enum lengths
    {
        BASS_DATA_FFT1024 = -2147483646,
        BASS_DATA_FFT2048 = -2147483645
    }

    [SerializeField] private float m_volume = 0f;
    public float volume {
        get { return m_volume; }
        set {
            m_volume = value;
            if (!streamInitialized) { return; }
            BASS_ChannelSetAttribute(stream, attribs.BASS_ATTRIB_VOL, m_volume);
        }
    }

    #region DLL - Stream Configuration and Initialization
    [DllImport("bass")]
    public static extern bool BASS_Init(int device, int freq, int flag, IntPtr hwnd, IntPtr clsid);

    [DllImport("bass")]
    public static extern int BASS_ErrorGetCode();

    [DllImport("bass")]
    public static extern bool BASS_SetConfig(configs config, int valuer);

    [DllImport("bass")]
    public static extern Int32 BASS_StreamCreateURL(string url, int offset, flags Flag, IntPtr dproc, IntPtr user);

    [DllImport("bass")]
    public static extern bool BASS_ChannelPlay(int stream, bool restart);

    [DllImport("bass")]
    public static extern bool BASS_StreamFree(int stream);

    [DllImport("bass")]
    public static extern bool BASS_Free();
    #endregion DLL - Stream Configuration and Initialization

    #region DLL - Stream Control and Analysis
    [DllImport("bass")]
    public static extern bool BASS_ChannelPause(int stream);

    [DllImport("bass")]
    public static extern bool BASS_SetVolume(float volume);

    [DllImport("bass")]
    public static extern bool BASS_ChannelGetAttribute(int handle, attribs attrib, out float value);

    [DllImport("bass")]
    public static extern bool BASS_ChannelSetAttribute(int handle, attribs attrib, float value);

    [DllImport("bass")]
    public static extern bool BASS_ChannelSlideAttribute(int handle, attribs attrib, float value, float time);

    [DllImport("bass")]
    public static extern long BASS_ChannelSeconds2Bytes(int handle, double pos);

    [DllImport("bass")]
    public static extern int BASS_ChannelGetData(int handle, float[] buffer, lengths length);
    #endregion DLL - Stream Control and Analysis

    private void Awake() {
        spectrum = new float[1024];
        paused = false;
        streamInitialized = false;

        if (!bassInitialized) {
            BASS_Free();

            bassInitialized = BASS_Init(-1, 44100, 0, IntPtr.Zero, IntPtr.Zero);
            if (!bassInitialized) {
                Debug.LogError("Unable to initialize BASS, error code: " + BASS_ErrorGetCode());
            }
        }

        StartCoroutine("RunInitialization");

        #if UNITY_EDITOR
        EditorApplication.playmodeStateChanged = HandleOnPlayModeChanged;
        #endif
    }

    private IEnumerator RunInitialization() {
        for (int i = 0; i < maxRetryAttempts; i++) {
            InitializeStream();

            if (streamInitialized) { yield break; }

            float retrySeconds = Mathf.Pow(2, i);
            Debug.Log("Initialization attempt #" + (i + 1) + " failed, retrying in " +
                      retrySeconds + " seconds.");
            yield return new WaitForSeconds(retrySeconds);
        }

        Debug.Log("Initialzation attempt #" + maxRetryAttempts + " failed, giving up.");
    }

    private void InitializeStream() {
        if (!bassInitialized || streamInitialized) { return; }

        BASS_SetConfig(configs.BASS_CONFIG_NET_PLAYLIST, 2);
        stream = BASS_StreamCreateURL(url, 0, flags.BASS_DEFAULT, IntPtr.Zero, IntPtr.Zero);

        if (stream != 0) {
            streamInitialized = true;
            volume = 0;
            BASS_ChannelPlay(stream, false);

            BASS_ChannelGetAttribute(stream, attribs.BASS_ATTRIB_FREQ, out sampleRate);
        } else {
            Debug.LogError("Unable to create stream, error code: " + BASS_ErrorGetCode());
        }
    }

    private void Start() {

    }

    private void Update() {
        if (!streamInitialized) { return; }

        if (Time.timeScale <= 0.001f && !paused) { Pause(); }
        if (Time.timeScale > 0.001f && paused) { Play(); }
        #if UNITY_EDITOR
            if (EditorApplication.isPaused || !EditorApplication.isPlaying) { Pause(); }
        #endif

        BASS_ChannelGetData(stream, spectrum, lengths.BASS_DATA_FFT2048);
    }

    #if UNITY_EDITOR
    private void HandleOnPlayModeChanged() {
        if (EditorApplication.isPaused || !EditorApplication.isPlaying) { Pause(); }
    }
    #endif

    private void Play() {
        if (!paused) { return; }
        paused = false;
        BASS_ChannelPlay(stream, false);
    }

    private void Pause() {
        if (paused) { return; }
        paused = true;
        BASS_ChannelPause(stream);
    }

    private void OnApplicationQuit() {
        BASS_StreamFree(stream);
        BASS_Free();
    }
}
