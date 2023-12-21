namespace KiboWebhookListener.Helpers;

public static class EnvHelper
{
	public static void LoadEnv()
	{
		if (!File.Exists(".env"))
		{
			Console.WriteLine("No .env file found");
			return;
		}

		foreach (var line in File.ReadAllLines(".env"))
		{
			if (line.StartsWith("#")) continue; // Skip comment lines
			var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length != 2) continue; // Skip malformed lines
			Environment.SetEnvironmentVariable(parts[0], parts[1]);
		}
	}
}
