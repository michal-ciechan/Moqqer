namespace MoqqerNamespace.Tests.TestClasses
{
    public class FizzBuzzGame
    {
        public IFizz Fizz { get; }
        public IBuzz Buzz { get; }

        public FizzBuzzGame(IFizz fizz, IBuzz buzz)
        {
            Fizz = fizz;
            Buzz = buzz;
        }

        public string Get(int number)
        {
            var isFizz = number%Fizz.Divisor == 0;
            var isBuzz = number%Buzz.Divisor == 0;

            if (isFizz && isBuzz)
                return "FizzBuzz";

            if (isFizz)
                return "Fizz";

            if (isBuzz)
                return "Buzz";

            return number.ToString();
        }
    }
}