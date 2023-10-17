public abstract class BaseState : IState
{

    protected readonly StM_PlayerController _playerController;

    protected BaseState(StM_PlayerController player)
    {
        this._playerController = player;
    }
    
    public virtual void OnEnter()
    {
        //noop
    }

    public virtual void Update()
    {
        //noop
    }

    public virtual void FixedUpdate()
    {
        //noop
    }

    public virtual void OnExit()
    {
        //noop
    }
}