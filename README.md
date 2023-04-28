![preview](https://img.shields.io/badge/-stable-darkgreen.svg)
![version](https://img.shields.io/badge/dynamic/json?color=blue&label=version&query=version&url=https%3A%2F%2Fraw.githubusercontent.com%2FNebukam%2Fcom.nebukam.cluster%2Fmaster%2Fpackage.json)
![in development](https://img.shields.io/badge/license-MIT-black.svg)
[![doc](https://img.shields.io/badge/documentation-darkgreen.svg)](https://nebukam.github.io/docs/unity/com.nebukam.cluster/)

# N:Cluster
#### Space partitioning library

N:Cluster is a Library to manage abstract, neighbor-based structures that can fit in a 3d-indexed structure (along 3 separate axis)

### Features
N:ORCA is currently in development, and while the repo is available to use and download, there is no documentation yet.

### High level concept
A Cluster is a 3D array on steroids, with extended functionalities such as **lookup wrapping**. Each slot has a size and position in space, at the cost of overall size : **a single cluster is limited to 255 x 255 x 255**.

It can be used for 1D & 2D structures as well, although given the way to works, it was more efficient to build it with 3 dimensions from the ground up. Each cluster's positionning and indexing functionalities are embedded in a ```ClusterBrain``` struct that manages how a cluster position its content as well as how indices are computed and retrieved.

---
## Hows

### Installation
> To be used with [Unity's Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html).  
> ⚠ [Git Dependency Resolver For Unity](https://github.com/mob-sakai/GitDependencyResolverForUnity) must be installed *before* in order to fetch nested git dependencies. (See the [Installation troubleshooting](#installation-troubleshooting) if you encounter issues).  

See [Unity's Package Manager : Getting Started](https://docs.unity3d.com/Manual/upm-parts.html)

### Quick Start

There is two main cluster you can use : ```ClusterSlotFlex<V, B>``` & ```ClusterSlotFixed<V, B>```.
Both are derived from the abstract ```SlotCluster<V, B>```, but manage their memory very differently :

- ```ClusterSlotFlex<V, B>``` relies on a List. As such, it is a good candidate when in need for a partially-filled 3D grid.
- ```ClusterSlotFixes<V, B>``` relies on an array. As such, it comes with a larger base memory footprint, but scales better when you need a full or near-full cluster.


```CSharp

    using Nebukam.Cluster;
    using Unity.Mathematics;

    ...

    // Create a cluster
    m_cluster = new SlotClusterFixed<Slot, CylindricBrain>();

    // Make sure to initialize it before using it
    // It requires a slot model, and max boundaries.
    // The slot model hold a single slot properties (size, etc)
    model = new SlotModel();

    // Init the cluster as a 10 x 10 x 10 cube
    m_cluster.Init(model, int3(10,10,10));

    ...


```

From there you can :

```CSharp

    // Try to get the slot at x:0, y:0, z:0 if it exists
    m_cluster.TryGet(0, 0, 0, out ISlot slot);

```

```CSharp

    // Try to get the slot containing an arbitrary position
    m_cluster.TryGet(float3(1.2f, 5.0f, 8.0f), out ISlot slot);

```

```CSharp

    // Get or create a slot at a given position
    ISlot slot = m_cluster.Add(0, 0, 0);

```

---
## Dependencies
- **Unity.Burst 1.1.2** [com.unity.burst]()
- **Unity.Jobs 0.0.7** [com.unity.jobs]()
- **Unity.Collections 1.3.1** [com.unity.collections]()
- **Unity.Mathematics 1.1.0** -- [com.unity.mathematics](https://github.com/Unity-Technologies/Unity.Mathematics)
- **N:Common** -- [com.nebukam.common](https://github.com/Nebukam/com.nebukam.common.git)
- **N:JobAssist** -- [com.nebukam.job-assist](https://github.com/Nebukam/com.nebukam.job-assist.git)

---
## Installation Troubleshooting

After installing this package, Unity may complain about missing namespace references error (effectively located in dependencies). What [Git Dependency Resolver For Unity](https://github.com/mob-sakai/GitDependencyResolverForUnity) does, instead of editing your project's package.json, is create local copies of the git repo *effectively acting as custom local packages*.
Hence, if you encounter issues, try the following:
- In the project explorer, do a ```Reimport All``` on the **Packages** folder (located at the same level as **Assets** & **Favorites**). This should do the trick.
- Delete Library/ScriptAssemblies from you project, then ```Reimport All```.
- Check the [Resolver usage for users](https://github.com/mob-sakai/GitDependencyResolverForUnity#usage)
