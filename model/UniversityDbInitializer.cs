using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.model
{
    class UniversityDbInitializer: CreateDatabaseIfNotExists<UniversityContext>
    {
        // Переопределяем метод Seed для добавления начальных данных при создании бд если ее нет
        protected override void Seed(UniversityContext context)
        {
            var faculties = new List<Faculty>
            {
                new Faculty { Name = "Факультет инженерии", Adress = "ул. Инженерная, 1", Telephone = "123-456-7890" },
                new Faculty { Name = "Факультет гуманитарных наук", Adress = "ул. Гуманитарная, 2", Telephone = "987-654-3210" },
                new Faculty { Name = "Факультет медицины", Adress = "ул. Здоровья, 3", Telephone = "555-123-4567" },
                new Faculty { Name = "Факультет экономики", Adress = "ул. Финансовая, 4", Telephone = "111-222-3333" },
                new Faculty { Name = "Факультет искусств", Adress = "ул. Художественная, 5", Telephone = "999-888-7777" },
            };
            faculties.ForEach(f => context.Faculties.Add(f));
            context.SaveChanges();

            var departments = new List<Department>
            {
                new Department { Name = "Информационные технологии", FacultyId = 1 },
                new Department { Name = "Литература", FacultyId = 2 },
                new Department { Name = "Хирургия", FacultyId = 3 },
                new Department { Name = "Психология", FacultyId = 2 },
                new Department { Name = "Экономика", FacultyId = 4 },

            };
            departments.ForEach(d => context.Departments.Add(d));
            context.SaveChanges();

            var teachers = new List<Teacher>
            {
                new Teacher { Name = "Иван", LastName = "Иванов", Address = "ул. Пушкина, 10", Telephone = "111-222-3333", DepartmentID = 1 },
                new Teacher { Name = "Елена", LastName = "Петрова", Address = "ул. Толстого, 5", Telephone = "444-555-6666", DepartmentID = 2 },
                new Teacher { Name = "Александр", LastName = "Сидоров", Address = "ул. Здоровья, 7", Telephone = "777-888-9999", DepartmentID = 3 },
                new Teacher { Name = "Мария", LastName = "Кузнецова", Address = "ул. Психологическая, 3", Telephone = "123-456-7890", DepartmentID = 4 },
                new Teacher { Name = "Владимир", LastName = "Смирнов", Address = "ул. Экономическая, 8", Telephone = "555-666-7777", DepartmentID = 5 },
                new Teacher { Name = "Ольга", LastName = "Козлова", Address = "ул. Геологическая, 15", Telephone = "777-111-2222", DepartmentID = 1 },
                new Teacher { Name = "Андрей", LastName = "Соловьев", Address = "ул. Литературная, 20", Telephone = "888-999-0000", DepartmentID = 2 },
                new Teacher { Name = "Наталья", LastName = "Петрова", Address = "ул. Хирургическая, 11", Telephone = "333-444-5555", DepartmentID = 3 },
                new Teacher { Name = "Екатерина", LastName = "Ильина", Address = "ул. Искусств, 25", Telephone = "111-222-3333", DepartmentID = 4 },
                new Teacher { Name = "Алексей", LastName = "Морозов", Address = "ул. Математическая, 8", Telephone = "444-555-6666", DepartmentID = 5 },
                new Teacher { Name = "Светлана", LastName = "Лебедева", Address = "ул. Медицинская, 17", Telephone = "777-888-9999", DepartmentID = 1 },
                new Teacher { Name = "Дмитрий", LastName = "Васнецов", Address = "ул. Экономическая, 14", Telephone = "123-456-7890", DepartmentID = 2 },
                new Teacher { Name = "Татьяна", LastName = "Соколова", Address = "ул. Психологическая, 21", Telephone = "555-666-7777", DepartmentID = 3 },

            };
            teachers.ForEach(t => context.Teachers.Add(t));
            context.SaveChanges();
        }
    }
}
