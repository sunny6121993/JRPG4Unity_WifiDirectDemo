public interface ITurnHandler
{
    int GetCurrentTurn();
    void OnTurnEnd(int currentTurn);
}