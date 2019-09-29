using Nebukam;
using Nebukam.Cluster;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

public class ClusterSetup : MonoBehaviour
{

    public int3 size = int3(1,1,1);
    public float debugRadius = 0.01f;
    public float3 slotSize = float3(1f);
    public float3 slotAnchor = float3(0.5f);

    public Transform positionTester;

    protected SlotCluster<Slot, ClusterBrain> m_cluster;
    protected SlotModel m_model = new SlotModel();

    private void Awake()
    {
        m_model.size = slotSize;
        m_model.anchor = slotAnchor;

        m_cluster = Nebukam.Pooling.Pool.Rent<SlotClusterFixed<Slot, ClusterBrain>>();
        m_cluster.Init(m_model, size, true);
    }

    private void Update()
    {
        ISlot slot;
        debugRadius = length(slotSize) * 0.5f;

        for (int i = 0, count = m_cluster.Count; i < count; i++)
        {
            slot = m_cluster[i];
            Nebukam.Utils.Draw.Cube(slot.pos, debugRadius, Color.red);
        }

        if(m_cluster.TryGet(positionTester.position, out slot))
        {
            Nebukam.Utils.Draw.Cube(slot.pos, debugRadius+0.1f, Color.green);
        }
               
    }

}

