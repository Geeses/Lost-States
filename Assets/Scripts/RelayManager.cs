using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : Singleton<RelayManager>
{
    [SerializeField]
    private string environment = "production";

    [SerializeField]
    private int maxNumberOfConnections = 4;

    public bool IsRelayEnabled => Transport != null && Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;

    public UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

    public async Task<RelayHostData> SetupRelay(string username)
    {
        var options = new InitializationOptions();
        options.SetEnvironmentName(environment);
        options.SetProfile(username);

        await UnityServices.InitializeAsync(options);

        if (AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SignOut();
        }

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
        Debug.Log($"Is SignedIn: {AuthenticationService.Instance.IsSignedIn}");
        Debug.Log($"Is Authorized: {AuthenticationService.Instance.IsAuthorized}");
        Debug.Log($"Is Expired: {AuthenticationService.Instance.IsExpired}");
        AuthenticationService.Instance.SignInFailed += (err) => {
            Debug.LogError(err);
        };

        Debug.Log($"Relay Server Starting With Max Connections: {maxNumberOfConnections}");

        Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxNumberOfConnections);

        RelayHostData relayHostData = new RelayHostData
        {
            Key = allocation.Key,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            IPv4Address = allocation.RelayServer.IpV4,
            ConnectionData = allocation.ConnectionData
        };

        relayHostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(relayHostData.AllocationID);

        Transport.SetRelayServerData(relayHostData.IPv4Address, relayHostData.Port, relayHostData.AllocationIDBytes,
                relayHostData.Key, relayHostData.ConnectionData);
        PlayerInfo info = await AuthenticationService.Instance.GetPlayerInfoAsync();

        Debug.Log($"Relay Server Generated Join Code: {relayHostData.JoinCode}");

        return relayHostData;
    }

    public async Task<RelayJoinData> JoinRelay(string joinCode, string username)
    {
        var options = new InitializationOptions();
        options.SetEnvironmentName(environment);
        options.SetProfile(username);

        await UnityServices.InitializeAsync(options);

        if (AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SignOut();
        }

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
        Debug.Log($"Is SignedIn: {AuthenticationService.Instance.IsSignedIn}");
        Debug.Log($"Is Authorized: {AuthenticationService.Instance.IsAuthorized}");
        Debug.Log($"Is Expired: {AuthenticationService.Instance.IsExpired}");
        AuthenticationService.Instance.SignInFailed += (err) => {
            Debug.LogError(err);
        };

        Debug.Log($"Client Joining Game With Join Code: {joinCode}");

        JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

        RelayJoinData relayJoinData = new RelayJoinData
        {
            Key = allocation.Key,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            IPv4Address = allocation.RelayServer.IpV4,
            JoinCode = joinCode
        };

        Transport.SetRelayServerData(relayJoinData.IPv4Address, relayJoinData.Port, relayJoinData.AllocationIDBytes,
            relayJoinData.Key, relayJoinData.ConnectionData, relayJoinData.HostConnectionData);

        Debug.Log($"Client Joined Game With Join Code: {joinCode}");

        return relayJoinData;
    }
}