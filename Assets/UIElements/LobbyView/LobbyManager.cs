using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager
{
    int maxPlayers = 2;
    public async Task<Lobby> TryCreatePublicLobbyAsync(string lobbyName)
    {
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = false;
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        return lobby;
    }
    public async Task<Lobby> TryCreatePrivateLobbyAsync(string lobbyName)
    {
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = true;
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        return lobby;
    }

    public async Task<List<Lobby>> GetAllLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 5;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            options.Order = new List<QueryOrder>()
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
            return lobbies.Results;
        } 
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return new List<Lobby>();
        }
    }

    public async void JoinLobby(string id)
    {
            try
            {
                await LobbyService.Instance.JoinLobbyByIdAsync(id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
    }

    void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}