using Unity.Netcode.Components;
public class ClientNetworkAnimator : NetworkTransform
{ 
    protected override bool OnIsServerAuthoritative() => false;
}
