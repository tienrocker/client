namespace Photon.LoadBalancing.Custom.Common.Quiz
{
    public enum SongGameState
    {
        NONE,
        WAIT, // wait client download
        READY, // client send ready state to server
        START, // start game
        PLAYING,
        RESULT,
        WAIT_NEXT_QUESTION,
        WAIT_NEXT_ROUND,
        END,
    }
}