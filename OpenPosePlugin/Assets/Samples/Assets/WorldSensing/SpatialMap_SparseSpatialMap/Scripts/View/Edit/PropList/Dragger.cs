//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using Common;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SpatialMap_SparseSpatialMap
{
    public class Dragger : MonoBehaviour
    {
        public GameObject OutlinePrefab;
        public GameObject FreeMove;
        public UnityEngine.UI.Toggle VideoPlayable;

        private RectTransform rectTransform;
        private UnityEngine.UI.Image dummy;
        private TouchController touchControl;
        private MapSession mapSession;
        private GameObject candidate;
        private GameObject selection;
        private bool isOnMap;
        private bool isMoveFree = true;

        public event Action<GameObject> CreateObject;
        public event Action<GameObject> DeleteObject;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            dummy = GetComponentInChildren<UnityEngine.UI.Image>(true);
            touchControl = GetComponentInChildren<TouchController>(true);
            OutlinePrefab = Instantiate(OutlinePrefab);
            OutlinePrefab.SetActive(false);
        }

        private void Update()
        {
            transform.position = Input.mousePosition;
            var isEditorOrStandalone = Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer;
            var isPointerOverGameObject = (isEditorOrStandalone && EventSystem.current.IsPointerOverGameObject())
                || (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId));

            if (candidate)
            {
                if (mapSession != null && !isPointerOverGameObject && Input.touchCount > 0)
                {
                    var point = mapSession.HitTestOne(new Vector2(Input.touches[0].position.x / Screen.width, Input.touches[0].position.y / Screen.height));
                    if (point.OnSome)
                    {
                        candidate.transform.position = point.Value + Vector3.up * candidate.transform.localScale.y / 2;
                        isOnMap = true;
                    }
                }

                if (isPointerOverGameObject || !isOnMap)
                {
                    HideCandidate();
                }
                else
                {
                    ShowCandidate();
                }
            }
            else
            {
                if (!isPointerOverGameObject && ((isEditorOrStandalone && Input.GetMouseButtonDown(0)) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        StopEdit();
                        StartEdit(hitInfo.collider.gameObject);
                    }
                }
            }

            if (mapSession != null && selection && !isMoveFree)
            {
                if (!isPointerOverGameObject && Input.touchCount == 1)
                {
                    var point = mapSession.HitTestOne(new Vector2(Input.touches[0].position.x / Screen.width, Input.touches[0].position.y / Screen.height));
                    if (point.OnSome)
                    {
                        selection.transform.position = point.Value + Vector3.up * selection.transform.localScale.y / 2;
                    }
                }
            }
        }

        private void OnDisable()
        {
            mapSession = null;
            StopEdit();
        }

        public void SetMapSession(MapSession session)
        {
            mapSession = session;
            if (mapSession.MapWorker)
            {
                mapSession.MapWorker.MapLoad += (arg1, arg2, arg3, arg4) =>
                {
                    StartCoroutine(CheckVideo());
                };
            }
        }

        public void SetFreeMove(bool free)
        {
            isMoveFree = free;
            if (selection)
            {
                if (free)
                {
                    touchControl.TurnOn(selection.transform, Camera.main, true, true, true, true);
                }
                else
                {
                    touchControl.TurnOn(selection.transform, Camera.main, false, false, true, true);
                }
            }
        }

        public void StartCreate(PropCellController controller)
        {
            StopEdit();
            isOnMap = false;
            rectTransform.sizeDelta = controller.GetComponent<RectTransform>().sizeDelta;
            dummy.sprite = controller.Templet.Icon;
            dummy.color = Color.white;
            candidate = Instantiate(controller.Templet.Object);
            candidate.name = controller.Templet.Object.name;
            if (candidate)
            {
                var video = candidate.GetComponentInChildren<VideoPlayerAgent>(true);
                if (video) { video.Playable = false; }
            }
            FreeMove.SetActive(false);
            HideCandidate();
        }

        public void StopCreate()
        {
            if (candidate.activeSelf)
            {
                if (CreateObject != null)
                {
                    CreateObject(candidate);
                    StartEdit(candidate);
                }
            }
            else
            {
                Destroy(candidate);
            }

            dummy.gameObject.SetActive(false);
            FreeMove.SetActive(true);
            isOnMap = false;
            candidate = null;
        }

        public void StartEdit(GameObject obj)
        {
            selection = obj;
            if (selection && VideoPlayable.isOn)
            {
                var video = selection.GetComponentInChildren<VideoPlayerAgent>(true);
                if (video) { video.Playable = true; }
            }

            var meshFilter = selection.GetComponentInChildren<MeshFilter>();
            OutlinePrefab.SetActive(true);
            OutlinePrefab.GetComponent<MeshFilter>().mesh = meshFilter.mesh;
            OutlinePrefab.transform.parent = meshFilter.transform;
            OutlinePrefab.transform.localPosition = Vector3.zero;
            OutlinePrefab.transform.localRotation = Quaternion.identity;
            OutlinePrefab.transform.localScale = Vector3.one;

            SetFreeMove(isMoveFree);
        }

        public void StopEdit()
        {
            if (selection)
            {
                var video = selection.GetComponentInChildren<VideoPlayerAgent>(true);
                if (video) { video.Playable = false; }
            }
            selection = null;
            if (OutlinePrefab)
            {
                OutlinePrefab.transform.parent = null;
                OutlinePrefab.SetActive(false);
            }
            if (touchControl)
            {
                touchControl.TurnOff();
            }
        }

        public void DeleteSelection()
        {
            if (!selection)
            {
                return;
            }
            if (DeleteObject != null)
            {
                DeleteObject(selection);
            }
            Destroy(selection);
            StopEdit();
        }

        public void ToggleVideoPlayable(bool playable)
        {
            if (selection)
            {
                var video = selection.GetComponentInChildren<VideoPlayerAgent>(true);
                if (video) { video.Playable = playable; }
            }
        }

        private void ShowCandidate()
        {
            dummy.gameObject.SetActive(false);
            candidate.SetActive(true);
        }

        private void HideCandidate()
        {
            candidate.SetActive(false);
            dummy.gameObject.SetActive(true);
        }

        private IEnumerator CheckVideo()
        {
            yield return new WaitForEndOfFrame();
            if (mapSession == null) { yield return 0; }
            foreach (var prop in mapSession.Maps[0].Props)
            {
                if (prop)
                {
                    var video = prop.GetComponentInChildren<VideoPlayerAgent>(true);
                    if (video) { video.Playable = false; }
                }
            }
        }
    }
}
