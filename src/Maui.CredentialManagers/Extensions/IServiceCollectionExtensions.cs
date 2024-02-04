using Maui.CredentialManagers.Platforms.Android.Services;
using Maui.CredentialManagers.Services;

namespace Maui.CredentialManagers.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCredentialManagerService(this IServiceCollection services, string googleServerClientId = "")
    {
//#if __ANDROID__
        services.AddScoped<CredentialManagerAndroidService>();
        services.AddScoped<CredentialManagerService>(provider =>
        {
            var dependency = provider.GetRequiredService<CredentialManagerAndroidService>();
            return new CredentialManagerService(dependency, googleServerClientId);
        });
//#endif

        return services;
    }
}
