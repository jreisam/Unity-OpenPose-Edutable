//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using UnityEngine;
using UnityEngine.UI;

public class Coloring3D : MonoBehaviour
{
    public CameraImageRenderer CameraRenderer;
    public Button ButtonChange;

    private Material material;
    private ImageTargetController imageTarget;
    private Text buttonText;
    private RenderTexture renderTexture;
    private Optional<bool> freezed;
    private RenderTexture freezedTexture;

    private void Awake()
    {
        buttonText = ButtonChange.transform.Find("Text").GetComponent<Text>();
        imageTarget = GetComponentInParent<ImageTargetController>();
        material = GetComponent<MeshRenderer>().material;
        CameraRenderer.RequestTargetTexture((camera, texture) => { renderTexture = texture; });
        imageTarget.TargetFound += () =>
        {
            if (freezed.OnNone)
            {
                buttonText.text = "Freeze";
                freezed = false;
            }
            ButtonChange.interactable = true;
        };
        imageTarget.TargetLost += () =>
        {
            ButtonChange.interactable = false;
        };
        ButtonChange.onClick.AddListener(() =>
        {
            if (freezed.Value)
            {
                freezed = false;
                buttonText.text = "Freeze";
                if (freezedTexture) { Destroy(freezedTexture); }
            }
            else
            {
                freezed = true;
                buttonText.text = "Thaw";
                if (freezedTexture) { Destroy(freezedTexture); }
                if (renderTexture)
                {
                    freezedTexture = new RenderTexture(renderTexture.width, renderTexture.height, 0);
                    Graphics.Blit(renderTexture, freezedTexture);
                }
                material.SetTexture("_MainTex", freezedTexture);
            }

        });
    }

    private void Update()
    {
        if (freezed.OnNone || freezed.Value || imageTarget.Target == null)
        {
            return;
        }
        var halfWidth = 0.5f;
        var halfHeight = 0.5f / imageTarget.Target.aspectRatio();
        Matrix4x4 points = Matrix4x4.identity;
        Vector3 targetAnglePoint1 = imageTarget.transform.TransformPoint(new Vector3(-halfWidth, halfHeight, 0));
        Vector3 targetAnglePoint2 = imageTarget.transform.TransformPoint(new Vector3(-halfWidth, -halfHeight, 0));
        Vector3 targetAnglePoint3 = imageTarget.transform.TransformPoint(new Vector3(halfWidth, halfHeight, 0));
        Vector3 targetAnglePoint4 = imageTarget.transform.TransformPoint(new Vector3(halfWidth, -halfHeight, 0));
        points.SetRow(0, new Vector4(targetAnglePoint1.x, targetAnglePoint1.y, targetAnglePoint1.z, 1f));
        points.SetRow(1, new Vector4(targetAnglePoint2.x, targetAnglePoint2.y, targetAnglePoint2.z, 1f));
        points.SetRow(2, new Vector4(targetAnglePoint3.x, targetAnglePoint3.y, targetAnglePoint3.z, 1f));
        points.SetRow(3, new Vector4(targetAnglePoint4.x, targetAnglePoint4.y, targetAnglePoint4.z, 1f));
        material.SetMatrix("_UvPints", points);
        material.SetMatrix("_RenderingViewMatrix", Camera.main.worldToCameraMatrix);
        material.SetMatrix("_RenderingProjectMatrix", GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false));
        material.SetTexture("_MainTex", renderTexture);
    }

    private void OnDestroy()
    {
        if (freezedTexture) { Destroy(freezedTexture); }
    }
}
