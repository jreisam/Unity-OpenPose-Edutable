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
using UnityEngine;
using UnityEngine.UI;

namespace ImageTracking_ImageTarget
{
    public class ImageTrackingSample : MonoBehaviour
    {
        public ARSession Session;
        public Text Status;

        private Dictionary<ImageTargetController, bool> imageTargetControllers = new Dictionary<ImageTargetController, bool>();
        private ImageTargetController controllerNamecard;
        private ImageTargetController controllerIdback;
        private ImageTrackerFrameFilter imageTracker;
        private VideoCameraDevice cameraDevice;

        private void Awake()
        {
            imageTracker = Session.GetComponentInChildren<ImageTrackerFrameFilter>();
            cameraDevice = Session.GetComponentInChildren<VideoCameraDevice>();

            // targets from scene
            controllerNamecard = GameObject.Find("ImageTarget-namecard").GetComponent<ImageTargetController>();
            controllerIdback = GameObject.Find("ImageTarget-idback").GetComponent<ImageTargetController>();
            imageTargetControllers[controllerNamecard] = false;
            imageTargetControllers[controllerIdback] = false;
            AddTargetControllerEvents(controllerNamecard);
            AddTargetControllerEvents(controllerIdback);

            CreateTargets();
        }

        private void Update()
        {
            bool isFront = false;
            if(cameraDevice.Device != null)
            {
                using (var cameraParameters = cameraDevice.Device.cameraParameters())
                {
                    if (cameraParameters.cameraDeviceType() == CameraDeviceType.Front)
                    {
                        isFront = true;
                    }
                }
            }

            var statusText = "CenterMode: " + Session.CenterMode + Environment.NewLine +
                "CenterTarget: " + (Session.CenterTarget ? Session.CenterTarget.name : null) + Environment.NewLine +
                "HorizontalFlip: " + (isFront ? Session.HorizontalFlipFront : Session.HorizontalFlipNormal) + Environment.NewLine +
                "Camera: " + (cameraDevice && cameraDevice.enabled ? "On" : "Off") + Environment.NewLine +
                "Tracking: " + (imageTracker && imageTracker.enabled ? "On" : "Off") + Environment.NewLine + Environment.NewLine +
                "Target Load Status:" + Environment.NewLine;

            foreach (var item in imageTargetControllers)
            {
                statusText += "\t" + item.Key.gameObject.name + ": " + item.Value + Environment.NewLine;
            }

            Status.text = statusText;
        }

        public void Tracking(bool on)
        {
            imageTracker.enabled = on;
        }

        public void UnloadTargets()
        {
            foreach (var item in imageTargetControllers)
            {
                item.Key.Tracker = null;
            }
        }

        public void LoadTargets()
        {
            foreach (var item in imageTargetControllers)
            {
                item.Key.Tracker = imageTracker;
            }
        }

        public void SwitchCenterMode()
        {
            while (true)
            {
                Session.CenterMode = (ARSession.ARCenterMode)(((int)Session.CenterMode + 1) % Enum.GetValues(typeof(ARSession.ARCenterMode)).Length);
                if (Session.CenterMode == ARSession.ARCenterMode.SpecificTarget)
                {
                    Session.CenterTarget = controllerNamecard;
                }
                if (Session.CenterMode == ARSession.ARCenterMode.FirstTarget ||
                    Session.CenterMode == ARSession.ARCenterMode.Camera ||
                    Session.CenterMode == ARSession.ARCenterMode.SpecificTarget)
                {
                    break;
                }
            }
        }

        public void EnableCamera(bool enable)
        {
            cameraDevice.enabled = enable;
        }

        public void SwitchHFlipMode()
        {
            if (cameraDevice.Device == null)
            {
                return;
            }
            using (var cameraParameters = cameraDevice.Device.cameraParameters())
            {
                if (cameraParameters.cameraDeviceType() == CameraDeviceType.Front)
                {
                    Session.HorizontalFlipFront = (ARSession.ARHorizontalFlipMode)(((int)Session.HorizontalFlipFront + 1) % Enum.GetValues(typeof(ARSession.ARHorizontalFlipMode)).Length);
                }
                else
                {
                    Session.HorizontalFlipNormal = (ARSession.ARHorizontalFlipMode)(((int)Session.HorizontalFlipNormal + 1) % Enum.GetValues(typeof(ARSession.ARHorizontalFlipMode)).Length);
                }
            }
        }

        public void NextCamera()
        {
            if (!cameraDevice || cameraDevice.Device == null)
            {
                return;
            }
            if (CameraDevice.cameraCount() == 0)
            {
                GUIPopup.EnqueueMessage("Camera unavailable", 3);
                cameraDevice.Close();
                return;
            }

            var index = cameraDevice.Device.index();
            index = (index + 1) % CameraDevice.cameraCount();
            cameraDevice.CameraOpenMethod = VideoCameraDevice.CameraDeviceOpenMethod.DeviceIndex;
            cameraDevice.CameraIndex = index;
            GUIPopup.EnqueueMessage("Switch to camera index: " + index, 3);

            cameraDevice.Close();
            cameraDevice.Open();
        }

        private void CreateTargets()
        {
            // dynamically load from image (*.jpg, *.png)
            var targetController = CreateTargetNode("ImageTarget-argame00");
            targetController.Tracker = imageTracker;
            targetController.SourceType = ImageTargetController.DataSource.ImageFile;
            targetController.ImageFileSource.PathType = PathType.StreamingAssets;
            targetController.ImageFileSource.Path = "sightplus/argame00.jpg";
            targetController.ImageFileSource.Name = "argame00";
            targetController.ImageFileSource.Scale = 0.1f;

            GameObject duck02 = Instantiate(Resources.Load("duck02")) as GameObject;
            duck02.transform.parent = targetController.gameObject.transform;

            // dynamically load from json string
            var imageJosn = JsonUtility.FromJson<ImageJson>(@"
            {
                ""images"" :
                [
                    {
                        ""image"" : ""sightplus/argame01.png"",
                        ""name"" : ""argame01""
                    },
                    {
                        ""image"" : ""sightplus/argame02.jpg"",
                        ""name"" : ""argame02"",
                        ""scale"" : 0.2
                    },
                    {
                        ""image"" : ""sightplus/argame03.jpg"",
                        ""name"" : ""argame03"",
                        ""scale"" : 1,
                        ""uid"" : ""uid string will be ignored""
                    }
                ]
            }");

            foreach (var image in imageJosn.images)
            {
                targetController = CreateTargetNode("ImageTarget-" + image.name);
                targetController.Tracker = imageTracker;
                targetController.ImageFileSource.PathType = PathType.StreamingAssets;
                targetController.ImageFileSource.Path = image.image;
                targetController.ImageFileSource.Name = image.name;
                targetController.ImageFileSource.Scale = image.scale;

                var duck03 = Instantiate(Resources.Load("duck03")) as GameObject;
                duck03.transform.parent = targetController.gameObject.transform;
            }
        }

        private ImageTargetController CreateTargetNode(string targetName)
        {
            GameObject go = new GameObject(targetName);
            var targetController = go.AddComponent<ImageTargetController>();
            AddTargetControllerEvents(targetController);
            imageTargetControllers[targetController] = false;
            return targetController;
        }

        private void AddTargetControllerEvents(ImageTargetController controller)
        {
            if (!controller)
            {
                return;
            }

            controller.TargetFound += () =>
            {
                Debug.LogFormat("Found target {{id = {0}, name = {1}}}", controller.Target.runtimeID(), controller.Target.name());
            };
            controller.TargetLost += () =>
            {
                Debug.LogFormat("Lost target {{id = {0}, name = {1}}}", controller.Target.runtimeID(), controller.Target.name());
            };
            controller.TargetLoad += (Target target, bool status) =>
            {
                imageTargetControllers[controller] = status ? true : imageTargetControllers[controller];
                Debug.LogFormat("Load target {{id = {0}, name = {1}, size = {2}}} into {3} => {4}", target.runtimeID(), target.name(), controller.Size, controller.Tracker.name, status);
            };
            controller.TargetUnload += (Target target, bool status) =>
            {
                imageTargetControllers[controller] = status ? false : imageTargetControllers[controller];
                Debug.LogFormat("Unload target {{id = {0}, name = {1}}} => {2}", target.runtimeID(), target.name(), status);
            };
        }

        [Serializable]
        public class ImageJson
        {
            public ImageFile[] images;
        }

        [Serializable]
        public class ImageFile
        {
            public string image;
            public string name;
            public float scale = 0.1f;
        }
    }
}
