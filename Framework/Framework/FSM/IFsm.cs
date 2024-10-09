
public interface IFsmState
{
    void Enter(params object[] param);
    void Update();
    void Exit();
}
