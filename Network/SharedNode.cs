#if GODOT4_0_OR_GREATER
namespace Cutulu.Network;

using System;
using Godot;

public partial class SharedNode : Node, IShared
{
    [Export] public Node Client { get; set; }
    [Export] public Node Host { get; set; }

    [Export] public Node[] Shared { get; set; }

    public virtual T Unpack<T>(Node parent, bool asClient, Action<Node> sharedChildren = null) => (this as IShared).DefaultSharedUnpackNode<T>(parent, asClient, sharedChildren);

    public virtual void _Unpack(bool asClient) { }

    public bool DestroyAfterUnpacking() => true;
}
#endif