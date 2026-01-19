using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HotelBookingApp
{
    // Структури для збереження даних (використовуються лише як контейнери)
    struct User
    {
        public int Id;
        public string Login;
        public string Password;
        public bool IsAdmin;
    }

    struct Hotel
    {
        public int Id;
        public string Name;
        public string City;
    }

    struct Room
    {
        public int Id;
        public int HotelId;
        public string RoomNumber;
        public decimal Price;
        public int Capacity;
    }

    struct Booking
    {
        public int Id;
        public int UserId;
        public int RoomId;
        public DateTime StartDate;
        public DateTime EndDate;
        public bool IsPaid;
    }

    class Program
    {
        // Глобальні змінні для зберігання даних у пам'яті
        static List<User> users = new List<User>();
        static List<Hotel> hotels = new List<Hotel>();
        static List<Room> rooms = new List<Room>();
        static List<Booking> bookings = new List<Booking>();
        static User currentUser; // Поточний авторизований користувач

        // Шляхи до файлів
        static string usersFile = "users.txt";
        static string hotelsFile = "hotels.txt";
        static string roomsFile = "rooms.txt";
        static string bookingsFile = "bookings.txt";

        static void Main(string[] args)
        {
            // Налаштування кодування для коректного відображення кирилиці
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            LoadData(); // Завантаження даних з файлів

            while (true)
            {
                if (currentUser.Id == 0)
                {
                    ShowAuthMenu();
                }
                else
                {
                    ShowMainMenu();
                }
            }
        }

        // --- Блок роботи з файлами ---

        // --- Виправлений блок роботи з файлами ---

        static void LoadData()
        {
            // Завантаження користувачів
            if (File.Exists(usersFile))
            {
                foreach (var line in File.ReadAllLines(usersFile, Encoding.UTF8))
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 4)
                    {
                        users.Add(new User
                        {
                            Id = int.Parse(parts[0]),
                            Login = parts[1],
                            Password = parts[2],
                            IsAdmin = bool.Parse(parts[3])
                        });
                    }
                }
            }
            // Додаємо адміна за замовчуванням, якщо база порожня
            if (users.Count == 0)
            {
                users.Add(new User { Id = 1, Login = "admin", Password = "admin", IsAdmin = true });
                SaveUsers();
            }

            // Завантаження готелів
            if (File.Exists(hotelsFile))
            {
                foreach (var line in File.ReadAllLines(hotelsFile, Encoding.UTF8))
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 3)
                    {
                        hotels.Add(new Hotel { Id = int.Parse(parts[0]), Name = parts[1], City = parts[2] });
                    }
                }
            }

            // Завантаження номерів
            if (File.Exists(roomsFile))
            {
                foreach (var line in File.ReadAllLines(roomsFile, Encoding.UTF8))
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 5)
                    {
                        rooms.Add(new Room
                        {
                            Id = int.Parse(parts[0]),
                            HotelId = int.Parse(parts[1]),
                            RoomNumber = parts[2],
                            Price = decimal.Parse(parts[3]),
                            Capacity = int.Parse(parts[4])
                        });
                    }
                }
            }

            // Завантаження бронювань
            if (File.Exists(bookingsFile))
            {
                foreach (var line in File.ReadAllLines(bookingsFile, Encoding.UTF8))
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 6)
                    {
                        bookings.Add(new Booking
                        {
                            Id = int.Parse(parts[0]),
                            UserId = int.Parse(parts[1]),
                            RoomId = int.Parse(parts[2]),
                            StartDate = DateTime.Parse(parts[3]),
                            EndDate = DateTime.Parse(parts[4]),
                            IsPaid = bool.Parse(parts[5])
                        });
                    }
                }
            }
        }

        static void SaveUsers()
        {
            List<string> lines = new List<string>();
            foreach (var u in users) lines.Add($"{u.Id}|{u.Login}|{u.Password}|{u.IsAdmin}");
            File.WriteAllLines(usersFile, lines, Encoding.UTF8);
        }

        static void SaveHotels()
        {
            List<string> lines = new List<string>();
            foreach (var h in hotels) lines.Add($"{h.Id}|{h.Name}|{h.City}");

            File.WriteAllLines(hotelsFile, lines, Encoding.UTF8);
        }

        static void SaveRooms()
        {
            List<string> lines = new List<string>();
            foreach (var r in rooms) lines.Add($"{r.Id}|{r.HotelId}|{r.RoomNumber}|{r.Price}|{r.Capacity}");
            File.WriteAllLines(roomsFile, lines, Encoding.UTF8);
        }

        static void SaveBookings()
        {
            List<string> lines = new List<string>();
            foreach (var b in bookings) lines.Add($"{b.Id}|{b.UserId}|{b.RoomId}|{b.StartDate.ToShortDateString()}|{b.EndDate.ToShortDateString()}|{b.IsPaid}");
            // Додано Encoding.UTF8
            File.WriteAllLines(bookingsFile, lines, Encoding.UTF8);
        }

        // --- Блок авторизації ---

        static void ShowAuthMenu()
        {
            Console.Clear();
            Console.WriteLine("«Вітаємо у сервісі бронювання готелів»");
            Console.WriteLine("1. Вхід");
            Console.WriteLine("2. Реєстрація");
            Console.WriteLine("3. Вихід");
            Console.Write("Оберіть дію: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1": Login(); break;
                case "2": Register(); break;
                case "3": Environment.Exit(0); break;
                default: Console.WriteLine("«Невірний вибір»"); Console.ReadKey(); break;
            }
        }

        static void Login()
        {
            string login = GetValidatedInput("Введіть логін: ");
            string pass = GetValidatedInput("Введіть пароль: ");

            foreach (var u in users)
            {
                if (u.Login == login && u.Password == pass)
                {
                    currentUser = u;
                    Console.WriteLine($"«Успішний вхід. Вітаємо, {u.Login}!»");
                    Console.ReadKey();
                    return;
                }
            }
            Console.WriteLine("«Невірний логін або пароль»");
            Console.ReadKey();
        }

        static void Register()
        {
            Console.WriteLine("«Реєстрація нового користувача»");

            string login;
            while (true)
            {
                // Використовуємо базову перевірку на пустий рядок, а потім додаткові умови
                login = GetValidatedInput("Придумайте логін (мін. 4 символи): ");

                if (login.Length < 4)
                {
                    Console.WriteLine("«Помилка: Логін занадто короткий. Потрібно мінімум 4 символи.»");
                    continue;
                }

                bool exists = false;
                foreach (var u in users)
                {
                    if (u.Login == login)
                    {
                        exists = true;
                        break;
                    }
                }

                if (exists)
                {
                    Console.WriteLine("«Помилка: Такий користувач вже існує. Спробуйте інший логін.»");
                }
                else
                {
                    break; // Логін підходить
                }
            }

            string pass;
            while (true)
            {
                pass = GetValidatedInput("Придумайте пароль (мін. 6 символів, повинен містити цифру): ");

                if (pass.Length < 6)
                {
                    Console.WriteLine("«Помилка: Пароль занадто короткий. Потрібно мінімум 6 символів.»");
                    continue;
                }

                bool hasDigit = false;
                foreach (char c in pass)
                {
                    if (char.IsDigit(c))
                    {
                        hasDigit = true;
                        break;
                    }
                }

                if (!hasDigit)
                {
                    Console.WriteLine("«Помилка: Пароль повинен містити хоча б одну цифру.»");
                    continue;
                }

                Console.Write("Повторіть пароль: ");
                string confirmPass = Console.ReadLine();

                if (pass != confirmPass)
                {
                    Console.WriteLine("«Помилка: Паролі не співпадають. Спробуйте ще раз.»");
                }
                else
                {
                    break; // Пароль пройшов всі перевірки
                }
            }

            int newId = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;

            User newUser = new User { Id = newId, Login = login, Password = pass, IsAdmin = false };
            users.Add(newUser);
            SaveUsers();

            Console.WriteLine("«Реєстрація успішна! Тепер увійдіть у систему.»");
            Console.ReadKey();
        }

        // --- Головне меню ---

        static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine($"«Меню користувача: {currentUser.Login}»");

            if (currentUser.IsAdmin)
            {
                Console.WriteLine("1. Управління готелями (CRUD)");
                Console.WriteLine("2. Управління номерами (CRUD)");
                Console.WriteLine("3. Перегляд всіх бронювань");
            }
            else
            {
                Console.WriteLine("1. Пошук та бронювання");
                Console.WriteLine("2. Мої бронювання");
                Console.WriteLine("3. Скасувати бронювання");
            }

            Console.WriteLine("0. Вихід з акаунту");
            Console.Write("Ваш вибір: ");

            string choice = Console.ReadLine();

            if (currentUser.IsAdmin)
            {
                switch (choice)
                {
                    case "1": ManageHotels(); break;
                    case "2": ManageRooms(); break;
                    case "3": ViewAllBookings(); break;
                    case "0": currentUser = new User(); break;
                    default: Console.WriteLine("«Невідома команда»"); Console.ReadKey(); break;
                }
            }
            else
            {
                switch (choice)
                {
                    case "1": SearchAndBook(); break;
                    case "2": ViewMyBookings(); break;
                    case "3": CancelBooking(); break;
                    case "0": currentUser = new User(); break;
                    default: Console.WriteLine("«Невідома команда»"); Console.ReadKey(); break;
                }
            }
        }

        // --- Функції Адміністратора ---

        static void ManageHotels()
        {
            Console.Clear();
            Console.WriteLine("«Управління готелями»");
            Console.WriteLine("1. Додати готель");
            Console.WriteLine("2. Показати всі готелі");
            Console.WriteLine("3. Видалити готель (та всі його номери)");
            Console.Write("Вибір: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                string name = GetValidatedInput("Назва готелю: ");
                string city = GetValidatedInput("Місто: ");
                int newId = hotels.Count > 0 ? hotels.Max(h => h.Id) + 1 : 1;
                hotels.Add(new Hotel { Id = newId, Name = name, City = city });
                SaveHotels();
                Console.WriteLine("«Готель додано»");
                Console.ReadKey();
            }
            else if (choice == "2")
            {
                Console.WriteLine("«Список готелів:»");
                foreach (var h in hotels)
                {
                    Console.WriteLine($"ID: {h.Id} | Готель: «{h.Name}» | Місто: {h.City}");
                }
                Console.ReadKey();
            }
            else if (choice == "3")
            {
                // Виводимо список перед видаленням
                Console.WriteLine("«Оберіть готель для видалення зі списку:»");
                foreach (var h in hotels)
                {
                    Console.WriteLine($"ID: {h.Id} | Готель: «{h.Name}» | Місто: {h.City}");
                }
                Console.WriteLine("--------------------------------------------------");

                int id = GetIntInput("Введіть ID готелю для видалення: ");

                int index = -1;
                for (int i = 0; i < hotels.Count; i++)
                {
                    if (hotels[i].Id == id)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    for (int i = rooms.Count - 1; i >= 0; i--)
                    {
                        if (rooms[i].HotelId == id)
                        {
                            rooms.RemoveAt(i);
                        }
                    }
                    SaveRooms();

                    hotels.RemoveAt(index);
                    SaveHotels();
                    Console.WriteLine("«Готель та всі його номери успішно видалено»");
                }
                else
                {
                    Console.WriteLine("«Готель з таким ID не знайдено»");
                }
                Console.ReadKey();
            }
        }

        static void ManageRooms()
        {
            Console.Clear();
            Console.WriteLine("«Управління номерами»");
            Console.WriteLine("1. Додати номер");
            Console.WriteLine("2. Список номерів");
            Console.WriteLine("3. Редагувати ціну номеру");
            Console.WriteLine("4. Видалити номер");
            Console.Write("Вибір: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.WriteLine("«Список доступних готелів:»");
                if (hotels.Count == 0)
                {
                    Console.WriteLine("«У системі немає жодного готелю. Спочатку додайте готель.»");
                    Console.ReadKey();
                    return;
                }

                foreach (var h in hotels)
                {
                    Console.WriteLine($"ID: {h.Id} | Готель: «{h.Name}» | Місто: {h.City}");
                }
                Console.WriteLine("--------------------------------------------------");

                int hotelId = GetIntInput("Введіть ID готелю, до якого додати номер: ");
                bool hotelExists = false;
                foreach (var h in hotels) if (h.Id == hotelId) hotelExists = true;

                if (!hotelExists)
                {
                    Console.WriteLine("«Готель з таким ID не існує»");
                    Console.ReadKey();
                    return;
                }

                string number = GetValidatedInput("Номер кімнати: ");
                decimal price = GetDecimalInput("Ціна за добу: ");
                int capacity = GetIntInput("Кількість місць: ");

                int newId = rooms.Count > 0 ? rooms.Max(r => r.Id) + 1 : 1;
                rooms.Add(new Room { Id = newId, HotelId = hotelId, RoomNumber = number, Price = price, Capacity = capacity });
                SaveRooms();
                Console.WriteLine("«Номер успішно додано»");
                Console.ReadKey();
            }
            else if (choice == "2")
            {
                Console.WriteLine("«Список номерів:»");
                foreach (var r in rooms)
                {
                    string hotelName = "Невідомо";
                    foreach (var h in hotels) if (h.Id == r.HotelId) hotelName = h.Name;
                    Console.WriteLine($"ID: {r.Id} | Готель: «{hotelName}» | Кімната: {r.RoomNumber} | Ціна: {r.Price} | Місць: {r.Capacity}");
                }
                Console.ReadKey();
            }
            else if (choice == "3")
            {
                // Виведення списку перед редагуванням ---
                Console.WriteLine("«Оберіть номер для редагування ціни зі списку:»");
                foreach (var r in rooms)
                {
                    string hotelName = "Невідомо";
                    foreach (var h in hotels) if (h.Id == r.HotelId) hotelName = h.Name;
                    Console.WriteLine($"ID: {r.Id} | Готель: «{hotelName}» | Кімната: {r.RoomNumber} | Поточна ціна: {r.Price}");
                }
                Console.WriteLine("--------------------------------------------------");
                // --------------------------------------------------

                int roomId = GetIntInput("Введіть ID номеру для зміни ціни: ");
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (rooms[i].Id == roomId)
                    {
                        decimal newPrice = GetDecimalInput("Нова ціна: ");
                        Room updated = rooms[i];
                        updated.Price = newPrice;
                        rooms[i] = updated;

                        SaveRooms();
                        Console.WriteLine("«Ціну оновлено»");
                        Console.ReadKey();
                        return;
                    }
                }
                Console.WriteLine("«Номер не знайдено»");
                Console.ReadKey();
            }
            else if (choice == "4")
            {
                Console.WriteLine("«Оберіть номер для видалення зі списку:»");
                foreach (var r in rooms)
                {
                    string hotelName = "Невідомо";
                    foreach (var h in hotels) if (h.Id == r.HotelId) hotelName = h.Name;
                    Console.WriteLine($"ID: {r.Id} | Готель: «{hotelName}» | Кімната: {r.RoomNumber} | Ціна: {r.Price}");
                }
                Console.WriteLine("--------------------------------------------------");

                int roomId = GetIntInput("Введіть ID номеру для видалення: ");
                int index = -1;

                for (int i = 0; i < rooms.Count; i++)
                {
                    if (rooms[i].Id == roomId)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    rooms.RemoveAt(index);
                    SaveRooms();
                    Console.WriteLine("«Номер успішно видалено»");
                }
                else
                {
                    Console.WriteLine("«Номер з таким ID не знайдено»");
                }
                Console.ReadKey();
            }
        }

        static void ViewAllBookings()
        {
            Console.WriteLine("«Всі бронювання в системі:»");
            foreach (var b in bookings)
            {
                string userLogin = "Невідомий";
                foreach (var u in users) if (u.Id == b.UserId) userLogin = u.Login;

                Console.WriteLine($"ID: {b.Id} | Клієнт: {userLogin} | Кімната ID: {b.RoomId} | Період: {b.StartDate.ToShortDateString()} - {b.EndDate.ToShortDateString()} | Оплачено: {b.IsPaid}");
            }
            Console.ReadKey();
        }

        // --- Функції Клієнта ---

        static void SearchAndBook()
        {
            Console.Clear();
            Console.WriteLine("«Пошук номерів»");
            string city = GetValidatedInput("Введіть місто для пошуку: ");
            int capacity = GetIntInput("Необхідна кількість місць: ");

            // Фільтрація
            List<Room> foundRooms = new List<Room>();
            foreach (var r in rooms)
            {
                foreach (var h in hotels)
                {
                    if (h.Id == r.HotelId && h.City.ToLower() == city.ToLower() && r.Capacity >= capacity)
                    {
                        foundRooms.Add(r);
                    }
                }
            }

            if (foundRooms.Count == 0)
            {
                Console.WriteLine("«На жаль, нічого не знайдено»");
                Console.ReadKey();
                return;
            }

            // Сортування (бульбашкою для прикладу алгоритму)
            Console.WriteLine("Сортувати за ціною? (1 - так, 0 - ні): ");
            if (Console.ReadLine() == "1")
            {
                for (int i = 0; i < foundRooms.Count - 1; i++)
                {
                    for (int j = 0; j < foundRooms.Count - i - 1; j++)
                    {
                        if (foundRooms[j].Price > foundRooms[j + 1].Price)
                        {
                            Room temp = foundRooms[j];
                            foundRooms[j] = foundRooms[j + 1];
                            foundRooms[j + 1] = temp;
                        }
                    }
                }
            }

            Console.WriteLine("«Знайдені варіанти:»");
            foreach (var r in foundRooms)
            {
                string hotelName = "";
                foreach (var h in hotels) if (h.Id == r.HotelId) hotelName = h.Name;
                Console.WriteLine($"ID: {r.Id} | Готель: «{hotelName}» | Кімната: {r.RoomNumber} | Ціна: {r.Price}");
            }

            Console.WriteLine("«Введіть ID номеру для бронювання або 0 для виходу»");
            int choiceId = GetIntInput("Ваш вибір: ");
            if (choiceId == 0) return;

            bool roomExists = false;
            foreach (var r in foundRooms) if (r.Id == choiceId) roomExists = true;

            if (roomExists)
            {
                CreateBooking(choiceId);
            }
            else
            {
                Console.WriteLine("«Невірний ID зі списку»");
                Console.ReadKey();
            }
        }

        static void CreateBooking(int roomId)
        {
            Console.WriteLine("«Оформлення бронювання»");

            DateTime start;
            while (true)
            {
                Console.Write("Введіть дату заїзду (рік-місяць-день): ");
                if (!DateTime.TryParse(Console.ReadLine(), out start))
                {
                    Console.WriteLine("«Некоректний формат дати. Спробуйте ще раз.»");
                    continue;
                }

                // Перевірка: дата заїзду не може бути меншою за поточну дату
                if (start.Date < DateTime.Now.Date)
                {
                    Console.WriteLine("«Помилка: Не можна забронювати номер на минулу дату.»");
                    continue;
                }

                break;
            }

            Console.Write("Введіть дату виїзду: ");
            DateTime end;
            // Дата виїзду має бути пізніше дати заїзду
            while (!DateTime.TryParse(Console.ReadLine(), out end) || end.Date <= start.Date)
            {
                Console.Write("«Дата виїзду має бути пізнішою за дату заїзду. Спробуйте ще раз: »");
            }

            // Перевірка на перетин дат з існуючими бронюваннями
            foreach (var b in bookings)
            {
                if (b.RoomId == roomId)
                {
                    // Якщо інтервали перетинаються
                    if (start < b.EndDate && end > b.StartDate)
                    {
                        Console.WriteLine("«На жаль, на ці дати номер вже зайнятий іншим клієнтом»");
                        Console.ReadKey();
                        return;
                    }
                }
            }

            Console.WriteLine("«Бажаєте оплатити зараз? (так/ні)»");
            bool isPaid = Console.ReadLine().ToLower() == "так";

            int newId = bookings.Count > 0 ? bookings.Max(b => b.Id) + 1 : 1;
            bookings.Add(new Booking
            {
                Id = newId,
                UserId = currentUser.Id,
                RoomId = roomId,
                StartDate = start,
                EndDate = end,
                IsPaid = isPaid
            });
            SaveBookings();
            Console.WriteLine("«Бронювання успішно створено!»");
            Console.ReadKey();
        }

        static void ViewMyBookings()
        {
            Console.WriteLine("«Ваші бронювання:»");
            bool found = false;
            foreach (var b in bookings)
            {
                if (b.UserId == currentUser.Id)
                {
                    found = true;
                    string roomInfo = "";
                    foreach (var r in rooms)
                    {
                        if (r.Id == b.RoomId)
                        {
                            foreach (var h in hotels) if (h.Id == r.HotelId) roomInfo = $"{h.Name}, №{r.RoomNumber}";
                        }
                    }
                    Console.WriteLine($"ID: {b.Id} | {roomInfo} | {b.StartDate.ToShortDateString()} - {b.EndDate.ToShortDateString()} | Сплачено: {(b.IsPaid ? "Так" : "Ні")}");
                }
            }
            if (!found) Console.WriteLine("«У вас поки немає бронювань»");
            Console.ReadKey();
        }
        static void CancelBooking()
        {
            Console.Clear();
            Console.WriteLine("«Скасування бронювання»");

            // Спочатку показуємо список бронювань користувача
            bool hasBookings = false;
            Console.WriteLine("«Ваші поточні бронювання:»");

            foreach (var b in bookings)
            {
                if (b.UserId == currentUser.Id)
                {
                    hasBookings = true;
                    string roomInfo = "";
                    foreach (var r in rooms)
                    {
                        if (r.Id == b.RoomId)
                        {
                            foreach (var h in hotels) if (h.Id == r.HotelId) roomInfo = $"{h.Name}, №{r.RoomNumber}";
                        }
                    }
                    Console.WriteLine($"ID: {b.Id} | {roomInfo} | {b.StartDate.ToShortDateString()} - {b.EndDate.ToShortDateString()} | Сплачено: {(b.IsPaid ? "Так" : "Ні")}");
                }
            }
            Console.WriteLine("--------------------------------------------------");

            if (!hasBookings)
            {
                Console.WriteLine("«У вас немає активних бронювань для скасування»");
                Console.ReadKey();
                return;
            }

            int bookingId = GetIntInput("Введіть ID бронювання, яке бажаєте скасувати (або 0 для виходу): ");

            if (bookingId == 0) return;

            // Шукаємо бронювання та перевіряємо, чи належить воно поточному користувачу
            int index = -1;
            for (int i = 0; i < bookings.Count; i++)
            {
                if (bookings[i].Id == bookingId && bookings[i].UserId == currentUser.Id)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                bookings.RemoveAt(index);
                SaveBookings();
                Console.WriteLine("«Бронювання успішно скасовано»");
            }
            else
            {
                Console.WriteLine("«Бронювання з таким ID не знайдено або воно вам не належить»");
            }
            Console.ReadKey();
        }

        // --- Допоміжні методи валідації ---

        static string GetValidatedInput(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("«Поле не може бути порожнім»");
                }
            } while (string.IsNullOrWhiteSpace(input));
            return input;
        }

        static int GetIntInput(string prompt)
        {
            int value;
            do
            {
                Console.Write(prompt);
            } while (!int.TryParse(Console.ReadLine(), out value));
            return value;
        }

        static decimal GetDecimalInput(string prompt)
        {
            decimal value;
            do
            {
                Console.Write(prompt);
            } while (!decimal.TryParse(Console.ReadLine(), out value));
            return value;
        }
    }
}