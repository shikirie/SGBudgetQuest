public abstract class SubService
{
    protected GameplayService gameplayService;
    public virtual void Initialize(GameplayService gameplayService)
    {
        this.gameplayService = gameplayService;
    }
    public virtual void Start() { }
    public virtual void Tick() { }
    public virtual void Dispose() { }
}