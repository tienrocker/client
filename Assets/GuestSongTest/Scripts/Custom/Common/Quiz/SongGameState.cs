namespace Photon.LoadBalancing.Custom.Common.Quiz
{
    public enum SongGameState
    {
        NONE,
        WAIT, // wait client download
        READY, // client send ready state to server
        START, // start game
        ROUND1,
        ROUND2,
        END,
    }
}