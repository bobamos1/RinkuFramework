using ConditionalQueries.Interfaces;
using ConditionalQueries.QueryValueConditions;
using ConditionalQueries.QueryValues;

namespace ConditionalQueries.SpecialConditions;
public class VariablesAsConditions : ISpecialCondition {
    public static IQueryValueCondition Parse(IQueryValue queryValue) {
        return queryValue switch {
            QueryValueSimple simp => GetVariablesFromSimple(simp),
            QueryValueSubquery sub => GetVariablesFromSubquery(sub),
            _ => throw new NotSupportedException()
        };
    }
    public static IQueryValueCondition GetVariablesFromSimple(QueryValueSimple queryValueSimple) {
        Span<int> indexes = stackalloc int[StringManipExtensions.StackallocIntBufferSizeLimit];
        var nb = queryValueSimple.SQL.AsSpan().FillIndexes('@', indexes);
        if (nb == 0)
            throw new Exception();
        if (nb == 1)
            return new SingleConditionByVariable(
                Query.GetVariable(queryValueSimple.SQL, indexes[0])
                ?? throw new Exception()
            );
        var results = new string[nb];
        for (int i = 0; i < nb; i++)
            results[i] = Query.GetVariable(queryValueSimple.SQL, indexes[i]) ?? throw new Exception();
        return new MultipleRequiredConditionByVariable(results);
    }
    public static IQueryValueCondition GetVariablesFromSubquery(QueryValueSubquery queryValueSubquery) {
        Span<int> indexes = stackalloc int[StringManipExtensions.StackallocIntBufferSizeLimit];
        var nbBefore = queryValueSubquery.SQLBefore.AsSpan().FillIndexes('@', indexes);
        var nbAfter = queryValueSubquery.SQLAfter.AsSpan().FillIndexes('@', indexes, nbBefore);
        if (nbBefore + nbAfter == 0)
            throw new Exception();
        if (nbBefore + nbAfter == 1)
            return new SingleConditionByVariable(
                Query.GetVariable(nbAfter == 0
                    ? queryValueSubquery.SQLBefore
                    : queryValueSubquery.SQLAfter,
                indexes[0]) ?? throw new Exception()
            );
        var results = new string[nbBefore + nbAfter];
        int i = 0;
        for (; i < nbBefore; i++)
            results[i] = Query.GetVariable(queryValueSubquery.SQLBefore, indexes[i]) ?? throw new Exception();
        for (; i < nbAfter; i++)
            results[i] = Query.GetVariable(queryValueSubquery.SQLAfter, indexes[i]) ?? throw new Exception();
        return new MultipleRequiredConditionByVariable(results);
    }
}
