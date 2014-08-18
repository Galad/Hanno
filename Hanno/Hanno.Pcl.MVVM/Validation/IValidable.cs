namespace Hanno.Validation
{
    public interface IValidable
    {
        IValidator Validator { get; }
    }
}