namespace MoqqerNamespace.Tests.TestClasses
{
    public class Fizz : IFizz
    {
        public Fizz(int divisor)
        {
            Divisor = divisor;
        }

        public int Divisor { get; set; }
    }
}