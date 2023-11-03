public abstract class BaseState : IState
{

    protected readonly StM_PlayerController _playerController;
    protected readonly CharacterControlsInput _input;

    protected BaseState(StM_PlayerController player, CharacterControlsInput input)
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