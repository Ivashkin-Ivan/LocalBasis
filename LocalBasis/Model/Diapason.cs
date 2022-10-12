using System;

namespace LocalBasis.Model
{
    public class Diapason
    {
        private readonly int from;
        private readonly int to;
        private readonly double result;

        public Diapason(int from, int to, double result)
        {
            this.from = from;
            this.to = to;
            this.result = result;
        }

        public double GetResult()
            => result;

        public bool IsValueInside(int value)
            => from <= value && value <= to;
    }

}
   
