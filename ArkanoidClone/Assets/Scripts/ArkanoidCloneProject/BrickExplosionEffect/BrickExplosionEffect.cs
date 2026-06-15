using UnityEngine;

namespace ArkanoidCloneProject.VFX
{
    // Self-managing MonoBehaviour — created by BrickExplosionController, destroys itself when done.
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class BrickExplosionEffect : MonoBehaviour
    {
        private static readonly int ProgressId     = Shader.PropertyToID("_Progress");
        private static readonly int ColorId        = Shader.PropertyToID("_Color");

        // The quad covers the brick plus expansion room so particles are visible outside the brick bounds.
        private const float ExpansionScale = 3f;
        private const int   SortingOrder   = 10;

        private MeshRenderer         _renderer;
        private MaterialPropertyBlock _block;
        private float                _duration;
        private float                _elapsed;

        public static void Spawn(Vector3 position, Vector3 brickWorldSize, Color color, float duration, Material sharedMaterial)
        {
            var go = new GameObject("BrickExplosion");
            go.transform.position   = position;
            go.transform.localScale = brickWorldSize * ExpansionScale;

            var mf   = go.AddComponent<MeshFilter>();
            mf.mesh  = BuildQuad();

            var mr              = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial   = sharedMaterial;
            mr.sortingOrder     = SortingOrder;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows   = false;

            var effect        = go.AddComponent<BrickExplosionEffect>();
            effect._renderer  = mr;
            effect._block     = new MaterialPropertyBlock();
            effect._duration  = duration;
            effect._elapsed   = 0f;

            effect._block.SetColor(ColorId,   color);
            effect._block.SetFloat(ProgressId, 0f);
            mr.SetPropertyBlock(effect._block);
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(_elapsed / _duration);

            _block.SetFloat(ProgressId, progress);
            _renderer.SetPropertyBlock(_block);

            if (progress >= 1f)
                Destroy(gameObject);
        }

        private static Mesh BuildQuad()
        {
            var mesh = new Mesh { name = "ExplosionQuad" };
            mesh.vertices  = new[] { new Vector3(-0.5f, -0.5f), new Vector3(0.5f, -0.5f), new Vector3(-0.5f, 0.5f), new Vector3(0.5f, 0.5f) };
            mesh.uv        = new[] { Vector2.zero, Vector2.right, Vector2.up, Vector2.one };
            mesh.triangles = new[] { 0, 2, 1, 2, 3, 1 };
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
