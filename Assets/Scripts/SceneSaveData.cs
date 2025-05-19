using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJT.CETIM
{
    [Serializable]
    public class AnchorSaveData
    {
        public string prefabName;
        public Vector3 position;
        public Quaternion rotation;
    }

    [Serializable]
    public class SceneSaveData
    {
        public string sceneName;
        public List<AnchorSaveData> anchors = new List<AnchorSaveData>();
    }
}
