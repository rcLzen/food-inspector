namespace FoodInspector.Services;

public interface ISecureStorageService
{
    Task<string> GetEncryptionKeyAsync();
    Task SetEncryptionKeyAsync(string key);
}

public class SecureStorageService : ISecureStorageService
{
    private const string EncryptionKeyName = "DbEncryptionKey";

    public async Task<string> GetEncryptionKeyAsync()
    {
        try
        {
            var key = await SecureStorage.GetAsync(EncryptionKeyName);
            if (string.IsNullOrEmpty(key))
            {
                key = GenerateRandomKey();
                await SetEncryptionKeyAsync(key);
            }
            return key;
        }
        catch (PlatformNotSupportedException ex)
        {
            // Platform doesn't support secure storage, generate and store in-memory key
            System.Diagnostics.Debug.WriteLine($"SecureStorage not supported: {ex.Message}");
            return GenerateRandomKey();
        }
        catch (Exception ex)
        {
            // If secure storage fails for other reasons, generate a key
            System.Diagnostics.Debug.WriteLine($"SecureStorage error: {ex.Message}");
            var key = GenerateRandomKey();
            await SetEncryptionKeyAsync(key);
            return key;
        }
    }

    public async Task SetEncryptionKeyAsync(string key)
    {
        await SecureStorage.SetAsync(EncryptionKeyName, key);
    }

    private string GenerateRandomKey()
    {
        var bytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
