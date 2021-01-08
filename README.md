[![codecov](https://codecov.io/gh/oxffaa/lfe/branch/master/graph/badge.svg)](https://codecov.io/gh/oxffaa/lfe)
![Build and test](https://github.com/oxffaa/lfe/workflows/Regular%20master%20build%20and%20publish/badge.svg)

# Oxffaa.LFE

LFE(lock-free extensions) is a library that contains two thread-safe and same time lock-free collections for patterns "multiple producers one consumer" and handling some data parallel.

## InputBox or multiple producers to one consumer

Initialize the box and share reference between producers and the consumer:
```c#
var box = new InputBox<int>();
```

A producer can looks like:
```c#
var val = ReadValue();
while (!box.TryAdd(val)){}
```

A consumer can looks like:
```c#
var values = box.TakeAll()
HandleValues(values);
```

## OutputBox or handling some data parallel

Initialize the box with data for handling:
```c#
var box = new OutputBox(someData);
```

Spawn several workers like that:
```c#
while (box.HasItems)
{
    if (box.TryTake(out var item))
        HandleItem(item);
}
```
