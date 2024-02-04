namespace Maui.CredentialManagers.Platforms.Android.Callbacks.Base;

internal abstract class VoidCallbackBase : Java.Lang.Object
{
    private readonly TaskCompletionSource _taskCompletionSource;

    public VoidCallbackBase(CancellationToken cancellationToken)
    {
        _taskCompletionSource = new TaskCompletionSource();
        cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());
    }

    public Task Task => _taskCompletionSource.Task;

    protected void ReportSuccess()
    {
        _taskCompletionSource.TrySetResult();
    }

    protected void ReportException(Exception exception)
    {
        _taskCompletionSource.TrySetException(exception);
    }
}