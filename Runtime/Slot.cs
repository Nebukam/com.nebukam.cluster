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

namespace Nebukam.Cluster
{

    public interface ISlot : IVertex
    {
        ISlotCluster<ISlot> cluster { get; set; }
        ByteTrio coordinates { get; set; }
    }

    public class Slot : Vertex, ISlot, Pooling.IRequireCleanUp
    {

        internal ByteTrio m_coordinates = ByteTrio.zero;
        internal ISlotCluster<ISlot> m_cluster = null;

        public ByteTrio coordinates
        {
            get { return m_coordinates; }
            set { m_coordinates = value; }
        }

        public ISlotCluster<ISlot> cluster
        {
            get { return m_cluster; }
            set { m_cluster = value; }
        }
        
        public virtual void CleanUp()
        {
            m_coordinates = ByteTrio.zero;
        }

    }
}
