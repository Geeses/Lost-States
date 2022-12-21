using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using System.Threading.Tasks;
using System;
// Game developer code
public class InitializeServices : MonoBehaviour
{

    async void Start()
    {
        await UnityServices.InitializeAsync();
        Debug.Log(UnityServices.State);
    }

    //public async Task SwitchProfileAndReSignInAsync(string profile)
    //{
    //    if (AuthenticationService.Instance.IsSignedIn)
    //    {
    //        AuthenticationService.Instance.SignOut();
    //    }

    //    AuthenticationService.Instance.SwitchProfile(profile);
    //    try
    //    {
    //        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    //    }
    //    catch (Exception e)
    //    {
    //        var reason = $"{e.Message} ({e.InnerException?.Message})";
    //        Debug.Log(reason);
    //        throw;
    //    }
    //}
}