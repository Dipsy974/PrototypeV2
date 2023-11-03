public abstract class BaseState : IState
{

    protected readonly StM_PlayerController _playerController;
    protected readonly StM_InputReader _input;

    protected BaseState(StM_PlayerController player, StM_InputReader input)
    {
        this._playerController = player;
        this._input = input;
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