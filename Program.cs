using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Book
{
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string Subject { get; private set; }
    public string Isbn { get; private set; }

    public Book(string title, string author, string subject, string isbn)
    {
        Title = title;
        Author = author;
        Subject = subject;
        Isbn = isbn;
    }
}

class Person
{
    public string SocialId { get; private set; }
    public string Password { get; set; }

    public Person(string socialId, string password)
    {
        SocialId = socialId;
        Password = password;
    }
}

class MainApp
{
    static List<Person> people = new List<Person>();
    static List<Book> books;

    static void SavePeopleToFile()
    {
        using (StreamWriter sw = new StreamWriter("people.txt"))
        {
            foreach (Person person in people)
            {
                sw.WriteLine($"{person.SocialId}:{person.Password}");
            }
        }
    }

    static void LoadPeopleFromFile()
    {
        people = new List<Person>();

        if (File.Exists("people.txt"))
        {
            using (StreamReader sr = new StreamReader("people.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        people.Add(new Person(parts[0], parts[1]));
                    }
                }
            }
        }
    }

    private static void InitializeLibrary()
    {
        books = new List<Book>()
    {
        new Book("Gone with the Wind", "Margaret Mitchell", "Historical Fiction", "9780446675536"),
        new Book("The Chronicles of Narnia", "C.S. Lewis", "Fantasy", "9780064471190"),
        new Book("The Picture of Dorian Gray", "Oscar Wilde", "Gothic", "9780141439570"),
        new Book("Moby-Dick", "Herman Melville", "Adventure", "9780142437247"),
        new Book("The Kite Runner", "Khaled Hosseini", "Historical Fiction", "9781594631931"),
        new Book("The Handmaid's Tale", "Margaret Atwood", "Dystopian", "9780385490818"),
        new Book("The Book Thief", "Markus Zusak", "Historical Fiction", "9780375842207"),
        new Book("Crime and Punishment", "Fyodor Dostoevsky", "Classic", "9780143107637")
    };

    }

    static Book SearchBook()
    {
        Console.WriteLine("\nEnter your search:");
        string searchTerm = Console.ReadLine().ToLower();

        if (books == null)
        {
            Console.WriteLine("The book list is empty, please initialize books first.");
            return null;
        }

        List<Book> results;

        try
        {
            results = books.Where(book => book.Title.ToLower().Contains(searchTerm) ||
                                           book.Author.ToLower().Contains(searchTerm) ||
                                           book.Subject.ToLower().Contains(searchTerm) ||
                                           book.Isbn.ToLower().Contains(searchTerm)).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred during the search: " + ex.Message);
            return null;
        }

        if (results.Count > 0)
        {
            Console.WriteLine("\nFound these results:");
            foreach (Book book in results)
            {
                Console.WriteLine("- " + book.Title + " by " + book.Author);
            }

            Console.WriteLine("\nWhich book do you want to borrow?");
            string chosenTitle = Console.ReadLine();

            Book chosenBook = results.Find(book => book.Title.Equals(chosenTitle, StringComparison.CurrentCultureIgnoreCase));
            if (chosenBook != null)
            {
                books.Remove(chosenBook);
                return chosenBook;
            }
            else
            {
                Console.WriteLine("This book is not available in the library.");
            }
        }
        else
        {
            Console.WriteLine("\nYour search did not produce any results.");
        }

        return null;
    }

    static void borrowBook(Book borrowedBook)
    {
        while (true)
        {
            Console.WriteLine("\nWhat do you want to do?");
            Console.WriteLine("1. Return the book");
            Console.WriteLine("2. Log out");

            Console.Write("Enter your choice (1-2): ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                books.Add(borrowedBook);
                Console.WriteLine("You have returned the book.");
                break;
            }
            else if (choice == "2")
            {
                Console.WriteLine("Logged out.");
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice. Try again");
            }
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome!");
        InitializeLibrary();
        LoadPeopleFromFile();

        while (true)
        {
            Console.WriteLine("\nWhat do you want to do?");
            Console.WriteLine("1. Create an account");
            Console.WriteLine("2. Log in");
            Console.WriteLine("3. Exit");

            Console.Write("Enter your choice (1-3): ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                CreateAccount();
            }
            else if (choice == "2")
            {
                Person loggedInPerson = LogIn();
                if (loggedInPerson != null)
                {
                    Console.WriteLine("Welcome to your account!");
                    LoggedInMenu(loggedInPerson);
                }
                else
                {
                    Console.WriteLine("Incorrect Social ID or password. Try again.");
                }
            }
            else if (choice == "3")
            {
                Console.WriteLine("Thank you for visiting! Goodbye.");
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice. Try again.");
            }
        }
    }

    static void LoggedInMenu(Person person)
    {
        while (true)
        {
            Console.WriteLine("\nWhat do you want to do?");
            Console.WriteLine("1. Search for a book");
            Console.WriteLine("2. Log out");
            Console.WriteLine("3. Change password");

            Console.Write("Enter your choice (1-3): ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Book borrowedBook = SearchBook();
                if (borrowedBook != null)
                {
                    Console.WriteLine("You have borrowed the book: " + borrowedBook.Title + " by " + borrowedBook.Author);
                    borrowBook(borrowedBook);
                }
            }
            else if (choice == "2")
            {
                Console.WriteLine("You have logged out.");
                break;
            }
            else if (choice == "3")
            {
                ChangePassword(person);
            }
            else
            {
                Console.WriteLine("Invalid choice. Try again.");
            }
        }
    }

    static void CreateAccount()
    {
        Console.WriteLine("\nTo create an account, please enter your Social ID and a password.");

        Console.Write("Social ID: ");
        string socialId = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        if (people.Any(p => p.SocialId == socialId))
        {
            Console.WriteLine("This Social ID is already registered. Try again with a different Social ID.");
        }
        else
        {
            people.Add(new Person(socialId, password));
            SavePeopleToFile();
            Console.WriteLine("Your account has been created. Thank you!");
        }
    }

    static Person LogIn()
    {
        Console.WriteLine("\nTo log in, please enter your Social ID and password.");

        Console.Write("Social ID: ");
        string socialId = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        var person = people.FirstOrDefault(p => p.SocialId == socialId && p.Password == password);

        if (person != null)
        {
            return person; // Return the person if login is successful
        }
        return null; // Return null if the login fails
    }

    static void ChangePassword(Person person)
    {
        Console.WriteLine("\nTo change your password, please enter your current password.");

        Console.Write("Current password: ");
        string currentPassword = Console.ReadLine();

        if (person.Password == currentPassword)
        {
            Console.Write("New password: ");
            string newPassword = Console.ReadLine();

            person.Password = newPassword;
            SavePeopleToFile(); // Save the updated people list to the file
            Console.WriteLine("Your password has been updated. Thank you!");
        }
        else
        {
            Console.WriteLine("Invalid password. Try again.");
        }
    }
}

