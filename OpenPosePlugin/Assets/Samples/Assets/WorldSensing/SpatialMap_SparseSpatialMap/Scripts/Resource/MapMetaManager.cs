//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SpatialMap_SparseSpatialMap
{
    public class MapMetaManager
    {
        private static readonly string root = Application.persistentDataPath + "/SparseSpatialMap";

        public static List<MapMeta> LoadAll()
        {
            var metas = new List<MapMeta>();
            var dirRoot = GetRootPath();
            try
            {
                foreach (var path in Directory.GetFiles(dirRoot, "*.meta"))
                {
                    try
                    {
                        metas.Add(JsonUtility.FromJson<MapMeta>(File.ReadAllText(path)));
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
            return metas;
        }

        public static bool Save(MapMeta meta)
        {
            try
            {
                File.WriteAllText(GetPath(meta.Map.ID), JsonUtility.ToJson(meta));
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
            return true;
        }

        public static bool Delete(MapMeta meta)
        {
            if (!File.Exists(GetPath(meta.Map.ID)))
            {
                return false;
            }
            try
            {
                File.Delete(GetPath(meta.Map.ID));
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
            return true;
        }

        private static string GetRootPath()
        {
            if (!File.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
            return root;
        }

        private static string GetPath(string id)
        {
            return GetRootPath() + "/" + id + ".meta";
        }
    }
}
