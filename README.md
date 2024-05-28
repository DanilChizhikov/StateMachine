# StateMachine
![](https://img.shields.io/badge/unity-2022.3+-000.svg)

## Description
This package shows a possible state machine implementation for Unity and C# projects.

## Table of Contents
- [Getting Started](#Getting-Started)
    - [Install manually (using .unitypackage)](#Install-manually-(using-.unitypackage))
    - [Install via UPM (using Git URL)](#Install-via-UPM-(using-Git-URL))
- [Basic Usage](#Basic-Usage)
    - [Example](#Example)
    - [Runtime Code](#Runtime-Code)
- [License](#License)

## Getting Started
Prerequisites:
- [GIT](https://git-scm.com/downloads)
- [Unity](https://unity.com/releases/editor/archive) 2022.3+
- [Extensions](https://github.com/DanilChizhikov/Extensions.git) 0.0.7+

### Install manually (using .unitypackage)
1. Download the .unitypackage from [releases](https://github.com/DanilChizhikov/StateMachine/releases/) page.
2. Open StateMachine.x.x.x.unitypackage

### Install via UPM (using Git URL)
1. Navigate to your project's Packages folder and open the manifest.json file.
2. Add this line below the "dependencies": { line
    - ```json title="Packages/manifest.json"
      "com.danilchizhikov.statemachine": "https://github.com/DanilChizhikov/statemachine.git?path=Assets/StateMachine",
      ```
UPM should now install the package.

## Example

- Custom example state
```csharp
public interface IExampleState : IState { }
```

- Custom example state machine
```csharp
public interface IExampleStateMachine : IStateMachine<ExampleState> { }

public sealed class ExampleStateMachine : StateMachine<ExampleState>, IExampleStateMachine
{
    public ExampleStateMachine(IEnumerable<ExampleState> states) : base(states) { }
}
```

## Basic Usage

### Runtime Code
First, you need to initialize the StateMachine<TState>, this can be done using different methods.
Here we will show the easiest way, which is not the method that we recommend using!
```csharp
public class StateMachineBootstrap : MonoBehaviour
{
    private static IExampleStateMachine _stateMachine;

    private bool _isInit = false;

    public static IExampleStateMachine StateMachine => _stateMachine;

    private async void Awake()
    {
        if (_stateMachine != null)
        {
            Destroy(gameObject);
            return;
        }

        ISomeState state = new SomeState();
        _stateMachine = new ExampleStateMachine(new ExampleState[]{ state });
        await _stateMachine.EnterAsync<ISomeState>();
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
public class ExampleState : State, IExampleState, IExitableState
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
    private readonly IExampleStateMachine _stateMachine;
    
    public Example(IExampleStateMachine stateMachine)
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

    private void StateChangedCallback(ExampleState previous, ExampleState current)
    {
        // some code
    }
}
```

## License

MIT