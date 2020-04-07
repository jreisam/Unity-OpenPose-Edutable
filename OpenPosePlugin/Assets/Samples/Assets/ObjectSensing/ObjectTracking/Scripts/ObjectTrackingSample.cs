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
using UnityEngine.UI;

namespace ObjectTracking
{
    public class ObjectTrackingSample : MonoBehaviour
    {
        public ARSession Session;
        public ObjectTargetController objectTarget;
        public Text Status;

        private VideoCameraDevice cameraDevice;
        private ObjectTrackerFrameFilter objectTracker;
        private bool loadstatus;

        private void Awake()
        {
            objectTracker = Session.GetComponentInChildren<ObjectTrackerFrameFilter>();
            cameraDevice = Session.GetComponentInChildren<VideoCameraDevice>();

            objectTarget = GameObject.Find("ObjectTarget").GetComponent<ObjectTargetController>();
            AddTargetControllerEvents(objectTarget);
        }

        private void Update()
        {
            bool isFront = false;
            if (cameraDevice.Device != null)
            {
                using (var cameraParameters = cameraDevice.Device.cameraParameters())
                {
                    if (cameraParameters.cameraDeviceType() == CameraDeviceType.Front)
                    {
                        isFront = true;
                    }
                }
            }
            Status.text = "CenterMode: " + Session.CenterMode + Environment.NewLine +
                "CenterTarget: " + (Session.CenterTarget ? Session.CenterTarget.name : null) + Environment.NewLine +
                "HorizontalFlip: " + (isFront ? Session.HorizontalFlipFront : Session.HorizontalFlipNormal) + Environment.NewLine +
                "Camera: " + (cameraDevice && cameraDevice.enabled ? "On" : "Off") + Environment.NewLine +
                "Tracking: " + (objectTracker && objectTracker.enabled ? "On" : "Off") + Environment.NewLine + Environment.NewLine +
                "Target Load Status:" + loadstatus + Environment.NewLine;
        }

        public void Tracking(bool on)
        {
            objectTracker.enabled = on;
        }

        public void UnloadTargets()
        {
            objectTarget.Tracker = null;
        }

        public void LoadTargets()
        {
            objectTarget.Tracker = objectTracker;
        }

        public void SwitchCenterMode()
        {
            while (true)
            {
                Session.CenterMode = (ARSession.ARCenterMode)(((int)Session.CenterMode + 1) % Enum.GetValues(typeof(ARSession.ARCenterMode)).Length);
                if (Session.CenterMode == ARSession.ARCenterMode.SpecificTarget)
                {
                    Session.CenterTarget = objectTarget;
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

        private void AddTargetControllerEvents(ObjectTargetController controller)
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
                loadstatus = status ? true : loadstatus;
                Debug.LogFormat("Load target {{id = {0}, name = {1}}} into {2} => {3}", target.runtimeID(), target.name(), controller.Tracker.name, status);
            };
            controller.TargetUnload += (Target target, bool status) =>
            {
                loadstatus = status ? false : loadstatus;
                Debug.LogFormat("Unload target {{id = {0}, name = {1}}} => {2}", target.runtimeID(), target.name(), status);
            };
        }
    }
}
