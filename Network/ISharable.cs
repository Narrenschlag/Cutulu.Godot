#if GODOT4_0_OR_GREATER
namespace Cutulu.Network;

using System;
using Godot;

public interface ISharable
{
    public T Unpack<T>(Node parent, bool asClient, Action<Node> sharedChildren = null);
    public bool DestroyAfterUnpacking();
}
#endif