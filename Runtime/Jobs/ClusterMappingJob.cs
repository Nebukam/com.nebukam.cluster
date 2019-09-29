// Copyright (c) 2019 Timothé Lapetite - nebukam@gmail.com.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Nebukam.Cluster
{

    /// <summary>
    /// Handle the mapping of coordinate to index.
    /// Since NativeHashMap is super slow on the main thread, it has a dedicated job.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="T"></typeparam>
    [BurstCompile]
    public struct ClusterMappingJob<S, T> : IJob
        where S : Slot, ISlot
        where T : struct, ISlotInfos<S>
    {

        [ReadOnly]
        public NativeArray<T> m_inputSlotInfos;

        public NativeHashMap<ByteTrio, int> m_coordinateMap;

        public void Execute()
        {
            for (int i = 0, count = m_inputSlotInfos.Length; i < count; i++)
                m_coordinateMap.TryAdd(m_inputSlotInfos[i].coord, i);
        }

    }
}
