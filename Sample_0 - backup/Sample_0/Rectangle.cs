using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample_0
{
    public class Rectangle : Point
    {
        protected double width;
        protected double height;

        public Rectangle(double x, double y,

        double width, double height) : base(x, y)
        {
            this.width = width;
            this.height = height;
        }

        public override double Area()
        {
            return this.width * this.height;
        }

        public override double Perimeter()
        {
            return 2 * (this.width + this.height);
        }
    }
}
