using UnityEngine;

public class GameplayStatusService : SubService
{
    private StatusUIController statusUIController;

    public override void Initialize(GameplayService gameplayService)
    {
        this.gameplayService = gameplayService;
        statusUIController = gameplayService.View.StatusUIController;
    }


}
