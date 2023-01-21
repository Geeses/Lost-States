using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Analytics;
using System.Collections.Generic;

// Game developer code
public class InitializeServices: Singleton<InitializeServices>
{

    [SerializeField]
    private string environment = "production";

    public async void InitializeWithUsername(string username)
    {
        var options = new InitializationOptions();
        options.SetEnvironmentName(environment);
        options.SetProfile(username);

        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            // Check Consents
            List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
        }
        catch (ConsentCheckException e)
        {
            var reason = $"{e.Message} ({e.InnerException?.Message})";
            Debug.Log(reason);
            throw;
        }

        // General Player Info
        Debug.Log(UnityServices.State);
        Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
        Debug.Log($"Is SignedIn: {AuthenticationService.Instance.IsSignedIn}");
        Debug.Log($"Is Authorized: {AuthenticationService.Instance.IsAuthorized}");
        Debug.Log($"Is Expired: {AuthenticationService.Instance.IsExpired}");
        Debug.Log("Analytics UserID: " + AnalyticsService.Instance.GetAnalyticsUserID());
    }
}