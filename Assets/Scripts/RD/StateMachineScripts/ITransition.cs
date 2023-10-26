using UnityEngine.UI;

public interface ITransition
{
    IState To { get; }
    IPredicate Condition { get; }
}