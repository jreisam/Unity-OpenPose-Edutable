//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using Common;
using easyar;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MotionTracking
{
    public class UIController : MonoBehaviour
    {
        public Text Status;
        public ARSession Session;
        public TouchController TouchControl;

        private VIOCameraDeviceUnion vioCamera;

        private void Awake()
        {
            vioCamera = Session.GetComponentInChildren<VIOCameraDeviceUnion>();
            TouchControl.TurnOn(TouchControl.gameObject.transform, Camera.main, true, true, true, true);
        }

        private void Update()
        {
            Status.text = "VIO Device Type: " + (vioCamera.Device == null ? "-" : vioCamera.Device.DeviceType.ToString()) + Environment.NewLine +
                "Tracking Status: " + (Session.WorldRootController == null ? "-" : Session.WorldRootController.TrackingStatus.ToString()) + Environment.NewLine +
                "CenterMode: " + Session.CenterMode + Environment.NewLine +
                Environment.NewLine +
                "Gesture Instruction" + Environment.NewLine +
                "\tMove in View: One Finger Move" + Environment.NewLine +
                "\tMove Near/Far: Two Finger Vertical Move" + Environment.NewLine +
                "\tRotate: Two Finger Horizontal Move" + Environment.NewLine +
                "\tScale: Two Finger Pinch";
        }

        public void SwitchCenterMode()
        {
            while (true)
            {
                Session.CenterMode = (ARSession.ARCenterMode)(((int)Session.CenterMode + 1) % Enum.GetValues(typeof(ARSession.ARCenterMode)).Length);
                if (Session.CenterMode == ARSession.ARCenterMode.Camera ||
                    Session.CenterMode == ARSession.ARCenterMode.WorldRoot)
                {
                    break;
                }
            }
        }
    }
}
