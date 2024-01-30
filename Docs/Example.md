# Example :bulb:
### If you want to learn how to write your own mini-games, then you should study the structure of mini-games.
### The basic structure:

#### Use the ***Event*** interface to inherit all the important methods and variables for the mini-game to work.
#### Also inherit ***IEventMap*** to launch schematic maps and ***IEventSound*** to launch music (See the detailed description below).

#### Event Information:
Information about the event that users can see.
```csharp
public override string Name { get; set; } = "Example";
public override string Description { get; set; } = "An example event based on the battle event.";
public override string Author { get; set; } = "KoT0XleB";
public override string CommandName { get; set; } = "example";
```        

#### Event Settings:
Settings you have access to that will change functionality of the event.
```csharp
// How long to wait after the round finishes, before the cleanup begins. Default is 10 seconds.
public override float PostRoundDelay { get; protected set; } = 10f; 

// If using NwApi or Exiled as the base plugin, set this to false, and manually add your plugin to Event.Events (List[Events]).
// This prevents double-loading your plugin assembly.
public override bool AutoLoad { get; protected set; } = true;

// Used to safely kill the while loop, without have to forcible kill the coroutine.
public override bool KillLoop { get; protected set; } = false;

// How many seconds the event waits after each ProcessFrame().
protected override float FrameDelayInSeconds { get; set; } = 1f;
```


#### Event Variables:
Variables you can use that are automatically managed by the framework.
```csharp
// The coroutine handle of the main event thread which calls ProcessFrame().
protected override CoroutineHandle GameCoroutine { get; set; }

// The DateTime (UTC) that the plugin started at. 
public override DateTime StartTime { get; protected set; }
        
// The elapsed time since the plugin started.
public override TimeSpan EventTime { get; protected set; }
```

#### Event API Methods
```csharp
// Used to register events for plugins.
protected override void RegisterEvents() { }

// Used to unregister events for plugins.
protected override void UnregisterEvents() { }

// Called when the event is started.
protected override void OnStart();

// Called after start in a coroutine. Can be used as a countdown coroutine.
protected override IEnumerator<float> BroadcastStartCountdown()

// Called after BroadcastStartCountdown is finished. Can be used to remove walls, or give players items.
protected override void CountdownFinished()

// Used to determine whether the event should end or not. 
// Returns true if the round is finished. False if the round should continue running.
protected abstract bool IsRoundDone();

// It is called as many times per second as is set in the FrameDelayInSeconds.
protected override void ProcessFrame() { }

// Called when the event is finished. If the event is stopped via OnStop, this won't be called, as the event never truly finishes properly.
protected abstract void OnFinished();

// Called if the event is forcibly stopped. If this is called, OnFinished won't be called.
protected override void OnStop() { }

// The overridable class for after and event is finished / stopped and cleanup is occuring.
protected virtual void OnCleanup() { }
```

#### Event maps and music
```csharp
// Used to register events for plugins.
protected override void RegisterEvents() { }
```
### In Exiled and NWApi there is a difference in creating mini-games. If the entire toolkit has already been created in Exiled, and the plugin just makes it convenient to create mini-games, but NWApi had to create its own toolkit. You can use the implemented cancellable events in the code for yourself.
