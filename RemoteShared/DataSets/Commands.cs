namespace RemoteShared.DataSets
{
    public enum Commands : byte
    {
        Login,
        CreateUser,
        JoinRoom,
        ListRooms,
        CreateRoom,
        InviteUser,
        FindUser
    }
}
