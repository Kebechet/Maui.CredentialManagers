using Android.Annotation;
using Android.App;
using Android.Content;
using AndroidX.Core.Content;
using AndroidX.Credentials;
using Maui.CredentialManagers.Platforms.Android.Callbacks;
using SatisFIT.Client.App.Platforms.Android.Services.Test;
using Application = Microsoft.Maui.Controls.Application;

namespace Maui.CredentialManagers.Platforms.Android.Services;

public class CredentialManagerAndroidService
{
    private readonly ICredentialManager _credentialManager;
    private Context _activityContext => Platform.CurrentActivity?.BaseContext ?? Platform.AppContext;

    public CredentialManagerAndroidService()
    {
        _credentialManager = CredentialManager.Create(Platform.AppContext);
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

        var context1 = Platform.AppContext;//1
        var context2 = Platform.CurrentActivity.BaseContext;//2
        var context3 = Platform.CurrentActivity?.ApplicationContext;//1
        var context4 = Application.Current?.Handler?.MauiContext?.Context;//1

        
        //var context5 = this.
        //Toolkit

        var act = context2;

        _credentialManager.CreateCredentialAsync(
            act,
            request,
            null,
            ContextCompat.GetMainExecutor(act),
            callback
        );

        return callback.Task;
    }
}