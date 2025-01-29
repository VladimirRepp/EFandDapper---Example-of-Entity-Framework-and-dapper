using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample_0
{
    public class Circle : Point
    {
        protected double radius;
        protected double height;

        public Circle(double x, double y,
        double radius) : base(x, y)
        {
            this.radius = radius;
        }

        public override double Area()
        {
            return Math.PI * this.radius * this.radius;
        }

        public override double Perimeter()
        {
            return Math.PI * this.radius * 2;
        }
    }
}
