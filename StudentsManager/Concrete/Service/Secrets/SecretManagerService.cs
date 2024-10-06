using Google.Cloud.SecretManager.V1;

namespace StudentsManager.Concrete.Service.Secrets;

public class SecretManagerService
{
    private readonly SecretManagerServiceClient _client = SecretManagerServiceClient.Create();

    public async Task<string> GetSecretAsync(string secretName, string projectId)
    {
        var secretVersion = $"projects/{projectId}/secrets/{secretName}/versions/latest";
        var result = await _client.AccessSecretVersionAsync(secretVersion);
        return result.Payload.Data.ToStringUtf8();
    }
}