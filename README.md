# CoreFSM
A simple, DI-friendly state machine library for Unity.

## Installation
Add the following line directly to Packages/manifest.json:
```json
"com.ts696.corefsm": "https://github.com/TS696/CoreFSM.git?path=Assets/CoreFSM#1.0.0"
```

## Basic usage
This library is recommended to be used alongside **VContainer**. [Official VContainer documentation](https://github.com/hadashiA/VContainer).

First, define the states and a fsm.
```csharp
public class MyEntryState : IState<MyFsm>
{
    public void OnEnter()
    {
        Debug.Log("MyEntryState OnEnter");
    }

    public NextState<MyFsm> OnTick()
    {
        return NextState<MyFsm>.TransitionTo<MyExitState>();
    }

    public void OnExit()
    {
        Debug.Log("MyEntryState OnExit");
    }
}

public class MyExitState : IState<MyFsm>
{
    public void OnEnter()
    {
        Debug.Log("MyExitState OnEnter");
    }

    public NextState<MyFsm> OnTick()
    {
        return NextState<MyFsm>.End();
    }

    public void OnExit()
    {
        Debug.Log("MyExitState OnExit");
    }
}

public class MyFsm : Fsm<MyFsm>
{
    public MyFsm(IEnumerable<IState<MyFsm>> states, Type startStateType) : base(states, startStateType)
    {
    }
}
```

Next, registering to LifetimeScope of VContainer.

``` csharp
public class MyLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterFsm<MyFsm>(fsmBuilder =>
        {
            fsmBuilder.RegisterStartState<MyEntryState>();
            fsmBuilder.RegisterState<MyExitState>();
        });
    }
}
```

Run fsm.

``` csharp
public class MyEntryPoint : ITickable
{
    private readonly MyFsm _fsm;
    public MyEntryPoint(MyFsm fsm)
    {
        _fsm = fsm;
    }

    public void Tick()
    {
        _fsm.Tick();
    }
}
```

## License
This library is released under the MIT License.
