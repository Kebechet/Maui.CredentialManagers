using Android.Annotation;
using Android.App;
using Android.Content;
using AndroidX.Core.Content;
using AndroidX.Credentials;
using Maui.CredentialManagers.Platforms.Android.Callbacks;
using SatisFIT.Client.App.Platforms.Android.Services.Test;

namespace Maui.CredentialManagers.Platforms.Android.Services;

public class CredentialManagerAndroidService
{
    private readonly ICredentialManager _credentialManager;
    private Context _activityContext => Platform.CurrentActivity ??
        throw new Exception("Current activity is null");

    public CredentialManagerAndroidService()
    {
        _credentialManager = CredentialManager.Create(_activityContext);
    }

    public async Task<CreatePublicKeyCredentialResponse?> CreatePublicKeyCredential(CreatePublicKeyCredentialRequest request, CancellationToken cancellationToken)
    {
        var response = await CreateCredential(request, cancellationToken);
        return (CreatePublicKeyCredentialResponse?)response;
    }
    public async Task<CreateCustomCredentialResponse?> CreateCustomCredential(CreateCustomCredentialRequest request, CancellationToken cancellationToken)
    {
        var response = await CreateCredential(request, cancellationToken);
        return (CreateCustomCredentialResponse?)response;
    }
    public async Task<CreatePasswordResponse?> CreatePassword(CreatePasswordRequest request, CancellationToken cancellationToken)
    {
        var response = await CreateCredential(request, cancellationToken);
        return (CreatePasswordResponse?)response;
    }
    [TargetApi(Value = 34)]
    public PendingIntent CreateSettingsPendingIntent()
    {
        return _credentialManager.CreateSettingsPendingIntent();
    }

    public async Task<GetCredentialResponse?> GetCredential(GetCredentialRequest request, CancellationToken cancellationToken)
    {
        var callback = new CredentialManagerCallback<GetCredentialResponse>(cancellationToken);

        _credentialManager.GetCredentialAsync(
            _activityContext,
            request,
            null,
            ContextCompat.GetMainExecutor(_activityContext),
            callback
        );

        return await callback.Task;
    }

    [TargetApi(Value = 34)]
    public async Task<PrepareGetCredentialResponse?> PrepareGetCredential(GetCredentialRequest request, CancellationToken cancellationToken)
    {
        var callback = new CredentialManagerCallback<PrepareGetCredentialResponse>(cancellationToken);

        _credentialManager.PrepareGetCredentialAsync(
            request,
            null,
            ContextCompat.GetMainExecutor(_activityContext),
            callback
        );

        return await callback.Task;
    }

    //[Binder] Caught a RuntimeException from the binder stub implementation.
    //[Binder] java.lang.NullPointerException: Parameter specified as non-null is null: method androidx.credentials.CredentialProviderFrameworkImpl$onClearCredential$outcome$1.onResult, parameter response
    //[Binder] 	at androidx.credentials.CredentialProviderFrameworkImpl$onClearCredential$outcome$1.onResult(Unknown Source:3)
    //[Binder] 	at androidx.credentials.CredentialProviderFrameworkImpl$onClearCredential$outcome$1.onResult(CredentialProviderFrameworkImpl.kt:367)
    //[Binder] 	at android.credentials.CredentialManager$ClearCredentialStateTransport.onSuccess(CredentialManager.java:775)
    //[Binder] 	at android.credentials.IClearCredentialStateCallback$Stub.onTransact(IClearCredentialStateCallback.java:95)
    //[Binder] 	at android.os.Binder.execTransactInternal(Binder.java:1363)
    //[Binder] 	at android.os.Binder.execTransact(Binder.java:1304)
    //TODO - fix and make public
    private async Task ClearCredentialState()
    {
        var request = new ClearCredentialStateRequest();
        var tcs = new TaskCompletionSource();

        _credentialManager.ClearCredentialState(
            request,
            new Continuation(tcs, default)
        );

        await tcs.Task;
    }
    //crashes on exception:
    //[CredManProvService] In CredentialProviderFrameworkImpl onClearCredential
    //[Binder] Caught a RuntimeException from the binder stub implementation.
    //[Binder] java.lang.NullPointerException: Parameter specified as non-null is null: method androidx.credentials.CredentialProviderFrameworkImpl$onClearCredential$outcome$1.onResult, parameter response
    //[Binder] 	at androidx.credentials.CredentialProviderFrameworkImpl$onClearCredential$outcome$1.onResult(Unknown Source:3)
    //[Binder] 	at androidx.credentials.CredentialProviderFrameworkImpl$onClearCredential$outcome$1.onResult(CredentialProviderFrameworkImpl.kt:367)
    //[Binder] 	at android.credentials.CredentialManager$ClearCredentialStateTransport.onSuccess(CredentialManager.java:775)
    //[Binder] 	at android.credentials.IClearCredentialStateCallback$Stub.onTransact(IClearCredentialStateCallback.java:95)
    //[Binder] 	at android.os.Binder.execTransactInternal(Binder.java:1363)
    //[Binder] 	at android.os.Binder.execTransact(Binder.java:1304)
    //public async Task ClearCredentialState(CancellationToken cancellationToken)
    //{
    //    var request = new ClearCredentialStateRequest();
    //    var callback = new CredentialManagerVoidCallback(cancellationToken);

    //    _credentialManager.ClearCredentialStateAsync(
    //        request,
    //        null,
    //        ContextCompat.GetMainExecutor(_activityContext),
    //        callback
    //    );

    //    await callback.Task;
    //}

    private Task<CreateCredentialResponse?> CreateCredential(CreateCredentialRequest request, CancellationToken cancellationToken)
    {
        var callback = new CredentialManagerCallback<CreateCredentialResponse>(cancellationToken);

        _credentialManager.CreateCredentialAsync(
            _activityContext,
            request,
            null,
            ContextCompat.GetMainExecutor(_activityContext),
            callback
        );

        return callback.Task;
    }
}