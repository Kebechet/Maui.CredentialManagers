using Android.Runtime;
using AndroidX.Credentials;
using Maui.CredentialManagers.Platforms.Android.Callbacks.Base;

namespace Maui.CredentialManagers.Platforms.Android.Callbacks;

internal class CredentialManagerVoidCallback : VoidCallbackBase, ICredentialManagerCallback
{
    public CredentialManagerVoidCallback(CancellationToken cancellationToken) : base(cancellationToken)
    {
    }

    public void OnResult(Java.Lang.Object? result)
    {
        ReportSuccess();
    }

    public void OnError(Java.Lang.Object e)
    {
        var exception = (Exception)e.JavaCast<Java.Lang.Exception>();
        ReportException(exception);
    }
}