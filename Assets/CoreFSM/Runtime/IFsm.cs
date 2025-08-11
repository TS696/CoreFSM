using System.Text;

namespace CoreFSM
{
    public interface IFsm
    {
        void Dump(StringBuilder sb);
    }

    public interface IFsm<TFsm> : IFsm where TFsm : IFsm<TFsm>
    {
        IState<TFsm> CurrentState { get; }

        void IFsm.Dump(StringBuilder sb)
        {
            sb.Append(CurrentState.GetType());

            if (CurrentState is IFsm fsm)
            {
                sb.Append(" -> ");
                fsm.Dump(sb);
            }
        }
    }
}
