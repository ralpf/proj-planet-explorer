using UnityEngine;



namespace Extensions.UnityAPI
{
    public static class Extension
    {
        public static void Reset(this Transform xf)
        {
            if (!xf) return;
            xf.localPosition = Vector3.zero;
            xf.localRotation = Quaternion.identity;
        }

        public static void DestroyChildren(this Transform xf)
        {
            if (!xf) return;
            while (xf.childCount > 0)
                xf.GetChild(0).gameObject.DestroySafe();
        }

        public static void DestroySafe(this GameObject go)
        {
            if (!go) return;
            if (Application.isPlaying) Object.Destroy(go);
            else Object.DestroyImmediate(go);
        }
    }
}