using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Controllers
{
    class Cat
    {
        public string Name { get; set; }
    }

    public class HomeController : BaseController
    {
        ViewParser _viewParser;

        public HomeController()
        {
            _viewParser = new ViewParser();
        }

        List<Dog> _dogs = new List<Dog>
        {
            new Dog
            {
                Id = "1",
                Name = "Bob",
                Color = "Brown",
                Age = 4,
                Url = "https://hips.hearstapps.com/hmg-prod.s3.amazonaws.com/images/dog-puppy-on-garden-royalty-free-image-1586966191.jpg?crop=1.00xw:0.669xh;0,0.190xh&resize=1200:*"
            },
            new Dog
            {
                Id = "2",
                Name = "Alice",
                Color = "Copper",
                Age = 2,
                Url = "https://i.insider.com/5484d9d1eab8ea3017b17e29?width=600&format=jpeg&auto=webp"
            }
        };

        public string Dog(string dogId)
        {
            Dog dog = _dogs.Where(x => x.Id == dogId).First();

            return _viewParser.ParseFile(GetExecutingRouteName, dog);
        }




        public string Cat()
        {
            Cat cat = new Cat { Name=  "Lester" };

            return _viewParser.ParseFile(GetExecutingRouteName, cat);
        }

        public string AddDog()
        {
            return _viewParser.ParseFile(GetExecutingRouteName);
        }
    }
}
