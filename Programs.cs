using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class Book
{
    public int id { get; set; }
    public string title { get; set; }
    public string author { get; set; }
    public decimal price { get; set; }
    public int stock { get; set; }
}

public class User
{
    public int id { get; set; }
    public string userName { get; set; }
    public string password { get; set; }
    public List<Order> orderHistory { get; set; } = new List<Order>();
}


public class Order
{
    public int orderId { get; set; }
    public int userId { get; set; }
    public List<Book> Books { get; set; }
    public DateTime orderDate { get; set; }
    public decimal totalAmout { get; set; }
}

public static class Database
{
    public static List<Book> Books = new List<Book>();
    public static List<User> Users = new List<User>();
}

public class Cart
{
    private List<Book> cartItems = new List<Book>();

    public void addToCart(Book book)
    {
        cartItems.Add(book);
    }

    public decimal calcuTotal()
    {
        return cartItems.Sum(book => book.price);
    }

    public List<Book> getCartItems()
    {
        return cartItems;
    }

    public void checkout(User user)
    {
        Order order = new Order
        {
            orderId = new Random().Next(1, 1000),
            userId = user.id,
            Books = new List<Book>(cartItems),
            orderDate = DateTime.Now,
            totalAmout = calcuTotal()

        };

        user.orderHistory.Add(order);
        cartItems.Clear();
        Console.WriteLine("Checkout successful! Your order has been placed.");
    }



   
}

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(@"  
  _____ _            _                _             
 |_   _| |          | |              | |            
   | | | |_ ___  ___| |__   ___   ___| |_ ___  _ __   
   | | | __/ _ \/ __| '_ \ / _ \ / __| __/ _ \| '__|  
  _| |_| ||  __/ (__| | | | (_) | (__| || (_) | |   
 |_____\__\___|\___|_| |_|\___/ \___|\__\___/|_|   

        ");
        Console.WriteLine(new string('*', 30));
        Console.WriteLine("*                      *");
        Console.WriteLine("*   Welcome To Fatiu Online Bookstore   *");
        Console.WriteLine("*                      *");
        Console.WriteLine(new string('*', 30));


        User loggedUser = null;
        Cart userCart = new Cart();

        bool start = true;
        while (start)
        {
            Console.WriteLine("\n1. Create Account\n2. Login\n3. Browse Books\n4. Add Book\n5. Add to Cart\n6. Checked Out\n7. Exist");
            Console.Write("Select an option: ");
            var choice = Console.ReadLine();// var can accept both int and string

            switch (choice)
            {
                case "1":
                    register();
                    break;
                case "2":
                    loggedUser = login();
                    break;
                case "3":
                    if (loggedUser != null)
                    {
                        browse(userCart); 
                    }
                    else
                    {
                        Console.WriteLine("Login before browsing books");
                    }
                    break;
                case "4":
                    addBook();
                    break;
                case "5":
                    if (loggedUser != null)
                    {
                        addToCart(userCart); 
                    }
                    else
                    {
                        Console.WriteLine("Login before adding books to cart");
                    }
                    break;
                case "6":
                    if(loggedUser != null)
                    {
                        checkout(userCart, loggedUser);
                    }
                    else
                    {
                        Console.WriteLine("Login to Check out");
                    }

                    break;
                case "7":
                    Console.WriteLine("Thanks for visiting");
                    start = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again");
                    break;
            }
        }

    }



    static void register()
    {
        Console.WriteLine("Sign Up.");
        Console.WriteLine("Enter Your Name: ");
        string name = Console.ReadLine();

        Console.WriteLine("Enter Your Password: ");
        string password = Console.ReadLine();

        // check is user name those not exist
        if (Database.Users.Any(user => user.userName == name))
        {
            Console.WriteLine("Username alredy exit.Try diffrent Name");
            return;
        }

        // add new user to our database bcoz is our databse that saving the user
        // now i create a anonymouse class for the user object
        User newUser = new User
        {
            id = Database.Users.Count + 1,
            userName = name,
            password = password
        };
        Database.Users.Add(newUser);
        Console.WriteLine("Register Successful");
    }


    public static User login()
    {
        Console.WriteLine("Login");
        Console.WriteLine("Enter Username: ");
        string name = Console.ReadLine();

        Console.WriteLine("Enter Password: ");
        string password = Console.ReadLine();

        User user = Database.Users.FirstOrDefault(u => u.userName == name && u.password == password);

        if (user != null)
        {
            Console.WriteLine("Login Successful");
            return user;
        }
        else
        {
            Console.WriteLine("User Dn not Exist. Try again");
            return null;
        }

    }

    public static void browse(Cart userCart)
    {
        if (Database.Books.Count == 0)
        {
            Console.WriteLine("No books are currently available.");
            return;
        }

        Console.WriteLine("\nAvailable Books: ");
        foreach (var book in Database.Books)
        {
            Console.WriteLine($"ID: {book.id}, Title: {book.title}, Author: {book.author}, Price: {book.price:C}, Stock: {book.stock}");

        }
    }


    public static void addBook()
    {
        Console.WriteLine("Add New Book");

        Console.WriteLine("Enter the book title: ");
        string title = Console.ReadLine();

        Console.WriteLine("Enter the author name: ");
        string author = Console.ReadLine();

        Console.WriteLine("Enter the price: ");
        decimal price;

        while (!decimal.TryParse(Console.ReadLine(), out price) || price < 0)
        {
            Console.WriteLine("Plese enter a valid price.");
        }

        Console.WriteLine("Enter Stock quantity: ");
        int stock;
        while (!int.TryParse(Console.ReadLine(), out stock) || stock < 0)
        {
            Console.WriteLine("Please enter a valid non-negative quantity.");
        }

        Book newBook = new Book
        {
            id = Database.Books.Count + 1,
            title = title,
            author = author,
            price = price,
            stock = stock

        };

        Database.Books.Add(newBook);
        Console.WriteLine("Book added Successfully!");
    }

    public static void addToCart(Cart userCart)
    {
        Console.WriteLine("Enter Book Id: ");
        int bookId;
        if (!int.TryParse(Console.ReadLine(), out bookId) || bookId <= 0)
        {
            Console.WriteLine("Please enter a valid ID");
        }


        var book = Database.Books.FirstOrDefault(b => b.id == bookId);
        if(book != null && book.stock > 0) {
            userCart.addToCart(book);
            book.stock--;
            Console.WriteLine($"{book.title} has been added to your cart.");

        }

        else
        {
            Console.WriteLine("Book not found");
        }
    }


    public static void checkout(Cart userCart, User user)
    {
        if (userCart.getCartItems().Count == 0)
        {
            Console.WriteLine("Your cart is empty.");
            return;
        } 

        userCart.checkout(user);
    }

}
