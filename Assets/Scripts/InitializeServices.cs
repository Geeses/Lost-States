using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
// Game developer code
public class InitializeServices : MonoBehaviour
{
    async void Start()
    {
        // UnityServices.InitializeAsync() will initialize all services that are subscribed to Core.
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log(UnityServices.State);
    }
}