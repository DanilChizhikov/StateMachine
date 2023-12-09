# StateMachine
![](https://img.shields.io/badge/unity-2022.3+-000.svg)

## Description
This package shows a possible state machine implementation for Unity and C# projects.

## Table of Contents
- [Getting Started](#Getting-Started)
    - [Install manually (using .unitypackage)](#Install-manually-(using-.unitypackage))
    - [Install via UPM (using Git URL)](#Install-via-UPM-(using-Git-URL))
- [Basic Usage](#Basic-Usage)
    - [Runtime Code](#Runtime-Code)
- [License](#License)

## Getting Started
Prerequisites:
- [GIT](https://git-scm.com/downloads)
- [Unity](https://unity.com/releases/editor/archive) 2022.3+

### Install manually (using .unitypackage)
1. Download the .unitypackage from [releases](https://github.com/DanilChizhikov/StateMachine/releases/) page.
2. Open StateMachine.x.x.x.unitypackage

### Install via UPM (using Git URL)
1. Navigate to your project's Packages folder and open the manifest.json file.
2. Add this line below the "dependencies": { line
    - ```json title="Packages/manifest.json"
      "com.danilchizhikov.statemachine": "https://github.com/DanilChizhikov/statemachine.git?path=Assets/StateMachine#0.0.2",
      ```
UPM should now install the package.

## Basic Usage

### Runtime Code
First, you need to initialize the StateMachine, this can be done using different methods.
Here we will show the easiest way, which is not the method that we recommend using!
```csharp
public class StateMachineBootstrap : MonoBehaviour
{
    private static IStateMachine _stateMachine;

    private bool _isInit = false;

    public static IStateMachine StateMachine => _stateMachine;

    private async void Awake()
    {
        if (_stateMachine != null)
        {
            Destroy(gameObject);
            return;
        }

        IState state = new ExampleState();
        _stateMachine = new StateMachine(state);
        await _stateMachine.EnterAsync<ExampleState>();
        _isInit = true;
    }

    private void OnDestroy()
    {
        if(_isInit)
        {
            _stateMachine.Dispose();
            _stateMachine = null;
        }
    }
}
```

```csharp
public class ExampleState : IExitableState
{
    public async Task EnterAsync(CancellationToken token)
    {
        // some code
    }

    public async Task ExitAsync(CancellationToken token)
    {
        // some code
    }
}
```

You can listen to the state change, this can be done as follows
```csharp
public class Example : IDisposable
{
    private readonly IStateMachine _stateMachine;
    
    public Example(IStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Initialize()
    {
        _stateMachine.OnStateChanged += StateChangedCallback;
    }
    
    public void Dispose()
    {
        _stateMachine.OnStateChanged -= StateChangedCallback;
    }

    private void StateChangedCallback(IState previous, IState current)
    {
        // some code
    }
}
```

The system supports states in which any value can be set, for this you need to use the following interface
```csharp
namespace MbsCore.StateMachine.Infrastructure
{
    public interface ISetupableState<in T>
    {
        void Setup(T value);
    }
}
```

After that, you can call a method from IStateMachine that accepts a generic argument
```csharp
Task EnterAsync<TState, T>(T value) where TState : IState;
```

## License

MIT