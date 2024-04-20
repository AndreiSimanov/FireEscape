namespace FireEscape.Factories.Interfaces;

public interface IBaseObjectFactory<out T, in P> where T : BaseObject where P : BaseObject
{
    T CreateDefault(P? parent);
}
