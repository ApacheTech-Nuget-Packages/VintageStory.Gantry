namespace Gantry.Core;

internal class GantrySettings
{
    internal bool DebugMode { get; set; }

    public static bool CheckIfNumberIsValidUsPhoneNumber(string phoneNumber)
    {
        if (phoneNumber.Length != 10)
        {
            Console.WriteLine("Requires a number length of 10.");
            return false;
        }

        return phoneNumber.All(char.IsNumber);
    }
}