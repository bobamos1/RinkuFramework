namespace ConditionalQueries.Interfaces;

public delegate IQueryValueCondition ParseSpecialCondition(IQueryValue queryValue);
public interface ISpecialCondition {
    public abstract static IQueryValueCondition Parse(IQueryValue queryValue);
}
