using Google.Cloud.SecretManager.V1;

namespace StudentsManager.Infra;

public static class SecretAccessor
{
    public static async Task<string> GetSecretAsync(string secretName, string projectId)
    {
        var client = await SecretManagerServiceClient.CreateAsync();
        var secretVersionName = $"projects/{projectId}/secrets/{secretName}/versions/latest";
        var result = await client.AccessSecretVersionAsync(secretVersionName);
        return result.Payload.Data.ToStringUtf8();
    }
}