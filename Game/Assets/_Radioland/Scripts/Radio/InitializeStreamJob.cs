// http://answers.unity3d.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class InitializeStreamJob : ThreadedJob
{
    private static int maxRetryAttempts = 4;

    private Queue<string> urls;
    private Dictionary<string, int> streams;
    private Dictionary<string, bool> initializationStatus;

    public InitializeStreamJob() {
        urls = new Queue<string>();
        streams = new Dictionary<string, int>();
        initializationStatus = new Dictionary<string, bool>();
    }

    public void QueueURL(string url) {
        urls.Enqueue(url);

        initializationStatus.Add(url, false);
    }

    public bool InitializedForURL(string url) {
        return initializationStatus[url];
    }

    public int GetStreamForURL(string url) {
        return streams[url];
    }

    protected override void ThreadFunction() {
        for (int i = 0; i < maxRetryAttempts; i++) {
            InitializeStreams();

            if (urls.Count == 0) {
                Debug.Log("Finished initializing streams for all provided urls.");
                return;
            }

            int retrySeconds = Mathf.FloorToInt(Mathf.Pow(2, i));
            Debug.Log("Initialization attempt #" + (i + 1) + " failed, retrying in " +
                      retrySeconds + " seconds.");

            Thread.Sleep(retrySeconds * 1000); // Milliseconds
        }

        Debug.Log("Initialzation attempt #" + maxRetryAttempts + " failed, giving up.");
    }

    private void InitializeStreams() {
        Queue<string> urlsNext = new Queue<string>();

        while (urls.Count > 0) {
            string url = urls.Dequeue();

            Debug.Log("Initializing stream for " + url);

            int stream = InitializeStream(url);

            if (stream == 0) {
                urlsNext.Enqueue(url);
            } else {
                streams.Add(url, stream);
                initializationStatus[url] = true;
            }
        }

        urls = urlsNext;
    }

    protected override void OnFinished() {

    }

    private int InitializeStream(string url) {
        AudioStream.BASS_SetConfig(AudioStream.configs.BASS_CONFIG_NET_PLAYLIST, 2);
        int stream = AudioStream.BASS_StreamCreateURL(url, 0, AudioStream.flags.BASS_DEFAULT, IntPtr.Zero, IntPtr.Zero);

        if (stream == 0) {
            Debug.LogError("Unable to create stream, error code: " + AudioStream.BASS_ErrorGetCode());
        }

        return stream;
    }

}
