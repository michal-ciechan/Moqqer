namespace MoqqerNamespace.Tests.TestClasses
{
    public class Buzz : IBuzz
    {
        public Buzz(int divisor)
        {
            Divisor = divisor;
        }

        public int Divisor { get; set; }
    }
}