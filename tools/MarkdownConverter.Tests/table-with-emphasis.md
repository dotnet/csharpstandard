# 1 Header

The product is computed according to the rules of IEC 60559 arithmetic. The following table lists the results of all possible combinations of nonzero finite values, zeros, infinities, and NaNs. In the table, `x` and `y` are positive finite values. `z` is the result of `x * y`, rounded to the nearest representable value. If the magnitude of the result is too large for the destination type, `z` is infinity. Because of rounding, `z` may be zero even though neither `x` nor `y` is zero.

|           | `+y`  | `-y`  | `+0`  | `-0`  | `+∞`  | `-∞`  | `NaN` |
| :-------- | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| **`+x`**  | `+z`  | `-z`  | `+0`  | `-0`  | `+∞`  | `-∞`  | `NaN` |
| **`-x`**  | `-z`  | `+z`  | `-0`  | `+0`  | `-∞`  | `+∞`  | `NaN` |
| **`+0`**  | `+0`  | `-0`  | `+0`  | `-0`  | `NaN` | `NaN` | `NaN` |
| **`-0`**  | `-0`  | `+0`  | `-0`  | `+0`  | `NaN` | `NaN` | `NaN` |
| **`+∞`**  | `+∞`  | `-∞`  | `NaN` | `NaN` | `+∞`  | `-∞`  | `NaN` |
| **`-∞`**  | `-∞`  | `+∞`  | `NaN` | `NaN` | `-∞`  | `+∞`  | `NaN` |
| **`NaN`** | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` | `NaN` |
