using BenchmarkDotNet.Running;
using ConditionalQueries;
using ConditionalQueries.Parsers;
using Data;
using Rinku.ConditionalSelect;
using Rinku.Context;
using Rinku.Http.ConditionalColumns;
using Rinku.Http.ConnectionClasses;
using Test;
var res = Retriver.ParsersDict;
var res2 = 0;//IParser<WithDataReader, string>.Instance;
//var c = res == res2;
/*
var test = new LookupBenchmark();
test.Setup();
var res = test.Delegate();
res = test.DelegateCached();
res = test.DelegateNew();
res = test.DelegateNewCached();
*/
//BenchmarkRunner.Run<Test.LookupBenchmark>();
//var t = new Test.Testi();
//Console.WriteLine((t.WithArr() == t.WithDT()).ToString());
//Console.WriteLine(Test.Testi.CmpWithDT());
/*
var item = new ConditionalSelectQuery<JSONContext>(@"SELECT TOP(100) * FROM Boxes b 
INNER JOIN vwSources sr ON sr.BoxTypeID = b.BoxTypeID AND sr.SourceID = b.SourceID 
LEFT JOIN Stations st ON st.ID = b.StationID 
INNER JOIN BoxTypes bt ON bt.ID = b.BoxTypeID 
INNER JOIN Products pr ON pr.ID = b.ProductID 
LEFT JOIN Statuses s ON s.ID = b.StatusID 
LEFT JOIN StatusGroups sg ON sg.ID = s.StatusGroupID 
LEFT JOIN Users u ON u.ID = b.UserID", new CondColList<JSONContext>()
    .Add(new SingleValCondCol<int>("ID", "b.ID")).WhenHasRoles([1])
);
var noctx = new NoContext(null, "", null);
var ctx = new JSONContext(noctx);
ctx.SetCredential(new User(1, [1]));
var res = item.Parse(ctx, [], out var template);
var item2 = "UNION ALL SELECT 1 FROM 4";
//foreach (var item in Testi.sqlQueries) {
Query query1 = item;
Query query2 = item2;
var query3 = query1.MergeWith(query2);
//if (item == res)
//continue;
Console.WriteLine(item);
Console.WriteLine();
Console.WriteLine(query1.Parse());
Console.WriteLine();
Console.WriteLine(query2.Parse());
Console.WriteLine();
Console.WriteLine(query3.Parse());
//}

Query query = "SELECT name, email FROM Users INNER JOIN potato on 1=1 WHERE /*potato/ id IN       (SELECT user_id FROM Orders WHERE #total > @minTotal AND order_date > @startDate)";
Console.WriteLine(query.GetParsableQuery());
query = query.GetParsableQuery();
Console.WriteLine(query.GetParsableQuery());
Console.WriteLine(query.Parse(["potato", "minTotal"]));
*/
