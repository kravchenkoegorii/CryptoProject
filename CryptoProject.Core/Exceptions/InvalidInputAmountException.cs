namespace CryptoProject.Core.Exceptions
{
    public class InvalidInputAmountException : Exception
    {
        public InvalidInputAmountException() : base() { }

        public InvalidInputAmountException(string message) : base(message) { }

    }
}
