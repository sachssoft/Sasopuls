# Command Dispatcher Layer – Examples (Pseudo Code Only)

> ⚠️ This document contains ONLY simplified examples. No production code.

---

## CommandDispatcher (concept example)

```csharp
// idea: central event broadcaster
class CommandDispatcher
{
    event Action Update;

    void Notify()
    {
        // triggers all commands to re-evaluate
        Update?.Invoke();
    }
}
```

---

## ICommandRule + Dispatcher idea

```csharp
// rule decides if command can run
interface ICommandRule
{
    bool CanExecute();
    event Action Changed;
}

// optional: rule informs dispatcher
class RuleExample
{
    void StateChanged()
    {
        // would notify dispatcher
    }
}
```

---

## Command reacting to dispatcher (concept)

```csharp
class CommandExample
{
    CommandDispatcher dispatcher;

    CommandExample(CommandDispatcher d)
    {
        dispatcher = d;

        // subscribe to global update signal
        dispatcher.Update += Refresh;
    }

    void Refresh()
    {
        // re-evaluate CanExecute
    }
}
```

---

## Rule triggering update flow

```csharp
// when something changes in UI or model
ruleChanged -> dispatcher.Notify()
```

---

## MainViewModel wiring (concept only)

```csharp
// viewmodel owns everything
vm
 ├─ dispatcher
 ├─ rules
 └─ commands
```

---

## Combine / Any idea

```csharp
// AND logic
ruleA && ruleB

// OR logic
ruleA || ruleB
```

---

## Execution flow (concept)

```text
UI change
  ↓
Rule state change
  ↓
Dispatcher.Notify()
  ↓
All commands refresh CanExecute
```

---

## Key idea

* Rules decide state
* Dispatcher broadcasts change
* Commands only listen

```
```
