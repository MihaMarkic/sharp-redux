# Sharp-redux

## State

State can be composed of following object types in the following order

1. Class that implements IDictionary is treated as Dictionary
2. Class that implements IEnumerable is treated as an array
3. Type that isn't a primitive
4. Primitive type

Cyclic graphs aren't supported by visualizer but should work with redux core. Also features like save state might now work with cyclic graphs - depends how one stores them.