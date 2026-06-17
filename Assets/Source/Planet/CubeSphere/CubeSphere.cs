using UnityEngine;
using Planets.Profiles;



namespace Planets.Generation
{
    public class CubeSphere : MonoBehaviour
    {
        [SerializeField, Range(2,128)] int resolution = 32;
        [SerializeField] PlanetProfile profile;
        [SerializeField] Material material;
        
        CubeSphereFace[] faces;

        [ContextMenu("Regenerate Planet")]
        private void Generate()
        {
            Clear();
            if (!profile) return;
            faces = new CubeSphereFace[6];

            for (int i = 0; i < 6; ++i)
            {
                faces[i] = new CubeSphereFace(transform, FaceNormals[i], (float)profile.radius, resolution, material);
                faces[i].Generate();
            }
        }

        [ContextMenu("Clear")]
        private void Clear()
        {
            while (transform.childCount > 0)
            {
                var xf = transform.GetChild(0);
                if (Application.isPlaying) Destroy(xf.gameObject);
                else DestroyImmediate(xf.gameObject);
            }
        }
        

        private static readonly Vector3[] FaceNormals = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
    }
}