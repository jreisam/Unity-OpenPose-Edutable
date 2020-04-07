//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SpatialMap_Dense_BallGame
{
    public class UIController : MonoBehaviour
    {
        public Text Status;
        public ARSession Session;
        public GameObject Ball;
        public int MaxBallCount = 30;
        public float BallLifetime = 15;

        private Color meshColor;
        private VIOCameraDeviceUnion vioCamera;
        private DenseSpatialMapBuilderFrameFilter dense;
        private List<GameObject> balls = new List<GameObject>();

        private void Awake()
        {
            vioCamera = Session.GetComponentInChildren<VIOCameraDeviceUnion>();
            dense = Session.GetComponentInChildren<DenseSpatialMapBuilderFrameFilter>();
        }

        private void Start()
        {
            meshColor = dense.MeshColor;
        }

        private void Update()
        {
            Status.text = "VIO Device Type: " + (vioCamera.Device == null ? "-" : vioCamera.Device.DeviceType.ToString()) + Environment.NewLine +
                "Tracking Status: " + (Session.WorldRootController == null ? "-" : Session.WorldRootController.TrackingStatus.ToString()) + Environment.NewLine +
                "Dense Mesh Block Count: " + dense.MeshBlocks.Count + Environment.NewLine +
                "Ball Count: " + balls.Count + "/" + MaxBallCount + Environment.NewLine +
                Environment.NewLine +
                "Gesture Instruction" + Environment.NewLine +
                "\tShoot Ball: Tap Screen";

            if (Input.GetMouseButtonDown(0) && Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                var launchPoint = Camera.main.transform;
                var ball = Instantiate(Ball, launchPoint.position, launchPoint.rotation);
                var rigid = ball.GetComponent<Rigidbody>();
                rigid.velocity = Vector3.zero;
                rigid.AddForce(ray.direction * 15f + Vector3.up * 5f);
                if (balls.Count > 0 && balls.Count == MaxBallCount)
                {
                    Destroy(balls[0]);
                    balls.RemoveAt(0);
                }
                balls.Add(ball);
                StartCoroutine(Kill(ball, BallLifetime));
            }
        }

        public void RenderMesh(bool show)
        {
            if (!dense)
            {
                return;
            }
            dense.RenderMesh = show;
        }


        public void TransparentMesh(bool trans)
        {
            if (!dense)
            {
                return;
            }
            dense.MeshColor = trans ? Color.clear : meshColor;
        }

        private IEnumerator Kill(GameObject ball, float lifetime)
        {
            yield return new WaitForSeconds(lifetime);
            if (balls.Remove(ball)) { Destroy(ball); }
        }
    }
}
