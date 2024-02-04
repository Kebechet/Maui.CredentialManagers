using Android.Runtime;
using AndroidX.Credentials;
using Maui.CredentialManagers.Platforms.Android.Callbacks.Base;

namespace SatisFIT.Client.App.Platforms.Android.Services.Test;

internal sealed class CredentialManagerCallback<TResponse> : CallbackBase<TResponse>, ICredentialManagerCallback
    where TResponse : Java.Lang.Object
{
    public CredentialManagerCallback(CancellationToken cancellationToken) : base(cancellationToken)
    {
    }

    public void OnResult(Java.Lang.Object? result)
    {
        var passwordResponse = result is not null
            ? (TResponse)result
            : null;

        ReportSuccess(passwordResponse);
    }

    public void OnError(Java.Lang.Object e)
    {
        var exception = (Exception)e.JavaCast<Java.Lang.Exception>();
        ReportException(exception);
    }
}