using System.Collections.Generic;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace Nebukam.Cluster
{

    [CreateAssetMenu(fileName = "SlotModelData", menuName = "Nebukam/Slate/SlotModelData", order = 1)]
    public class SlotModelData : ScriptableObject
    {

        protected SlotModel m_slotModel = new SlotModel();

        [Header("Slot settings")]
        public float3 slotSize = float3(1f);
        public float3 slotAnchor = float3(0f);

        public SlotModel model {
            get
            {
                m_slotModel.m_slotSize = slotSize;
                m_slotModel.anchor = slotAnchor;
                return m_slotModel;
            }
        }

        public static implicit operator SlotModel(SlotModelData m) { return m.model; }

    }

}
