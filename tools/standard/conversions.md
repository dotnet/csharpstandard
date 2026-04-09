> <!-- Example: {template:"standalone-lib-without-using", name:"AnonymousFunctionsConv2"} -->
> <!-- Maintenance Note: A version of this type exists in additional-files as "R.cs". As such, certain changes to this type definition might need to be reflected in that file, in which case, *all* examples using that file should be tested. -->
> ```csharp
> delegate R Func<A,R>(A arg);
> ```
>
> In the assignments
>
> <!-- Example: {template:"standalone-console-without-using", name:"AnonymousFunctionsConv3", expectedErrors:["CS0266","CS1662"], additionalFiles:["R.cs"]} -->
> ```csharp
> Func<int,int> f1 = x => x + 1; // Ok
> Func<int,double> f2 = x => x + 1; // Ok
> Func<double,int> f3 = x => x + 1; // Error
> Func<int, Task<int>> f4 = async x => x + 1; // Ok
> ```
>
> the parameter and return types of each anonymous function are determined from the type of the variable to which the anonymous function is assigned.
