public abstract class BaseState : IState
{

    protected readonly StM_PlayerController _playerController;

    protected BaseState(StM_PlayerController player)
    {
        this._playerController = player;
    }
    
    public void OnEnter()
    {
        //noop
    }

    public void Update()
    {
        //noop
    }

    public void FixedUpdate()
    {
        //noop
    }

    public void OnExit()
    {
        //noop
    }
}