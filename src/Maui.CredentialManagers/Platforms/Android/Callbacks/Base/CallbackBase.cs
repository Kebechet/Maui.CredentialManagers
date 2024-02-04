namespace Maui.CredentialManagers.Platforms.Android.Callbacks.Base;

internal abstract class CallbackBase<TResult> : Java.Lang.Object
{
    private readonly TaskCompletionSource<TResult?> _taskCompletionSource;

    public CallbackBase(CancellationToken cancellationToken)
    {
        _taskCompletionSource = new TaskCompletionSource<TResult?>();
        cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());
    }

    public Task<TResult?> Task => _taskCompletionSource.Task;

    protected void ReportSuccess(TResult? result)
    {
        _taskCompletionSource.TrySetResult(result);
    }

    protected void ReportException(Exception exception)
    {
        _taskCompletionSource.TrySetException(exception);
    }
}