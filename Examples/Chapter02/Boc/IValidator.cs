namespace Boc.Services
{
    // type parameter could be declared as covariant or contravariant
    // 1 class for each validation and a simple interface
    public interface IValidator<T> // in forces contravariant
    {
        bool IsValid(T t);
    }
}