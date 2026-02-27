using ValiKop.Api.Security;

public static class DebugPasswordGenerator
{
    public static void Run()
    {
        var (hash, salt) = PasswordHasher.Hash("admin123");

        Console.WriteLine("HASH:");
        Console.WriteLine(hash);

        Console.WriteLine("SALT:");
        Console.WriteLine(salt);
    }
}
