using Maui.CredentialManagers.Models;
using Maui.CredentialManagers.Models.Options;
using Maui.CredentialManagers.Platforms.Android.Services;

namespace Maui.CredentialManagers.Services;

public partial class CredentialManagerService
{
    private readonly CredentialManagerAndroidService _credentialManagerAndroidService;
    private string _googleServerClientId;

    public CredentialManagerService(CredentialManagerAndroidService credentialManagerAndroidService, string googleServerClientId = "")
    {
        _credentialManagerAndroidService = credentialManagerAndroidService;
        _googleServerClientId = googleServerClientId;
    }

    public partial Task<CredentialManagerResultDto<bool>> CreatePasswordCredential(PasswordCredentialDto passwordCredential, CancellationToken cancellationToken);
    public partial Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(CancellationToken cancellationToken);
    public partial Task<CredentialManagerResultDto<CredentialDto>> GetPasswordCredential(GetPasswordCredentialOptionsDto getPasswordCredentialOptionsDto, CancellationToken cancellationToken);
    public partial Task<CredentialManagerResultDto<CredentialDto>> ContinueWithSso(CancellationToken cancellationToken);
}
