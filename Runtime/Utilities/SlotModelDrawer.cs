using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Nebukam.Cluster
{
    public class SlotModelDrawer : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool draw = true;
        public SlotModelData model;
        public Color color = Color.white;
        public bool wire = false;

        private void OnDrawGizmos()
        {
            SlotModel m = model?.model;

            if(!draw || m == null)
                return;

            Gizmos.color = color;

            if (wire)
                Gizmos.DrawWireCube(m.m_offset + (float3)transform.position, m.size);
            else
                Gizmos.DrawCube(m.m_offset + (float3)transform.position, m.size);

        }
#endif
    }
}
