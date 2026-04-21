using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkScript : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;

    }

}
