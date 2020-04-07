//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SpatialMap_SparseSpatialMap
{
    public class MapDockController : MonoBehaviour
    {
        public Button OpenButton;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (((Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer) && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
            {
                ShowAndHide(OpenButton.gameObject.activeSelf);
            }
        }

        public void ShowAndHide(bool isShow)
        {
            StopAllCoroutines();
            if (isShow)
            {
                StartCoroutine(Show());
            }
            else
            {
                StartCoroutine(Hide());
            }
        }

        private IEnumerator Show()
        {
            var offsetMax = rectTransform.offsetMax;
            var offsetMin = rectTransform.offsetMin;
            OpenButton.gameObject.SetActive(false);
            while (offsetMax.x < 0)
            {
                offsetMax.x += Screen.width * Time.deltaTime;
                offsetMin.x += Screen.width * Time.deltaTime;
                rectTransform.offsetMax = offsetMax;
                rectTransform.offsetMin = offsetMin;
                if (offsetMax.x > 0)
                {
                    offsetMax.x = 0;
                    offsetMin.x = 0;
                    rectTransform.offsetMax = offsetMax;
                    rectTransform.offsetMin = offsetMin;
                }
                yield return 0;
            }
        }

        private IEnumerator Hide()
        {
            var offsetMax = rectTransform.offsetMax;
            var offsetMin = rectTransform.offsetMin;
            var width = rectTransform.rect.width + Screen.width * 0.01f;
            while (offsetMax.x > -width && offsetMin.x > -width)
            {
                offsetMax.x -= Screen.width * Time.deltaTime;
                offsetMin.x -= Screen.width * Time.deltaTime;
                rectTransform.offsetMax = offsetMax;
                rectTransform.offsetMin = offsetMin;
                if (offsetMax.x < -width)
                {
                    offsetMax.x = -width;
                    offsetMin.x = -width;
                    rectTransform.offsetMax = offsetMax;
                    rectTransform.offsetMin = offsetMin;
                }
                yield return 0;
            }
            OpenButton.gameObject.SetActive(true);
        }
    }
}
