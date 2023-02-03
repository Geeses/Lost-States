using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Collections;
using Unity.Services.Lobbies;

// Game developer code
public class InitializeServices: Singleton<InitializeServices>
{
    public async void InitializeWithUsername(string username)
    {
        var options = new InitializationOptions();
        options.SetProfile(username);

        try
        {
            await UnityServices.InitializeAsync(options);
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
        Debug.Log($"Profile: {AuthenticationService.Instance.Profile}");
        Debug.Log($"Is SignedIn: {AuthenticationService.Instance.IsSignedIn}");
        Debug.Log($"Is Authorized: {AuthenticationService.Instance.IsAuthorized}");
        Debug.Log($"Is Expired: {AuthenticationService.Instance.IsExpired}");
        Debug.Log("Analytics UserID: " + AnalyticsService.Instance.GetAnalyticsUserID());
    }

    public void InitializeHeartbeatLobbyCoroutine(string lobbyId, int seconds)
    {
        StartCoroutine(HeartbeatLobbyCoroutine(lobbyId, seconds));
    }

    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            Debug.Log("Heartbit for lobby sended");
            yield return delay;
        }
    }
}