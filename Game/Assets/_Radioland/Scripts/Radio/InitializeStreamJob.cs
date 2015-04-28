// http://answers.unity3d.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class InitializeStreamJob : ThreadedJob
{
    public string in_url;
    public int out_stream;
    public bool successful;

    private static int maxRetryAttempts = 2;

    protected override void ThreadFunction() {
        Debug.Log("ThreadFunction");

        successful = false;

        for (int i = 0; i < maxRetryAttempts; i++) {
            bool initialized = InitializeStream();

            if (initialized) {
                successful = true;
                return;
            }

            int retrySeconds = Mathf.FloorToInt(Mathf.Pow(2, i));
            Debug.Log("Initialization attempt #" + (i + 1) + " failed, retrying in " +
                      retrySeconds + " seconds.");

            Thread.Sleep(retrySeconds * 1000); // Milliseconds
        }

        Debug.Log("Initialzation attempt #" + maxRetryAttempts + " failed, giving up.");

        return;
    }

    protected override void OnFinished() {

    }

    private bool InitializeStream() {
        AudioStream.BASS_SetConfig(AudioStream.configs.BASS_CONFIG_NET_PLAYLIST, 2);
        out_stream = AudioStream.BASS_StreamCreateURL(in_url, 0, AudioStream.flags.BASS_DEFAULT, IntPtr.Zero, IntPtr.Zero);

        if (out_stream != 0) {
            return true;
        } else {
            Debug.LogError("Unable to create stream, error code: " + AudioStream.BASS_ErrorGetCode());
            return false;
        }
    }

}
