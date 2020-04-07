//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using System;
using UnityEngine;

namespace VideoRecording
{
    public class RecordingSample : MonoBehaviour
    {
        public bool RecordWatermark;
        public Material WatermarkMaterial;
        public GameObject Cube;

        private VideoRecorder videoRecorder;
        private string filePath;
        private CameraRecorder cameraRecorder;
        private GameObject cube;

        private void Awake()
        {
            cube = Instantiate(Cube);
            videoRecorder = FindObjectOfType<VideoRecorder>();
            videoRecorder.FilePathType = VideoRecorder.OutputPathType.PersistentDataPath;
            videoRecorder.StatusUpdate += (status, msg) =>
            {
                if (status == RecordStatus.OnStarted)
                {
                    GUIPopup.EnqueueMessage("Recording start", 5);
                }
                if (status == RecordStatus.FailedToStart || status == RecordStatus.FileFailed || status == RecordStatus.LogError)
                {
                    GUIPopup.EnqueueMessage("Recording Error: " + status + ", details: " + msg, 5);
                }
                Debug.Log("RecordStatus: " + status + ", details: " + msg);
            };
        }

        private void OnDestroy()
        {
            if (cube)
            {
                Destroy(cube);
            }
        }

        public void SampleStart()
        {
            if (!videoRecorder)
            {
                return;
            }
            if (!videoRecorder.IsReady)
            {
                return;
            }

            if (cube)
            {
                Destroy(cube);
            }
            cube = Instantiate(Cube);
            filePath = "EasyAR_Recording_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".mp4";
            videoRecorder.FilePath = filePath;
            videoRecorder.StartRecording();
            cameraRecorder = Camera.main.gameObject.AddComponent<CameraRecorder>();
            cameraRecorder.Setup(videoRecorder, WatermarkMaterial);
            if (RecordWatermark)
            {
                cameraRecorder.Setup(videoRecorder, WatermarkMaterial);
            }
            else
            {
                cameraRecorder.Setup(videoRecorder, null);
            }
        }

        public void SampleStop()
        {
            if (!videoRecorder)
            {
                return;
            }
            if (videoRecorder.StopRecording())
            {
                GUIPopup.EnqueueMessage("Recording finished, video saved to Unity Application.persistentDataPath" + Environment.NewLine +
                    "Filename: " + filePath + Environment.NewLine +
                    "PersistentDataPath: " + Application.persistentDataPath + Environment.NewLine +
                    "You can change sample code if you prefer to record videos into system album", 8);
                AdjustVideoAndPlay();
            }
            else
            {
                GUIPopup.EnqueueMessage("Recording failed", 5);
            }
            if (cameraRecorder)
            {
                cameraRecorder.Destroy();
            }
        }

        private void AdjustVideoAndPlay()
        {
            if (!cube)
            {
                return;
            }
            var player = cube.GetComponent<UnityEngine.Video.VideoPlayer>();
            player.transform.localScale = new Vector3(1, (float)Screen.height / Screen.width, 1);

            if (player.isPlaying)
            {
                player.Stop();
            }
            player.url = FileUtil.PathToUrl(Application.persistentDataPath + "/" + filePath);
            player.Play();

            // Unity bug: https://issuetracker.unity3d.com/issues/android-video-player-cannot-play-files-located-in-the-persistent-data-directory-on-android-10
            // more info in this thread: https://forum.unity.com/threads/error-videoplayer-on-android.742451/
            // We are not going to fix or workaround this bug here, the video playback is just a visual feedback for recording result but not a recording feature. You can still find your recorded videos in the directory.
#if UNITY_ANDROID && !UNITY_EDITOR
            int sdkVersion = 0;
            using (var buildVersion = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                sdkVersion = buildVersion.GetStatic<int>("SDK_INT");
            }
            if (sdkVersion >= 29)
            {
                GUIPopup.EnqueueMessage("Video recorded may not play on cube due to Unity VideoPlayer bug on Android Q", 5);
            }
#endif
        }
    }
}
