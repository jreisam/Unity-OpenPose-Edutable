//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ImageTracking_CloudRecognition
{
    public class CloudRecognizerSample : MonoBehaviour
    {
        public ARSession Session;
        public UnityEngine.UI.Text Status;
        public bool UseOfflineCache = true;
        public string OfflineCachePath;

        private CloudRecognizerFrameFilter cloudRecognizer;
        private ImageTrackerFrameFilter tracker;
        private List<GameObject> targetObjs = new List<GameObject>();
        private List<string> loadedCloudTargetUids = new List<string>();
        private Optional<CloudStatus> cloudStatus;
        private int cachedTargetCount;

        private void Awake()
        {
            tracker = Session.GetComponentInChildren<ImageTrackerFrameFilter>();
            cloudRecognizer = Session.GetComponentInChildren<CloudRecognizerFrameFilter>();

            if (UseOfflineCache)
            {
                if (string.IsNullOrEmpty(OfflineCachePath))
                {
                    OfflineCachePath = Application.persistentDataPath + "/CloudRecognizerSample";
                }
                if (!Directory.Exists(OfflineCachePath))
                {
                    Directory.CreateDirectory(OfflineCachePath);
                }
                if (Directory.Exists(OfflineCachePath))
                {
                    var targetFiles = Directory.GetFiles(OfflineCachePath);
                    foreach (var file in targetFiles)
                    {
                        if (Path.GetExtension(file) == ".etd")
                        {
                            LoadOfflineTarget(file);
                        }
                    }
                }
            }

            cloudRecognizer.CloudUpdate += (status, targets) =>
            {
                if (cloudStatus != status)
                {
                    Debug.LogFormat("Cloud status changed: {0} with {1} targets", status, targets.Count);
                    cloudStatus = status;
                }

                foreach (var target in targets)
                {
                    var uid = target.uid();
                    if (loadedCloudTargetUids.Contains(uid))
                    {
                        continue;
                    }

                    LoadCloudTarget(target.Clone() as ImageTarget);
                }
            };
        }

        private void Update()
        {
            Status.text = "Cloud Recognizing: " + (cloudRecognizer && cloudRecognizer.enabled ? "On" : "Off") + Environment.NewLine +
                "CloudStatus: " + (cloudRecognizer && cloudRecognizer.enabled ? cloudStatus.ToString() : "-") + Environment.NewLine +
                "CachedTargets: " + cachedTargetCount + Environment.NewLine +
                "LoadedTargets: " + loadedCloudTargetUids.Count;
        }

        private void OnDestroy()
        {
            foreach (var obj in targetObjs)
            {
                Destroy(obj);
            }
        }

        public void ClearAll()
        {
            if (Directory.Exists(OfflineCachePath))
            {
                var targetFiles = Directory.GetFiles(OfflineCachePath);
                foreach (var file in targetFiles)
                {
                    if (Path.GetExtension(file) == ".etd")
                    {
                        File.Delete(file);
                    }
                }
            }
            foreach (var obj in targetObjs)
            {
                Destroy(obj);
            }
            targetObjs.Clear();
            loadedCloudTargetUids.Clear();
            cachedTargetCount = 0;
        }

        private void LoadCloudTarget(ImageTarget target)
        {
            var uid = target.uid();
            loadedCloudTargetUids.Add(uid);
            var go = new GameObject(uid);
            targetObjs.Add(go);
            var targetController = go.AddComponent<ImageTargetController>();
            targetController.SourceType = ImageTargetController.DataSource.Target;
            targetController.TargetSource = target;
            LoadTargetIntoTracker(targetController);

            targetController.TargetLoad += (loadedTarget, result) =>
            {
                if (!result)
                {
                    Debug.LogErrorFormat("target {0} load failed", uid);
                    return;
                }
                AddCubeOnTarget(targetController);
            };

            if (UseOfflineCache && Directory.Exists(OfflineCachePath))
            {
                if (target.save(OfflineCachePath + "/" + target.uid() + ".etd"))
                {
                    cachedTargetCount++;
                }
            }
        }

        private void LoadOfflineTarget(string file)
        {
            var go = new GameObject(Path.GetFileNameWithoutExtension(file) + "-offline");
            targetObjs.Add(go);
            var targetController = go.AddComponent<ImageTargetController>();
            targetController.SourceType = ImageTargetController.DataSource.TargetDataFile;
            targetController.TargetDataFileSource.PathType = PathType.Absolute;
            targetController.TargetDataFileSource.Path = file;
            LoadTargetIntoTracker(targetController);

            targetController.TargetLoad += (loadedTarget, result) =>
            {
                if (!result)
                {
                    Debug.LogErrorFormat("target data {0} load failed", file);
                    return;
                }
                loadedCloudTargetUids.Add(loadedTarget.uid());
                cachedTargetCount++;
                AddCubeOnTarget(targetController);
            };
        }

        private void LoadTargetIntoTracker(ImageTargetController controller)
        {
            controller.Tracker = tracker;
            controller.TargetFound += () =>
            {
                cloudRecognizer.enabled = false;
            };
            controller.TargetLost += () =>
            {
                cloudRecognizer.enabled = true;
            };
        }

        private void AddCubeOnTarget(ImageTargetController controller)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = Resources.Load("Materials/EasyAR") as Material;
            cube.transform.parent = controller.transform;
            cube.transform.localPosition = new Vector3(0, 0, -0.1f);
            cube.transform.eulerAngles = new Vector3(0, 0, 180);
            cube.transform.localScale = new Vector3(0.5f, 0.5f / controller.Target.aspectRatio(), 0.2f);
        }
    }
}
