using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class AttachNode
{
    public string name;
    public Transform transform;
    public SpacecraftPart part;
}

[SerializeField]
public class SpacecraftPartConfig
{
    public Mesh mesh;
    public MeshCollider collider;
    public float mass;
    public AttachNode root;
    public List<AttachNode> nodes;
}
