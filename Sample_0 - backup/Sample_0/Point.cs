using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample_0
{
    /// <summary>
    /// Создам модель методом Code First
    /// </summary>
    public abstract class Point
    {
        protected double x;
        protected double y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public abstract double Area();
        public abstract double Perimeter();
    }
}
