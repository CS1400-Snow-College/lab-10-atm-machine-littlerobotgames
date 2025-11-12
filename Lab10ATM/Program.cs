using System.ComponentModel.Design;

class Program
{
    public static List<User> users = new List<User>();
    public static void Main(string[] args)
    {
        users = LoadUsers();

        User loggedUser = null;

        int tries = 3;
        List<string> transactions = new List<string>();

        while(tries-- > 0)
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("PIN: ");
            int pin = Convert.ToInt32(Console.ReadLine());

            loggedUser = FindUser(username, pin);

            if (loggedUser != null)
            {
                tries = 0;
            }
        }

        if (loggedUser !=  null)
        {
            bool exit = false;
            do
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("    (1) Check Balance");
                Console.WriteLine("    (2) Withdraw");
                Console.WriteLine("    (3) Deposit");
                Console.WriteLine("    (4) Display last 5 transactions");
                Console.WriteLine("    (5) Quick Withdraw $40");
                Console.WriteLine("    (6) Quick Withdraw $100");
                Console.WriteLine("    (7) End Session");

                Console.Write("Enter number of option: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                switch(choice)
                {
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{loggedUser.Balance:C}");

                        transactions.Add("Checked Balance");
                        break;
                    case 2:
                        Console.Write("How much to withdraw? ");
                        double amount = Convert.ToDouble(Console.ReadLine());

                        if (amount < 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Must enter a positive amount");
                            break;
                        }

                        bool result = loggedUser.Withdraw(amount);

                        if (!result)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Not enough in balance to withdraw");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Withdraw successful");
                            transactions.Add($"Withdrew {amount:C}");
                        }
                        break;
                    case 3:
                        Console.Write("How much to deposit? ");
                        amount = Convert.ToDouble(Console.ReadLine());

                        if (amount < 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Must enter a positive amount");
                            break;
                        }

                        loggedUser.Deposit(amount);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Deposit Successful");
                        transactions.Add($"Deposited {amount:C}");
                        break;
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Transaction History");

                        if (transactions.Count == 0)
                        {
                            Console.WriteLine("    No history to display");
                        }
                        int num = 0;
                        for (int i = transactions.Count - 1; i >= 0; i--)
                        {
                            Console.WriteLine("    " + transactions[i]);
                            num++;

                            if (num == 5)
                            {
                                break;
                            }
                        }
                        break;
                    case 5:
                        result = loggedUser.Withdraw(40);

                        if (!result)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Not enough in balance to withdraw");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Withdraw successful");
                            transactions.Add("Withdrew $40.00");
                        }
                        break;
                    case 6:
                        result = loggedUser.Withdraw(100);

                        if (!result)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Not enough in balance to withdraw");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Withdraw successful");
                            transactions.Add("Quick Withdrew $100.00");
                        }
                        break;
                    case 7:
                        SaveUsers();
                        exit = true;
                        break;
                    default:
                        break;
                }
            }
            while (!exit);
        }
        else
        {
            Console.WriteLine("Too many login attempts, try again later");
        }
    }
    public class User
    {
        public string Username;
        public int Pin;
        public double Balance;

        public User(string username, int pin, double balance)
        {
            Username = username;
            Pin = pin;
            Balance = balance;
        }
        public bool TryLogin(string username, int pin)
        {
            if (username == Username && pin == Pin)
            {
                return true;
            }

            return false;
        }
        public bool Withdraw(double amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                return true;
            }
            
            return false;
        }
        public void Deposit(double amount)
        {
            Balance += amount;
        }
    }

    public static List<User> LoadUsers()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

        string[] database = File.ReadAllLines(projectDirectory + "/bank.txt");
        List<User> users = new List<User>();

        for (int i = 0; i < database.Length; i++)
        {
            string[] userData = database[i].Split(',');

            string userName = userData[0];
            int pin = Convert.ToInt32(userData[1]);
            double balance = Convert.ToDouble(userData[2]);

            users.Add(new User(userName, pin, balance));
        }

        return users;
    }
    public static User FindUser(string username, int pin)
    {
        foreach(User user in users)
        {
            if (user.TryLogin(username, pin))
            {
                return user;
            }
        }

        return null;
    }

    public static void SaveUsers()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

        string[] dataLines = new string[users.Count];

        for (int i = 0; i < dataLines.Length; i++)
        {
            User tempUser = users[i];
            dataLines[i] = $"{tempUser.Username},{tempUser.Pin},{tempUser.Balance}";
        }

        File.WriteAllLines(projectDirectory + "/bank.txt", dataLines);
    }
}