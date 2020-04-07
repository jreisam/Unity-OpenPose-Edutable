//=============================================================================================================================
//
// Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
// EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
// and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//=============================================================================================================================

using System.Collections;
using UnityEngine;

namespace ImageTracking_TargetOnTheFly
{
    public class TargetOnTheFly : MonoBehaviour
    {
        [HideInInspector]
        public bool StartShowMessage;
        public GUISkin Skin;

        private bool isShowing;
        private ImageTargetManager imageManager;
        private FilesManager imageCreator;

        private void Start()
        {
            imageManager = FindObjectOfType<ImageTargetManager>();
            imageCreator = FindObjectOfType<FilesManager>();
            Skin = Instantiate(Skin);
        }

        private void OnGUI()
        {
            if (StartShowMessage)
            {
                if (!isShowing)
                    StartCoroutine(ShowMessageAndLoadTarget());
                StartShowMessage = false;
            }
            GUI.Box(new Rect(Screen.width / 2 - 250, 30, 500, 60), "The box area will be used as ImageTarget. Take photo!", Skin.GetStyle("Box"));
            GUI.Box(new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), "", Skin.GetStyle("Button"));

            if (isShowing)
                GUI.Box(new Rect(Screen.width / 2 - 65, Screen.height / 2, 130, 60), "Photo Saved", Skin.GetStyle("Box"));


            if (GUI.Button(new Rect(Screen.width / 2 - 80, Screen.height - 85, 160, 80), "Take Photo", Skin.GetStyle("Button")))
                imageCreator.StartTakePhoto();

            if (GUI.Button(new Rect(30, Screen.height - 85, 150, 80), "Clear Targets", Skin.GetStyle("Button")))
            {
                imageCreator.ClearAllImageFiles();
                imageManager.ClearAllTarget();
            }
        }

        private IEnumerator ShowMessageAndLoadTarget()
        {
            isShowing = true;
            yield return new WaitForSeconds(2f);
            isShowing = false;
            imageManager.LoadTarget();
        }

        private void OnDestroy()
        {
            if (Skin)
            {
                Destroy(Skin);
            }
        }
    }
}
