using System.Runtime.CompilerServices;

class MyTaskMethodBuilder<T>
{
    public static MyTaskMethodBuilder<T> Create()
    {
        return new MyTaskMethodBuilder<T>();
    }

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
    {
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
    }

    public void SetException(Exception exception)
    {
    }

    public void SetResult(T result)
    {
    }

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
    }

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
    }

    public MyTask<T> Task { get; }
}
