using AndroidX.Credentials;
using Maui.CredentialManagers.Models;
using Maui.CredentialManagers.Models.Options;
using Xamarin.GoogleAndroid.Libraries.Identity.GoogleId;

namespace Maui.CredentialManagers.Services;

//https://developer.android.com/training/sign-in/passkeys
//https://developer.android.com/reference/androidx/credentials/CredentialManager
public partial class CredentialManagerService
{
    public async partial Task<CredentialManagerResultDto<bool>> CreatePasswordCredential(PasswordCredentialDto passwordCredential, CancellationToken cancellationToken)
    {
        var createPasswordRequest = new CreatePasswordRequest(passwordCredential.Id, passwordCredential.Password);

        try
        {
            var res = await _credentialManagerAndroidService.CreatePassword(createPasswordRequest, cancellationToken);
            if (res is null)
            {
                return new CredentialManagerResultDto<bool>
                {
                    ErrorMessage = "No password credential found"
                };
            }

            return new CredentialManagerResultDto<bool>
            {
                Data = true
            };
        }
        catch (Exception e)
        {
            return new CredentialManagerResultDto<bool>
            {
                ErrorMessage = e.Message
            };
        }
    }

    public async partial Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(CancellationToken cancellationToken)
    {
        return await GetPasswordCredential(new GetPasswordCredentialOptionsDto(), cancellationToken);
    }

    public async partial Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(GetPasswordCredentialOptionsDto getPasswordCredentialOptionsDto, CancellationToken cancellationToken)
    {
        var passwordOption = new GetPasswordOption();

        var googleIdOption = new GetGoogleIdOption.Builder()
           .SetFilterByAuthorizedAccounts(getPasswordCredentialOptionsDto.OnlyAuthorizedAccounts)
           .SetServerClientId(_googleServerClientId)
           .SetNonce(Guid.NewGuid().ToString())
           .SetAutoSelectEnabled(getPasswordCredentialOptionsDto.IsCredentialAutoSelectEnabled)
           .SetRequestVerifiedPhoneNumber(getPasswordCredentialOptionsDto.RequestVerifiedPhoneNumber)
           .Build();

        var getCredentialRequest = new GetCredentialRequest.Builder()
            .AddCredentialOption(passwordOption)
            .AddCredentialOption(googleIdOption)
            .SetPreferIdentityDocUi(getPasswordCredentialOptionsDto.PreferIdentityDocUi)
            .SetPreferImmediatelyAvailableCredentials(getPasswordCredentialOptionsDto.PreferImmediatelyAvailableCredentials)
            .Build();

        return await ProcessGetCredentialRequest(getCredentialRequest, cancellationToken);
    }

    //https://developers.google.com/identity/android-credential-manager/android/reference/com/google/android/libraries/identity/googleid/GetSignInWithGoogleOption
    public async partial Task<CredentialManagerResultDto<CredentialDto>> ContinueWithSso(CancellationToken cancellationToken)
    {
        var signInWithGoogleOption = new GetSignInWithGoogleOption(_googleServerClientId, "", Guid.NewGuid().ToString());

        var getCredentialRequest = new GetCredentialRequest.Builder()
            .AddCredentialOption(signInWithGoogleOption)
            .Build();

        return await ProcessGetCredentialRequest(getCredentialRequest, cancellationToken);
    }

    private async Task<CredentialManagerResultDto<CredentialDto>> ProcessGetCredentialRequest(GetCredentialRequest getCredentialRequest, CancellationToken cancellationToken)
    {
        try
        {
            var res = await _credentialManagerAndroidService.GetCredential(getCredentialRequest, cancellationToken);
            if (res is null || res.Credential is null)
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    ErrorMessage = "No password credential found"
                };
            }

            if (res.Credential.GetType() == typeof(PublicKeyCredential))
            {
                var responseJson = ((PublicKeyCredential)res.Credential).AuthenticationResponseJson;

                return new CredentialManagerResultDto<CredentialDto>
                {
                    Data = new CredentialDto
                    {
                        PublicKeyCredential = new PublicKeyCredentialDto
                        {
                            AuthenticationResponseJson = responseJson
                        }
                    }
                };
            }
            else if (res.Credential.GetType() == typeof(PasswordCredential))
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    Data = new CredentialDto
                    {
                        PasswordCredential = new PasswordCredentialDto
                        {
                            Id = ((PasswordCredential)res.Credential).Id,
                            Password = ((PasswordCredential)res.Credential).Password
                        }
                    }
                };
            }
            else if (res.Credential.GetType() == typeof(CustomCredential))
            {
                try
                {
                    var googleIdTokenCredential = GoogleIdTokenCredential.CreateFrom(((CustomCredential)res.Credential).Data);

                    return new CredentialManagerResultDto<CredentialDto>
                    {
                        Data = new CredentialDto
                        {
                            GoogleIdTokenCredential = new GoogleIdTokenCredentialDto
                            {
                                Id = googleIdTokenCredential.Id,
                                IdToken = googleIdTokenCredential.IdToken,
                                DisplayName = googleIdTokenCredential.DisplayName,
                                FamilyName = googleIdTokenCredential.FamilyName,
                                GivenName = googleIdTokenCredential.GivenName,
                                ProfilePictureUri = googleIdTokenCredential.ProfilePictureUri?.ToString(),
                                PhoneNumber = googleIdTokenCredential.PhoneNumber
                            }
                        }
                    };
                }
                catch (GoogleIdTokenParsingException e)
                {
                    return new CredentialManagerResultDto<CredentialDto>
                    {
                        ErrorMessage = "Received an invalid Google ID token response"
                    };
                }
            }
            else
            {
                return new CredentialManagerResultDto<CredentialDto>
                {
                    ErrorMessage = "Unexpected type of credential"
                };
            }
        }
        catch (Exception e)
        {
            return new CredentialManagerResultDto<CredentialDto>
            {
                ErrorMessage = e.Message
            };
        }
    }
}
