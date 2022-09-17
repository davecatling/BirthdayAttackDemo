namespace BirthdayAttackDemo
{
    internal class Program
    {
        private const string REAL_PATH = @"C:\Confession\confession_real.txt";
        private const string FAKE_PATH = @"C:\Confession\confession_fake.txt";

        static void Main(string[] args)
        {
            var birthdayAttacker = new BirthdayAttacker(REAL_PATH,FAKE_PATH,2);
            Console.WriteLine($"Real SHA256: {birthdayAttacker.RealHash}");
            var attempts = 0;
            while (birthdayAttacker.HashEndCharsInCommon < 4)
            {
                birthdayAttacker.MakeModifiedFile();
                attempts++;
                Console.WriteLine($"Real SHA256: { birthdayAttacker.RealHash}");
                Console.WriteLine($"Fake SHA256: {birthdayAttacker.FakeHash}");
                Console.WriteLine($"In common: {birthdayAttacker.HashEndCharsInCommon} Attempts: {attempts}");
                Console.WriteLine("----------");
            }
            Console.ReadKey();
        }
    }
}