
using System.Collections.Generic;
using System.Linq;


namespace CarShop
{
    public class AutoSalon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Car> Cars { get; set; }

        public int CarsWithBigEngine()
        {
            return Cars.Count(car => car.Engine > 2.0);
        }
    }

    public class Car
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public double Engine { get; set; }

        public override string ToString()
        {
            return $"{Id},{Model},{Engine}";
        }
    }

}
