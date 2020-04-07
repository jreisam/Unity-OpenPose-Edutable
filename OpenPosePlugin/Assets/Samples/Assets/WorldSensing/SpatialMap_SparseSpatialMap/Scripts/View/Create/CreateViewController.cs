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
using UnityEngine;
using UnityEngine.UI;

namespace SpatialMap_SparseSpatialMap
{
    public class CreateViewController : MonoBehaviour
    {
        public GameObject Tips;
        public GameObject UploadPopup;
        public GameObject InfoWithPreview;
        public GameObject InfoWithoutPreview;
        public InputField MapNameInput;
        public Button UploadOKButton;
        public Button UploadCancelButton;
        public Button SaveButton;
        public Button TipsButton;
        public Button SnapshotButton;
        public Toggle PreviewToggle;
        public RawImage PreviewImage;
        public UnityEngine.UI.Image PreviewImageBorder;
        public Text SaveStatus;

        private MapSession mapSession;
        private bool isTipsOn;
        private bool withPreview = true;
        private string mapName;
        private Texture2D capturedImage;
        private int uploadingTime;

        private void OnEnable()
        {
            TipsButton.gameObject.SetActive(true);
            SaveButton.gameObject.SetActive(true);
            SaveButton.interactable = false;

            PreviewToggle.isOn = true;

            isTipsOn = false;
            Tips.SetActive(false);

            StopUploadUI();
            UploadPopup.transform.localScale = Vector3.zero;
            UploadPopup.gameObject.SetActive(false);
            var buttonText = UploadOKButton.transform.Find("Text").GetComponent<Text>();
            buttonText.text = "OK";
        }

        private void Update()
        {
            if ((mapSession.MapWorker.LocalizedMap == null || mapSession.MapWorker.LocalizedMap.PointCloud.Count <= 20) && !Application.isEditor)
            {
                SaveButton.interactable = false;
            }
            else
            {
                SaveButton.interactable = true;
            }
        }

        private void OnDestroy()
        {
            if (capturedImage)
            {
                Destroy(capturedImage);
            }
        }

        public void SetMapSession(MapSession session)
        {
            mapSession = session;
        }

        public void Save()
        {
            SaveButton.gameObject.SetActive(false);
            TipsButton.gameObject.SetActive(false);
            UploadPopup.gameObject.SetActive(true);
            MapNameInput.text = mapName = "Map_" + DateTime.Now.ToString("yyyy-MM-dd_HHmm");
            mapSession.MapWorker.enabled = false;
            Snapshot();
            StartCoroutine(ShowPopup(UploadPopup.transform, Vector3.one));
        }

        public void ShowTips()
        {
            isTipsOn = !isTipsOn;
            Tips.SetActive(isTipsOn);
        }

        public void Snapshot()
        {
            var oneShot = Camera.main.gameObject.AddComponent<OneShot>();
            oneShot.Shot(true, (texture) =>
            {
                if (capturedImage)
                {
                    Destroy(capturedImage);
                }
                capturedImage = texture;
                PreviewImage.texture = capturedImage;
            });
        }

        public void TogglePreview(bool enable)
        {
            withPreview = enable;
            InfoWithPreview.SetActive(withPreview);
            InfoWithoutPreview.SetActive(!withPreview);
            PreviewImageBorder.gameObject.SetActive(withPreview);
        }

        public void TogglePreview()
        {
            if (!PreviewToggle.interactable)
            {
                return;
            }
            PreviewToggle.isOn = !withPreview;
        }

        public void OnMapNameChange(string name)
        {
            UploadOKButton.interactable = !string.IsNullOrEmpty(name);
            mapName = name;
        }

        public void Upload()
        {
            using (var buffer = easyar.Buffer.wrapByteArray(capturedImage.GetRawTextureData()))
            using (var image = new easyar.Image(buffer, PixelFormat.RGB888, capturedImage.width, capturedImage.height))
            {
                mapSession.Save(mapName, withPreview ? image : null);
            }
            StartUploadUI();
            StartCoroutine(SavingStatus());
            StartCoroutine(Saving());
        }

        private IEnumerator Saving()
        {
            while (mapSession.IsSaving)
            {
                yield return 0;
            }
            if (mapSession.Saved)
            {
                gameObject.SetActive(false);
                ViewManager.Instance.LoadMainView();
            }
            else
            {
                var buttonText = UploadOKButton.transform.Find("Text").GetComponent<Text>();
                buttonText.text = "Retry";
                StopUploadUI();
            }
        }

        private IEnumerator SavingStatus()
        {
            while (mapSession.IsSaving)
            {
                SaveStatus.text = "Upload and Generate Map.";
                for (int i = 0; i < uploadingTime; ++i)
                {
                    SaveStatus.text += ".";
                }
                uploadingTime = (uploadingTime + 1) % 3;
                yield return new WaitForSeconds(1);
            }
            SaveStatus.text = "Upload and Generate Map";
        }

        private static IEnumerator ShowPopup(Transform t, Vector3 scale)
        {
            while (t.transform.localScale.x < scale.x)
            {
                t.transform.localScale += scale * 6 * Time.deltaTime;
                if (t.transform.localScale.x > scale.x)
                {
                    t.transform.localScale = scale;
                }
                yield return 0;
            }
        }

        private void StartUploadUI()
        {
            UploadOKButton.interactable = false;
            PreviewToggle.interactable = false;
            MapNameInput.interactable = false;
            UploadCancelButton.interactable = false;
            SnapshotButton.interactable = false;
            uploadingTime = 0;
        }

        private void StopUploadUI()
        {
            SaveStatus.text = "Upload and Generate Map";
            UploadOKButton.interactable = true;
            PreviewToggle.interactable = true;
            MapNameInput.interactable = true;
            UploadCancelButton.interactable = true;
            SnapshotButton.interactable = true;
            uploadingTime = 0;
        }
    }
}
