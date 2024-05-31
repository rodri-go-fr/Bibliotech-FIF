using System.Globalization;
using System.IO;
using static Program;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Bienvenido, a continuacion ingresa tus credenciales ");
        while (true)
        {
            Console.Write("Usuario: ");
            string user = Console.ReadLine();
            Console.Write("Contraseña: ");
            string password = Console.ReadLine();

            if (login(user,password))
            {
                if (isAdmin(user))
                {
                    menuAdmin();
                }
                else
                {
                    menuUser(user);
                }
            }
            else
            {
                Console.WriteLine("Usuario o contraseña incorrectos");
            }

        }
    }

    public static bool isAdmin(string user)
    {
        return user == "admin";
    }

    public static void menuAdmin()
    {
        while (true)
        {
            Console.WriteLine("\nMenu Administrativo");
            Console.WriteLine("1. Alta de usuario");
            Console.WriteLine("2. Baja de usuario");
            Console.WriteLine("3. Agregar libro");
            Console.WriteLine("4. Agregar ejemplares");
            Console.WriteLine("5. Quitar ejemplares");
            Console.WriteLine("6. Quitar libro");
            Console.WriteLine("7. Consultar libros prestados");
            Console.WriteLine("8. Consultar status usuario");
            Console.WriteLine("9. Consultar status libros");
            Console.WriteLine("10. Cerrar Sesion");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Console.WriteLine("Ingresa el expediente del usuario, contraseña deseada y nombre completo (separados por ,)");
                    Admin.addUser(Console.ReadLine());
                    break;
                case "2":
                    Console.WriteLine("Ingresa el expediente del usuario a dar de baja");
                    Admin.removeUser(Console.ReadLine());
                    break;
                case "3":
                    Console.WriteLine("Ingresa el nombre, autor, año, genero y cantidad del libro a agregar (separados por ,)");
                    Admin.addBook(Console.ReadLine());
                    break;
                case "4":
                    Console.WriteLine("Ingresa el id del libro y la cantidad de ejemplares a agregar (separados por ,)");
                    string line = Console.ReadLine();
                    string [] exemplars = new string[2];
                    exemplars = line.Split(',');
                    Admin.addExemplar(int.Parse(exemplars[0]), int.Parse(exemplars[1]));
                    break;
                case "5":
                    Console.WriteLine("Ingresa el id del libro y la cantidad de ejemplares a quitar (separados por ,)");
                    string line5 = Console.ReadLine();
                    string[] exemplarsToRemove = new string[2];
                    exemplarsToRemove = line5.Split(',');
                    Admin.removeExemplar(int.Parse(exemplarsToRemove[0]), int.Parse(exemplarsToRemove[1]));
                    break;
                case "6":
                    Console.WriteLine("Ingresa el id del libro a quitar");
                    Admin.removeBook(Console.ReadLine());
                    break;
                case "7":
                    Admin.listAllBorrowedBooks();
                    break;
                case "8":
                    Admin.listAllUserswithOverdueBooks();
                    break;
                case "9":
                    Admin.listAllBookswithoutExemplars();
                    break;
                case "10":
                    return;
                default:
                    Console.WriteLine("Opción no válida");
                    break;
            }
        }
    }

    public static void menuUser(string user)
    {
        while (true)
        {
            Console.WriteLine("\nMenu de usuario");
            Console.WriteLine("1. Buscar libro");
            Console.WriteLine("2. Solicitar prestamo libro");
            Console.WriteLine("3. Verificar días faltantes");
            Console.WriteLine("4. Devolver libro");
            Console.WriteLine("5. Transferir préstamo");
            Console.WriteLine("6. Recomendar libro");
            Console.WriteLine("7. Salir al login");

            Console.WriteLine("Selecciona una opción: ");
            int option = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            switch (option)
            {
                case 1:
                    Console.WriteLine("Como deseas buscar el libro?");
                    User.searchBook();
                    break;
                case 2:
                    Console.WriteLine("Ingresa el id del libro a solicitar: ");
                    int idLibro = Convert.ToInt32(Console.ReadLine());
                    User.borrowBook(user, idLibro.ToString());
                    break;
                case 3:
                    User.remainingDays(user);
                    break;
                case 4:
                    User.returnBook(user);
                    break;
                case 5:
                    Console.WriteLine("Ingresa el id del libro a transferir: ");
                    User.listBorrowedBooks(user);
                    string bookID = Console.ReadLine();
                    Console.WriteLine("Ingresa el nombre del usuario al que se transferira: ");
                    string newUsername = Console.ReadLine();
                    User.transferBorrow(user, bookID, newUsername);
                    break;
                case 6:
                    Console.WriteLine("Que libro te gustaria ver en la biblioteca?");
                    User.sugestBook(Console.ReadLine());
                    break;
                case 7:
                    return;
                default:
                    Console.WriteLine("Opción no válida");
                    break;
            }
        }
    }

    public static bool login(string user, string password)
    {
        string [,] users = DataAccess.readFile(Config.pathUser, 6);
        for (int i = 0; i < users.GetLength(0); i++)
        {
            if (users[i, 0] == user && users[i, 1] == password)
            {
                return true;
            }
        }

        return false;
    }

    public static class Config
    {
        public static string pathUser = @"C:\Users\rodri\source\repos\Intro_ProgramacionCSharp\ProyectoFinal\usuario.txt";
        public static string pathBooks = @"C:\Users\rodri\source\repos\Intro_ProgramacionCSharp\ProyectoFinal\libros.txt";
        public static string pathLog = @"C:\Users\rodri\source\repos\Intro_ProgramacionCSharp\ProyectoFinal\log.txt";
        public static string pathRecommendations = @"C:\Users\rodri\source\repos\Intro_ProgramacionCSharp\ProyectoFinal\recommendations.txt";
    }

    public static class DataAccess
    {
        public static string[,] readFile(string path, int columns)
        {

            string[] lines = File.ReadAllLines(path);
            string[,] users = new string[lines.Length, columns];
            for (int i = 0; i < lines.Length; i++)
            {
                string [] data = lines[i].Split(',');
                for (int j = 0; j < data.Length; j++)
                {
                    users[i, j] = data[j];
                }
            }
            return users;
        }

        public static void writeAllFile(string[,] users, string path)
        {
            string[] lines = new string[users.GetLength(0)];

            for (int i = 0; i < users.GetLength(0); i++)
            {
                string [] lineParts = new string[users.GetLength(1)];
                for (int j = 0; j < users.GetLength(1); j++)
                {
                    lineParts[j] = users[i, j];
                }
                lines[i] = string.Join(",", lineParts);
            }
            File.WriteAllLines(path, lines);
        }

        public static void appendData(string data, string path)
        {
            // Leer todo el contenido del archivo para verificar el último carácter
            string allText = File.ReadAllText(path);

            // Añadir un solo salto de línea solo si no está vacío y no termina con un salto de línea
            using (StreamWriter sw = File.AppendText(path))
            {
                if (!string.IsNullOrEmpty(allText) && !allText.EndsWith(Environment.NewLine))
                {
                    sw.WriteLine();
                }
                sw.WriteLine(data.Trim());
            }
        }

        public static void logAction(string action)
        {
            DateTime date = DateTime.Now;
            File.AppendAllText(Config.pathLog, action + " " +date.ToString() + "\n");
        }

        public static string showBookInfo(string id)
        {
            string[,] books = readFile(Config.pathBooks, 9);
            for (int i = 0; i < books.GetLength(0); i++)
            {
                if (books[i, 0] == id)
                {
                    // Mostrar información del libro en una sola linea, solo informacion relevante para el usuario
                    //Console.WriteLine($"ID: {books[i, 0]} Título: {books[i, 1]} Autor: {books[i, 2]} Año: {books[i, 3]} Género: {books[i, 4]}");
                    return $"ID: {books[i, 0]} Título: {books[i, 1]} Autor: {books[i, 2]}";
                }

            }
            return "Libro no encontrado";
        }
        
    }

    public static class Admin
    {
        public static void addUser(string line)
        {
            string[] data = line.Split(',');
            string[,] users = DataAccess.readFile(Config.pathUser, 6);

            if(data.Length != 3)
            {
                Console.WriteLine("Datos incompletos");
                return;
            }

            // Comprobamos si el usuario ya existe
            for (int i = 0; i < users.GetLength(0); i++)
            {
                if (users[i, 0] == data[0])
                {
                    Console.WriteLine("El usuario ya existe");
                    return;
                }
            }

            // Si el usuario no existe, continuamos con la alta
            string user = data[0].Trim();
            string password = data[1].Trim();
            string name = data[2].Trim();
            string userLine = ($"{user},{password},{name},0,0,0");
            File.AppendAllText(Config.pathUser, userLine + "\n");
            Console.WriteLine("Usuario añadido con éxito");
            DataAccess.logAction($"Usuario añadido: {userLine}.");
        }

        public static void removeUser(string user)
        {
            string[,] users = DataAccess.readFile(Config.pathUser, 6);
            int userCount = users.GetLength(0);
            int columns = users.GetLength(1);

            bool userFound = false;

            List<string> overdueUsers = listAllUserswithOverdueBooks();

            foreach (string username in overdueUsers)
            {
                if (username == user)
                {
                    return;
                }
            }

            // Lista temporal para almacenar los nuevos usuarios
            string[,] newUsers = new string[userCount - 1, columns];
            int newUserIndex = 0;
            
            for (int i = 0; i < userCount; i++)
            {
                if (users[i, 0] == user)
                {
                    userFound = true;
                    continue; // Saltamos la copia de este usuario para removerlo
                }
                else if (newUserIndex < userCount - 1)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        newUsers[newUserIndex, j] = users[i, j];
                    }
                    newUserIndex++;
                }
            }

            if (userFound)
            {
                DataAccess.writeAllFile(newUsers, Config.pathUser);
                DataAccess.logAction($"Usuario removido: {user}.");
                Console.WriteLine("Usuario removido con éxito");
            }
            else
            {
                Console.WriteLine("El usuario no existe");
            }
        }

        public static void addBook(string book)
        {
            string[] data = book.Split(',');
            if(data.Length != 5)
            {
                Console.WriteLine("Datos incompletos");
                return;
            }
            string id = (DataAccess.readFile(Config.pathBooks, 9).GetLength(0) + 1).ToString();
            string title = data[0].Trim();
            string author = data[1].Trim();
            string year = data[2].Trim();
            string genre = data[3].Trim();
            string quantity = data[4].Trim();
            string bookline = ($"{id},{title},{author},{year},{genre},{quantity},,,");
            File.AppendAllText(Config.pathBooks, bookline + "\n");
            Console.WriteLine("Libro añadido con éxito");
            DataAccess.logAction($"Libro añadido: {book}.");
        }

        public static void removeBook(string id)
        {
            string[,] books = DataAccess.readFile(Config.pathBooks, 9);
            int bookCount = books.GetLength(0);
            int columns = books.GetLength(1);

            var newBooks = new string[bookCount - 1, columns];
            // Este índice nos ayudará a no perder la referencia de los nuevos libros
            int newBookIndex = 0;

            for (int i = 0; i < bookCount; i++)
            {
                if (books[i, 0] == id)
                {
                    // Si el libro está prestado (books[i, 7] no está vacío y no es "0"), no se puede eliminar
                    if (!string.IsNullOrEmpty(books[i, 7]) && books[i, 7] != "0")
                    {
                        Console.WriteLine("El libro no se puede eliminar porque está prestado");
                        return;
                    }
                    // Si el libro no está prestado, se salta la iteración para no copiar este libro a newBooks
                    continue;
                }

                // Copiar los libros que no coinciden con el ID al nuevo arreglo
                for (int j = 0; j < columns; j++)
                {
                    newBooks[newBookIndex, j] = books[i, j];
                }
                newBookIndex++;
            }
            Console.WriteLine("Libro eliminado con éxito");
            DataAccess.writeAllFile(newBooks, Config.pathBooks);
            DataAccess.logAction($"Libro removido: {id}.");
        }


        public static void addExemplar(int id, int quantity)
        {
            string[,] books =  DataAccess.readFile(Config.pathBooks,9);
            for (int i = 0; i < books.GetLength(0); i++)
            {
                if (books[i, 0] == id.ToString())
                {
                    books[i, 5] = (int.Parse(books[i, 5]) + quantity).ToString();
                }
            }
            DataAccess.writeAllFile(books, Config.pathBooks);
            Console.WriteLine("Ejemplares añadidos con éxito");
            DataAccess.logAction($"{quantity} ejemplares del libro: {id} añadidos.");
        }

        public static void removeExemplar(int id, int quantity)
        {
            string[,] books = DataAccess.readFile(Config.pathBooks, 9);

            for (int i = 0; i < books.GetLength(0); i++)
            {
                if (books[i, 0] == id.ToString())
                {
                    books[i, 5] = (int.Parse(books[i, 5]) - quantity).ToString();
                }
            }
            DataAccess.writeAllFile(books, Config.pathBooks);
            Console.WriteLine("Ejemplares retirados con éxito");
            DataAccess.logAction($"{quantity} ejemplares del libro: {id} retirados.");
        }

        public static void listAllBorrowedBooks()
        {
            string[,] books = DataAccess.readFile(Config.pathBooks, 9);
            for (int i = 1; i < books.GetLength(0); i++)
            {
                if (books[i, 6] != "")
                {
                    Console.WriteLine($"Libro: {books[i, 1]} \t Prestado por: {books[i, 6]}");
                }
            }
        }

        public static List<string> listAllUserswithOverdueBooks()
        {
            string[,] users = DataAccess.readFile(Config.pathUser, 6);
            List<string> overdueUsers = new List<string>();

            for (int i = 1; i < users.GetLength(0); i++)
            {
                string[] borrowedDates = users[i, 4].Split(';');
                string[] borrowedBooks = users[i, 5].Split(';');

                for (int j = 0; j < borrowedDates.Length; j++)
                {
                    if (!string.IsNullOrWhiteSpace(borrowedDates[j]) && borrowedDates[j] != "0" && DateTime.Compare(DateTime.ParseExact(borrowedDates[j], "M/d/yyyy h:mm:ss tt", new CultureInfo("en-US")), DateTime.Now) < 0)
                    {
                        overdueUsers.Add(users[i, 0]);
                        Console.WriteLine($"Usuario: {users[i, 0]} \t Libro pendiente de devolución: {DataAccess.showBookInfo(borrowedBooks[j])}");
                        break; // Break to avoid adding the same user multiple times
                    }
                }
            }

            return overdueUsers;
        }

        public static void listAllBookswithoutExemplars()
        {
            string[,] books = DataAccess.readFile(Config.pathBooks, 9);
            for (int i = 0; i < books.GetLength(0); i++)
            {
                if (books[i, 5] == "0")
                {
                    Console.WriteLine($"Libro: {books[i, 1]} \t Sin ejemplares disponibles");
                }
            }
        }

    }

    public static class User
    {
        public static void searchBook()
        {
            Console.WriteLine("1. Por ID\n2. Por Titulo\n3. Por Autor\n4. Por año\n5. Por genero");
            int option = Convert.ToInt32(Console.ReadLine());
            string[,] books = DataAccess.readFile(Config.pathBooks, 9);
            bool found = false;

            switch (option)
            {
                case 1:
                    Console.WriteLine("Ingresa el id del libro");
                    int id = Convert.ToInt32(Console.ReadLine());
                    for (int i = 0; i < books.GetLength(0); i++)
                    {
                        if (books[i, 0] == id.ToString())
                        {
                            Console.WriteLine($"Titulo: {books[i, 1]}");
                            found = true;
                        }
                    }
                    break;

                case 2:
                    Console.WriteLine("Ingresa el titulo del libro");
                    string title = Console.ReadLine();
                    title = title.Trim().ToLower();
                    for (int i = 0; i < books.GetLength(0); i++)
                    {
                        if (books[i, 1].ToLower().Contains(title))
                        {
                            Console.WriteLine($"Book ID: {books[i, 0]} Titulo: {books[i, 1]}");
                            found = true;
                        }
                    }
                    break;

                case 3:
                    Console.WriteLine("Ingresa el autor del libro");
                    string author = Console.ReadLine();
                    author = author.Trim().ToLower();
                    for (int i = 0; i < books.GetLength(0); i++)
                    {
                        if (books[i, 2].ToLower().Contains(author))
                        {
                            Console.WriteLine($"Book ID: {books[i, 0]} Titulo: {books[i, 1]}");
                            found = true;
                        }
                    }
                    break;

                case 4:
                    Console.WriteLine("Ingresa el año del libro");
                    int year = Convert.ToInt32(Console.ReadLine());
                    for (int i = 0; i < books.GetLength(0); i++)
                    {
                        if (books[i, 3] == year.ToString())
                        {
                            Console.WriteLine($"Book ID: {books[i, 0]} Titulo: {books[i, 1]}");
                            found = true;
                        }
                    }
                    break;

                case 5:
                    Console.WriteLine("Ingresa el genero del libro");
                    string genre = Console.ReadLine();
                    genre = genre.Trim().ToLower();
                    for (int i = 0; i < books.GetLength(0); i++)
                    {
                        if (books[i, 4].ToLower().Contains(genre))
                        {
                            Console.WriteLine($"Book ID: {books[i, 0]} Titulo: {books[i, 1]}");
                            found = true;
                        }
                    }
                    break;

                default:
                    Console.WriteLine("Opción no válida");
                    break;
            }

            if (!found)
            {
                Console.WriteLine("No se encontro el libro");
            }
        }

        public static void borrowBook(string username, string bookId)
        {
            string[,] books = DataAccess.readFile(Config.pathBooks, 9);
            string[,] users = DataAccess.readFile(Config.pathUser, 6);

            //Si no hay libros en la base de datos por lo tanto no se puede prestar
            if (books.GetLength(0) == 0)
            {
                Console.WriteLine("No hay libros disponibles");
                return;
            }

            bool bookFound = false;

            for (int i = 0; i < books.GetLength(0); i++)
            {
                if (books[i, 0] == bookId)
                {
                    bookFound = true;
                    if (int.Parse(books[i, 5]) == 0)
                    {
                        Console.WriteLine("No hay ejemplares disponibles");
                        return;
                    }

                    for (int j = 0; j < users.GetLength(0); j++)
                    {
                        if (users[j, 0] == username)
                        {
                            if (canBorrow(username) is false)
                            {
                                return;
                            }
                            else 
                            {
                                users[j, 5] = users[j, 5] + bookId+";";//Añadir el libro a los libros prestados
                                users[j, 3] = (int.Parse(users[j, 3]) + 1).ToString(); //Aumentar el contador de libros prestados
                                users[j, 4] = users[j, 4] + DateTime.Now.AddDays(5).ToString() + ";"; //Añadir la fecha de prestamo
                                books[i, 5] = (int.Parse(books[i, 5]) - 1).ToString(); //Disminuir el contador de ejemplares
                                books[i, 6] = books[i, 6] + username +";"; //Añadir el usuario que presto el libro 
                                books[i, 8] = books[i, 8] + DateTime.Now.ToString() + ";"; //Añadir la fecha de prestamo
                                books[i, 7] = books[i, 7] + DateTime.Now.AddDays(5).ToString() +";"; //Añadir la fecha de devolución

                                DataAccess.writeAllFile(users, Config.pathUser);
                                DataAccess.writeAllFile(books, Config.pathBooks);
                                Console.WriteLine("Libro prestado con éxito");
                                return;
                            }
                        }
                    }
                }
            }

            //Si al iterar todos los libros no se encuentra el libro
            if (!bookFound)
            {
                Console.WriteLine("Libro no encontrado");
            }
        }

        public static bool canBorrow(string username)
        {
            string[,] users = DataAccess.readFile(Config.pathUser, 6);
            for (int i = 0; i < users.GetLength(0); i++)
            {
                if (users[i, 0] == username)
                {
                    if (int.Parse(users[i, 3]) == 3)
                    {
                        Console.WriteLine("No puedes pedir más libros");
                        return false;
                    }else if (int.Parse(users[i,3])>0)
                    {
                        string[ ] duedates = users[i, 4].Split(";");
                        for (int j = 0; j < duedates.Length; j++)
                        {
                            if (duedates[j] != "" && DateTime.Compare(DateTime.ParseExact(duedates[j], "M/d/yyyy h:mm:ss tt", new CultureInfo("en-US")), DateTime.Now) <0)
                            {
                                Console.WriteLine("Libro pendiente de devolucion");
                                return false;
                            }
                            Console.WriteLine(duedates[j]);
                        }
                        
                    }
                }
            }
            return true;
        }

        public static void remainingDays(string username)
        {
            // Vamos a mostrar al usuario los libros que tiene prestados y los días que le faltan para devolverlos
            string[,] users = DataAccess.readFile(Config.pathUser, 6);

            for (int i = 0; i < users.GetLength(0); i++)
            {
                if (users[i, 0] == username)
                {
                    if (int.Parse(users[i, 3]) == 0)
                    {
                        Console.WriteLine("No tienes libros prestados");
                    }
                    else
                    {
                        string[] books = users[i, 5].Split(";");
                        string[] duedates = users[i, 4].Split(";");
                        for (int j = 0; j < books.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(duedates[j]))
                            {
                                DateTime dueDate = DateTime.ParseExact(duedates[j], "M/d/yyyy h:mm:ss tt", new CultureInfo("en-US"));
                                TimeSpan timeRemaining = dueDate.Subtract(DateTime.Now);
                                int daysRemaining = timeRemaining.Days;
                                if(daysRemaining > 0)
                                {
                                    Console.WriteLine($"Libro: {books[j]} Dias restantes: {daysRemaining} Fecha de devolución: {duedates[j]}");
                                }else
                                {
                                    Console.WriteLine("Libro vencido");
                                    Console.WriteLine("En tu devolucion tendras que pagar una multa de 10 pesos por dia de retraso");
                                    Console.WriteLine("Total: " + daysRemaining * -10 + " pesos");
                                }
                                
                            }
                        }
                    }
                    break;
                }
            }
        }

        public static void listBorrowedBooks (string username)
        {
            string[,] users = DataAccess.readFile(Config.pathUser, 6);
            string[,] books = DataAccess.readFile(Config.pathBooks, 9);
            for (int i = 0; i < users.GetLength(0); i++)
            {
                if (users[i, 0] == username)
                {
                    if (users[i, 5] == "")
                    {
                        Console.WriteLine("No tienes libros prestados");
                        return;
                    }
                    else
                    {
                        string[] borrowedBooks = users[i, 5].Split(";");
                        for (int j = 0; j < borrowedBooks.Length; j++)
                        {
                            if (borrowedBooks[j] != "")
                            {
                                for (int k = 0; k < books.GetLength(0); k++)
                                {
                                    if (books[k, 0] == borrowedBooks[j])
                                    {
                                        Console.WriteLine($"ID: {books[k, 0]} Nombre: {books[k, 1]}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void returnBook(string username)
        {
            string[,] users = DataAccess.readFile(Config.pathUser, 6);
            string[,] books = DataAccess.readFile(Config.pathBooks, 9);
            bool userFound = false;
            bool bookReturned = false;

            // Encontrar el usuario
            for (int i = 0; i < users.GetLength(0); i++)
            {
                if (users[i, 0] == username)
                {
                    userFound = true;
                    int borrowedCount = int.Parse(users[i, 3]);
                    if (borrowedCount == 0)
                    {
                        Console.WriteLine("No tienes libros prestados");
                        return;
                    }

                    string[] borrowedBooks = users[i, 5].Split(";");
                    string[] borrowedDates = users[i, 4].Split(";");

                    Console.WriteLine("Ingresa el id del libro a devolver: ");
                    User.listBorrowedBooks(username);   
                    string bookID = Console.ReadLine();
                    for (int j = 0; j < borrowedBooks.Length; j++)
                    {
                        if (borrowedBooks[j] == bookID)
                        {
                            DateTime dueDate = DateTime.ParseExact(borrowedDates[j], "M/d/yyyy h:mm:ss tt", new CultureInfo("en-US"));
                            TimeSpan timeRemaining = dueDate.Subtract(DateTime.Now);
                            int daysRemaining = timeRemaining.Days;

                            if (daysRemaining < 0)
                            {
                                int totalFine = daysRemaining * -10;
                                Console.WriteLine("Libro vencido");
                                Console.WriteLine("En tu devolución tendrás que pagar una multa de 10 pesos por día de retraso");
                                Console.WriteLine("Total: " + totalFine + " pesos");

                                // Cambiar la due date por la cantidad de multa
                                borrowedDates[j] = totalFine.ToString();
                                borrowedBooks[j] = "";
                                users[i, 5] = string.Join(";", borrowedBooks);
                                users[i, 4] = string.Join(";", borrowedDates);
                                users[i, 3] = (int.Parse(users[i, 3]) - 1).ToString();
                                bookReturned = true;
                                break;
                            }
                            else
                            {
                                borrowedBooks[j] = "";
                                borrowedDates[j] = "";
                                users[i, 5] = string.Join(";", borrowedBooks);
                                users[i, 4] = string.Join(";", borrowedDates);
                                users[i, 3] = (int.Parse(users[i, 3]) - 1).ToString();
                                bookReturned = true;
                                break;
                            }
                        }
                    }

                    if (bookReturned)
                    {
                        // Actualizar información del libro
                        for (int k = 0; k < books.GetLength(0); k++)
                        {
                            if (books[k, 0] == bookID)
                            {
                                string[] borrowedUsers = books[k, 6].Split(";");
                                string[] bookBorrowedDates = books[k, 7].Split(";");
                                string[] borrowedOns = books[k, 8].Split(";");
                                for (int m = 0; m < borrowedUsers.Length; m++)
                                {
                                    if (borrowedUsers[m] == username)
                                    {
                                        borrowedUsers[m] = "";
                                        bookBorrowedDates[m] = "";
                                        borrowedOns[m] = "";
                                    }
                                }
                                books[k, 6] = string.Join(";", borrowedUsers);
                                books[k, 7] = string.Join(";", bookBorrowedDates);
                                books[k, 8] = string.Join(";", borrowedOns);
                                books[k, 5] = (int.Parse(books[k, 5]) + 1).ToString();
                                break;
                            }
                        }

                        Console.WriteLine("Libro devuelto con éxito");
                        DataAccess.writeAllFile(users, Config.pathUser);
                        DataAccess.writeAllFile(books, Config.pathBooks);
                        DataAccess.logAction($"Usuario {username} regresó libro id: {bookID}");
                        break;
                    }
                }
            }

            if (!userFound)
            {
                Console.WriteLine("Usuario no encontrado");
            }
            else if (!bookReturned)
            {
                Console.WriteLine("Libro no encontrado entre los préstamos del usuario");
            }
        }

        public static void transferBorrow(string username, string bookID, string newUsername)
        {
            string[,] users = DataAccess.readFile(Config.pathUser, 6);
            string[,] books = DataAccess.readFile(Config.pathBooks, 9);

            // Verifica si el nuevo usuario puede tomar prestado el libro
            bool canNewUserBorrow = false;
            for (int i = 0; i < users.GetLength(0); i++)
            {
                if (users[i, 0] == newUsername)
                {
                    if (canBorrow(newUsername))
                    {
                        canNewUserBorrow = true;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("El nuevo usuario no puede tomar prestado el libro.");
                        return;
                    }
                }
            }

            if (!canNewUserBorrow)
            {
                Console.WriteLine("El nuevo usuario no fue encontrado.");
                return;
            }

            // Variable para verificar si el libro fue encontrado y eliminado del usuario original
            bool bookRemoved = false;

            // Recorre el arreglo de usuarios para encontrar al usuario actual que tiene el libro
            for (int i = 0; i < users.GetLength(0); i++)
            {
                if (users[i, 0] == username) // Si encuentra al usuario actual
                {
                    // Obtiene los libros prestados y las fechas de devolución del usuario actual
                    string[] borrowedBooks = users[i, 5].Split(";");
                    string[] borrowedDates = users[i, 4].Split(";");

                    // Recorre los libros prestados para encontrar el libro específico
                    for (int j = 0; j < borrowedBooks.Length; j++)
                    {
                        if (borrowedBooks[j] == bookID) // Si encuentra el libro específico
                        {
                            // Elimina el libro y la fecha de devolución del usuario actual
                            borrowedBooks[j] = "";
                            borrowedDates[j] = "";
                            users[i, 5] = string.Join(";", borrowedBooks);
                            users[i, 4] = string.Join(";", borrowedDates);
                            // Actualiza el contador de libros prestados del usuario actual
                            users[i, 3] = (int.Parse(users[i, 3]) - 1).ToString();
                            bookRemoved = true; // Marca que el libro fue encontrado y eliminado
                            break;
                        }
                    }
                    if (bookRemoved) break; // Si el libro fue encontrado y eliminado, sale del bucle
                }
            }

            // Si el libro no fue encontrado en los libros prestados por el usuario original
            if (!bookRemoved)
            {
                Console.WriteLine("El libro especificado no está prestado por el usuario original.");
                return;
            }

            // Recorre nuevamente el arreglo de usuarios para asignar el libro al nuevo usuario
            for (int i = 0; i < users.GetLength(0); i++)
            {
                if (users[i, 0] == newUsername) // Si encuentra al nuevo usuario
                {
                    // Obtiene los libros prestados y las fechas de devolución del nuevo usuario
                    string[] borrowedBooks = users[i, 5].Split(";");
                    string[] borrowedDates = users[i, 4].Split(";");

                    // Recorre los libros prestados para encontrar un espacio vacío
                    for (int j = 0; j < borrowedBooks.Length; j++)
                    {
                        if (borrowedBooks[j] == "") // Si encuentra un espacio vacío
                        {
                            // Asigna el libro y la nueva fecha de devolución al nuevo usuario
                            borrowedBooks[j] = bookID;
                            borrowedDates[j] = DateTime.Now.AddDays(5).ToString();
                            users[i, 5] = string.Join(";", borrowedBooks);
                            users[i, 4] = string.Join(";", borrowedDates);
                            // Actualiza el contador de libros prestados del nuevo usuario
                            users[i, 3] = (int.Parse(users[i, 3]) + 1).ToString();
                            Console.WriteLine("Libro prestado con éxito al nuevo usuario.");
                            break;
                        }
                    }
                }
            }

            // Escribe todos los datos de los usuarios de nuevo en el archivo
            DataAccess.writeAllFile(users, Config.pathUser);
            // Registra la acción de transferencia en el log
            DataAccess.logAction($"Usuario {username} transfirió libro id: {bookID} a {newUsername}");
        }

        public static void sugestBook(string recommendation)
        {
            File.AppendAllText(Config.pathRecommendations, recommendation + "\n");
            Console.WriteLine("Gracias por tu recomendación");
        }

    }
}
