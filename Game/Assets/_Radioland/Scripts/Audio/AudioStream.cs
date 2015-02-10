using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// Reference: http://forum.unity3d.com/threads/web-radio-streaming-with-bass-dll.168046/

public class AudioStream : MonoBehaviour
{
    [SerializeField] private string url;

    private int stream;

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
        BASS_ATTRIB_VOL = 2
    }

    [SerializeField] private float m_volume = 0f;
    public float volume {
        get { return m_volume; }
        set {
            m_volume = value;
            BASS_ChannelSetAttribute(stream, attribs.BASS_ATTRIB_VOL, m_volume);
        }
    }

    #region DLL - Stream Configuration and Initialization
    [DllImport("bass")]
    public static extern bool BASS_Init(int device, int freq, int flag, IntPtr hwnd, IntPtr clsid);

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

    #region DLL - Stream Control
    [DllImport("bass")]
    public static extern bool BASS_SetVolume(float volume);

    [DllImport("bass")]
    public static extern bool BASS_ChannelSetAttribute(int stream, attribs attrib, float value);

    [DllImport("bass")]
    public static extern bool BASS_ChannelSlideAttribute(int stream, attribs attrib, float value, float time);
    #endregion DLL - Stream Control

    private void Awake() {
        if (BASS_Init(-1, 44100, 0, IntPtr.Zero, IntPtr.Zero)) {
            BASS_SetConfig(configs.BASS_CONFIG_NET_PLAYLIST, 2);
            stream = BASS_StreamCreateURL(url, 0, flags.BASS_DEFAULT, IntPtr.Zero, IntPtr.Zero);

            if (stream != 0) {
                volume = 0;
                BASS_ChannelPlay(stream, false);
            }
        }
    }

    private void OnApplicationQuit() {
        BASS_StreamFree(stream);
        BASS_Free();
    }
}
