using UnityEngine;

namespace HologramShadersPro
{
    public class UnscaledTime : MonoBehaviour
    {
        private Material[] materials;

        private void Start()
        {
            var renderer = GetComponent<Renderer>();
            materials = renderer.materials;
        }

        private void Update()
        {
            float unscaledTime = Time.unscaledTime;

            for(int i = 0; i < materials.Length; ++i)
            {
                materials[i].SetFloat("_UnscaledTime", unscaledTime);
            }
        }
    }
}
