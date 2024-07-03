using console.ConfigEncryption;

Console.WriteLine("Config Values Encryption Program");

while (true)
{
    Console.Write("\nEnter the value to encrypt (or type 'exit' to quit): ");
    string input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine("No input provided. Please try again.");
        continue;
    }

    if (input.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
    {
        Console.WriteLine("Exiting the program. Goodbye!");
        break;
    }

    string encryptedValue = ConfigEncryption.Encrypt(input);
    Console.WriteLine($"Encrypted value: ENC:{encryptedValue}");
}