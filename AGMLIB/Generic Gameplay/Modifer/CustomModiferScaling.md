# Custom Modifier Scaling

`CustomModiferScaling` changes how repeated modifiers lose strength when several modifiers affect the same stat.

The strongest modifier is counted as stack `1`, the next strongest as stack `2`, and so on. Each stack gets multiplied by a scale value.

```text
final contribution = original modifier value * scale
```

`Minimum Scale` and `Maximum Scale` clamp the final scale after the selected formula is calculated.

For example, if `Minimum Scale` is `0.25`, no stack can contribute less than 25% of its original value.

## Shared Values

| Value | Meaning |
|---|---|
| Priority | If multiple scaling rules apply, the highest priority rule wins. |
| Scale Modifiers | Applies the formula to percentage modifiers. |
| Scale Literals | Applies the formula to flat/literal modifiers. |
| Only When Stat Has Stacking Penalty | Only applies custom scaling to stats that normally use Nebulous stacking penalties. |
| Minimum Scale | Lowest allowed scale after the formula. |
| Maximum Scale | Highest allowed scale after the formula. |

## Vanilla Penalty

Uses the same style of curve as Nebulous' built-in stacking penalty.

Nebulous uses `Penalty Factor = 3.5` by default unless a specific stat overrides it.

The first stack is full strength. The penalty starts on the second stack.

```text
scale = e ^ -(((stack - 1) / penalty factor) ^ 2)
```

`Penalty Factor` controls how harsh the falloff is. Higher values make later stacks stronger.

### Penalty Factor 1

| Stack | Scale |
|---:|---:|
| 1 | 1.0000 |
| 2 | 0.3679 |
| 3 | 0.0183 |
| 4 | 0.0001 |
| 5 | ~0.0000 |

### Penalty Factor 2

| Stack | Scale |
|---:|---:|
| 1 | 1.0000 |
| 2 | 0.7788 |
| 3 | 0.3679 |
| 4 | 0.1054 |
| 5 | 0.0183 |

### Penalty Factor 3.5, Nebulous Default

| Stack | Scale |
|---:|---:|
| 1 | 1.0000 |
| 2 | 0.9216 |
| 3 | 0.7214 |
| 4 | 0.4797 |
| 5 | 0.2709 |
| 6 | 0.1299 |
| 7 | 0.0529 |
| 8 | 0.0183 |
## No Penalty

Every stack applies at full strength.

```text
scale = 1
```

| Stack | Scale |
|---:|---:|
| 1 | 1.00 |
| 2 | 1.00 |
| 3 | 1.00 |
| 4 | 1.00 |
| 5 | 1.00 |

Use this when repeated modifiers should stack normally with no diminishing returns.

## Linear

Each later stack loses a fixed amount.

```text
scale = 1 - ((stack - 1) * linear step)
```

With `Linear Step = 0.25`:

| Stack | Scale |
|---:|---:|
| 1 | 1.00 |
| 2 | 0.75 |
| 3 | 0.50 |
| 4 | 0.25 |
| 5 | 0.00 |

Use this when you want predictable, simple falloff.

## Exponential

Each later stack is multiplied by the same ratio.

```text
scale = exponential factor ^ (stack - 1)
```

With `Exponential Factor = 0.5`:

| Stack | Scale |
|---:|---:|
| 1 | 1.000 |
| 2 | 0.500 |
| 3 | 0.250 |
| 4 | 0.125 |
| 5 | 0.063 |

With `Exponential Factor = 0.75`:

| Stack | Scale |
|---:|---:|
| 1 | 1.000 |
| 2 | 0.750 |
| 3 | 0.563 |
| 4 | 0.422 |
| 5 | 0.316 |

Use this for smooth diminishing returns where every extra stack still matters.

## Power

Later stacks are divided by the stack number raised to a power.

```text
scale = 1 / (stack ^ power)
```

With `Power = 2`:

| Stack | Scale |
|---:|---:|
| 1 | 1.000 |
| 2 | 0.250 |
| 3 | 0.111 |
| 4 | 0.063 |
| 5 | 0.040 |

With `Power = 1`:

| Stack | Scale |
|---:|---:|
| 1 | 1.000 |
| 2 | 0.500 |
| 3 | 0.333 |
| 4 | 0.250 |
| 5 | 0.200 |

Use this when the first few stacks should matter most, while later stacks still have a small tail.
