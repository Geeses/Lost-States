public struct LobbyData
{
    public string _name;
    private int _usersAllowed;
    public int _usersJoined;

    public LobbyData(string name, int usersAllowed, int usersJoined)
    {
        _name = name;
        _usersAllowed = usersAllowed;
        _usersJoined = usersJoined;

    }
    public string usersInLobby
    {
        get { return _usersJoined + "/" + _usersAllowed; }
    }
}
