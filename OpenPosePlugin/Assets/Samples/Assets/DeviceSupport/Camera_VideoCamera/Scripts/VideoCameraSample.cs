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

namespace Camera_VideoCamera
{
    public class VideoCameraSample : MonoBehaviour
    {
        public ARSession arSession;
        public MeshRenderer CubeRenderer;
        public Text CameraStatus;
        public Toggle FlipSwitch;

        private VideoCameraDevice videoCamera;
        private CameraImageRenderer cameraRenderer;
        private Texture cubeTexture;
        private Action<Camera, RenderTexture> targetTextureEventHandler;

        private void Awake()
        {
            videoCamera = arSession.GetComponentInChildren<VideoCameraDevice>();
            cameraRenderer = arSession.GetComponentInChildren<CameraImageRenderer>();
            cubeTexture = CubeRenderer.material.mainTexture;
            targetTextureEventHandler = (camera, texture) =>
            {
                if (texture)
                {
                    CubeRenderer.material.mainTexture = texture;
                }
                else
                {
                    CubeRenderer.material.mainTexture = cubeTexture;
                    if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Metal)
                    {
                        CubeRenderer.transform.localScale = new Vector3(-1, -1, 1);
                    }
                    else
                    {
                        CubeRenderer.transform.localScale = new Vector3(1, 1, 1);
                    }
                }
            };

            videoCamera.DeviceOpened += () =>
            {
                if (videoCamera.Device == null)
                {
                    return;
                }
                var flip = videoCamera.Device.type() == CameraDeviceType.Front ? arSession.HorizontalFlipFront : arSession.HorizontalFlipNormal;
                FlipSwitch.isOn = flip == ARSession.ARHorizontalFlipMode.World;
            };
        }

        private void Update()
        {
            if (videoCamera.Device == null)
            {
                CameraStatus.text = "Camera: Unavailable";
                return;
            }

            using (var cameraParameters = videoCamera.Device.cameraParameters())
            {
                CameraStatus.text = "Camera: " + (videoCamera.enabled ? "On" : "Off") + Environment.NewLine +
                    "Camera Index: " + videoCamera.Device.index() + Environment.NewLine +
                    "Camera Count: " + CameraDevice.cameraCount() + Environment.NewLine +
                    "Camera Type: " + cameraParameters.cameraDeviceType() + Environment.NewLine +
                    "HorizontalFlip: " + (cameraParameters.cameraDeviceType() == CameraDeviceType.Front ? arSession.HorizontalFlipFront : arSession.HorizontalFlipNormal);
            }

            if (CubeRenderer.material.mainTexture != cubeTexture)
            {
                if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Metal)
                {
                    CubeRenderer.transform.localScale = new Vector3(-1, -(float)Screen.height / Screen.width, 1);
                }
                else
                {
                    CubeRenderer.transform.localScale = new Vector3(1, (float)Screen.height / Screen.width, 1);
                }
            }
        }

        public void NextCamera()
        {
            if (!videoCamera || videoCamera.Device == null)
            {
                return;
            }
            if (CameraDevice.cameraCount() == 0)
            {
                GUIPopup.EnqueueMessage("Camera unavailable", 3);
                videoCamera.Close();
                return;
            }

            var index = videoCamera.Device.index();
            index = (index + 1) % CameraDevice.cameraCount();
            videoCamera.CameraOpenMethod = VideoCameraDevice.CameraDeviceOpenMethod.DeviceIndex;
            videoCamera.CameraIndex = index;
            GUIPopup.EnqueueMessage("Switch to camera index: " + index, 3);

            videoCamera.Close();
            videoCamera.Open();
        }

        public void Capture(bool on)
        {
            if (!videoCamera || videoCamera.Device == null)
            {
                return;
            }

            if (on)
            {
                cameraRenderer.RequestTargetTexture(targetTextureEventHandler);
            }
            else
            {
                cameraRenderer.DropTargetTexture(targetTextureEventHandler);
            }
            return;
        }

        public void EnableCamera(bool enable)
        {
            videoCamera.enabled = enable;
        }

        public void ShowCameraImage(bool show)
        {
            cameraRenderer.enabled = show;
        }

        public void HFlip(bool flip)
        {
            if (!videoCamera || videoCamera.Device == null)
            {
                return;
            }
            bool isFront = false;
            using (var cameraParameters = videoCamera.Device.cameraParameters())
            {
                if(cameraParameters.cameraDeviceType() == CameraDeviceType.Front)
                {
                    isFront = true;
                }
            }

            if (isFront)
            {
                arSession.HorizontalFlipFront = flip ? ARSession.ARHorizontalFlipMode.World : ARSession.ARHorizontalFlipMode.None;
            }
            else
            {
                arSession.HorizontalFlipNormal = flip ? ARSession.ARHorizontalFlipMode.World : ARSession.ARHorizontalFlipMode.None;
            }
        }
    }
}
