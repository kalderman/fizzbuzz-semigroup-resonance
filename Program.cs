using System;
using System.Collections.Generic;
using System.Linq;

namespace SemigroupResonanceFizzBuzz
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<T> generate<T>(T first, Func<T, T> next)
            {
                var current = first;
                yield return current;
                while (true)
                {
                    current = next(current);
                    yield return current;
                }
            }

            IEnumerable<IMaybe<string>> fizz()
            {
                yield return Maybe.None<string>();
                yield return Maybe.None<string>();
                yield return Maybe.Some("Fizz");
            }


            IEnumerable<IMaybe<string>> buzz()
            {
                yield return Maybe.None<string>();
                yield return Maybe.None<string>();
                yield return Maybe.None<string>();
                yield return Maybe.None<string>();
                yield return Maybe.Some("Buzz");
            }

            string toString<T>(T t) => t.ToString();

            var fizzBuzz = generate(1, x => ++x).Select(toString)
                .Zip(
                    fizz().cycle().Zip(
                        buzz().cycle(), Maybe.compose<string>(string.Concat)),
                    Maybe.fromMaybe);

            fizzBuzz.Take(100).ToList().ForEach(Console.WriteLine);

            Console.ReadKey();
        }
    }

    public static class Seq
    {
        public static IEnumerable<T> cycle<T>(this IEnumerable<T> ts)
        {
            while (true)
                foreach (var t in ts)
                    yield return t;
        }
    }
    public interface IMaybe<A>
    {
        B Match<B>(Func<A, B> some, Func<B> none);
    }
    public record Some<A>(A a) : IMaybe<A>
    {
        public B Match<B>(Func<A, B> some, Func<B> none) => some(a);
    }
    public record None<A>() : IMaybe<A>
    {
        public B Match<B>(Func<A, B> some, Func<B> none) => none();
    }
    public static class Maybe
    {
        public static IMaybe<A> Some<A>(A a) => new Some<A>(a);
        public static IMaybe<A> None<A>() => new None<A>();
        public static T fromMaybe<T>(T @default, IMaybe<T> m)
            => m.Match(t => t, () => @default);
        public static Func<IMaybe<T>, IMaybe<T>, IMaybe<T>> compose<T>(Func<T, T, T> f)
            => (mA, mB) => mA.Match(a => Some(mB.Match(b => f(a, b), () => a)), () => mB);
    }
}
