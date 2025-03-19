using System.Data;
using System.Text;
using BenchmarkDotNet.Attributes;
using HTMLTemplating;
using Microsoft.Data.SqlClient;
using Rinku.Tools.Data;


//using Rinku.PropertyParsers.DataReader;
using Rinku.Tools.HTML;
using DataTable = Rinku.Tools.Data.DataTable;

namespace Test;

/*
public interface IParsable<T> : IDTPropertyParser {
    public T Parse(int index, DataTable dt);
    public int Index { get; }
}
public class StringParser(string Name, int Index) : IParsable<HTMLItem> {
    public string Name { get; } = Name;
    public int Index { get; set; } = Index;
    public HTMLItem Parse(int index, DataTable dt) => new HTMLElement(Tag.TD, dt.Get<string>(Name, index));
}
public class IntParser(string Name, int Index) : IntDTParser(Name), IParsable<HTMLItem> {
    public int Index { get; set; } = Index;
    public HTMLItem Parse(int index, DataTable dt) => new HTMLElement(Tag.TD, dt.Get<int>(Name, index).ToString());
}
[MemoryDiagnoser]
public class Testi {
    public static readonly string[] sqlQueries = [
        // Query with multiple nested SELECTs and aggregate functions
        "SELECT CustomerID, (SELECT MAX(Amount) FROM Orders WHERE CustomerID = Customers.CustomerID) AS MaxOrderAmount FROM Customers WHERE EXISTS (SELECT 1 FROM Orders WHERE Orders.CustomerID = Customers.CustomerID AND Amount > 1000)",
    
        // Recursive CTE with different levels of recursion and ORDER BY
        "WITH RecursiveHierarchy AS (SELECT EmployeeID, ManagerID, 0 AS Level FROM Employees WHERE ManagerID IS NULL UNION ALL SELECT e.EmployeeID, e.ManagerID, rh.Level + 1 FROM Employees e INNER JOIN RecursiveHierarchy rh ON e.ManagerID = rh.EmployeeID) SELECT EmployeeID, Level FROM RecursiveHierarchy WHERE Level <= 5 ORDER BY Level DESC",
    
        // Query with multiple window functions
        "SELECT OrderID, CustomerID, Amount, RANK() OVER (PARTITION BY CustomerID ORDER BY Amount DESC) AS OrderRank, SUM(Amount) OVER (PARTITION BY CustomerID) AS TotalSpent FROM Orders WHERE OrderDate > '2024-01-01'",
    
        // Complex JOIN with a subquery in the ON clause
        "SELECT p.ProductID, p.Name, o.OrderID FROM Products p INNER JOIN Orders o ON o.CustomerID = (SELECT CustomerID FROM Customers WHERE Customers.Email = 'test@example.com') WHERE o.OrderDate > '2024-01-01'",
    
        // Query using HAVING with aggregate function on a subquery
        "SELECT CustomerID, COUNT(*) AS OrderCount FROM Orders WHERE OrderDate > '2024-01-01' GROUP BY CustomerID HAVING COUNT(*) > (SELECT COUNT(*) FROM Orders WHERE OrderDate > '2024-01-01' AND CustomerID = 'C001')",
    
        // Query with multiple conditions in the JOIN ON clause
        "SELECT p.ProductID, p.Name, od.Quantity FROM Products p INNER JOIN OrderDetails od ON p.ProductID = od.ProductID AND od.Quantity > 5 WHERE EXISTS (SELECT 1 FROM Orders o WHERE o.OrderID = od.OrderID AND o.OrderDate > '2024-01-01')",
    
        // Nested SELECT in SELECT clause
        "SELECT OrderID, (SELECT SUM(Amount) FROM Payments WHERE Payments.OrderID = Orders.OrderID) AS TotalPayments FROM Orders WHERE OrderDate > '2024-01-01' AND (SELECT COUNT(*) FROM OrderDetails WHERE OrderDetails.OrderID = Orders.OrderID) > 2",
    
        // Window function with ROWS BETWEEN frame
        "SELECT OrderID, Amount, SUM(Amount) OVER (ORDER BY OrderDate ROWS BETWEEN 1 PRECEDING AND 1 FOLLOWING) AS RunningTotal FROM Orders WHERE OrderDate > '2024-01-01'",
    
        // Complex subquery with UNION in the WHERE clause
        "SELECT CustomerID, COUNT(*) FROM Orders WHERE CustomerID IN (SELECT CustomerID FROM Orders WHERE Amount > 500 UNION SELECT CustomerID FROM Orders WHERE OrderDate BETWEEN '2024-01-01' AND '2024-12-31') GROUP BY CustomerID",
    
        // Multi-table JOIN with a WHERE clause and subqueries
        "SELECT e.EmployeeID, e.Name, p.ProjectID FROM Employees e INNER JOIN Projects p ON e.EmployeeID = p.EmployeeID WHERE EXISTS (SELECT 1 FROM Departments d WHERE d.DepartmentID = p.DepartmentID AND d.Name = 'Sales')",
    
        // Query with CASE expression inside an aggregate function
        "SELECT CustomerID, SUM(CASE WHEN Status = 'Completed' THEN Amount ELSE 0 END) AS CompletedAmount FROM Orders WHERE OrderDate > '2024-01-01' GROUP BY CustomerID",
    
        // Query using EXISTS with multiple levels of subqueries and aggregate functions
        "SELECT CustomerID, (SELECT AVG(Amount) FROM Orders WHERE Orders.CustomerID = Customers.CustomerID) AS AvgOrderAmount FROM Customers WHERE EXISTS (SELECT 1 FROM Orders WHERE Orders.CustomerID = Customers.CustomerID AND Amount > (SELECT AVG(Amount) FROM Orders WHERE CustomerID = 'C001'))",
    
        // Query with DISTINCT and multiple window functions
        "SELECT DISTINCT CustomerID, COUNT(*) OVER (PARTITION BY CustomerID ORDER BY OrderDate) AS OrderCount, ROW_NUMBER() OVER (PARTITION BY CustomerID ORDER BY OrderDate DESC) AS OrderRow FROM Orders WHERE OrderDate > '2024-01-01'",
    
        // Complex query with JSON functions
        "SELECT CustomerID, JSON_QUERY(OrderDetails, '$.items') AS OrderItems FROM Orders WHERE JSON_VALUE(OrderDetails, '$.status') = 'Shipped' AND OrderDate > '2024-01-01'",
    
        // Query with nested UNION and ORDER BY
        "SELECT CustomerID, SUM(Amount) FROM Orders WHERE OrderDate BETWEEN '2024-01-01' AND '2024-12-31' GROUP BY CustomerID UNION ALL SELECT CustomerID, COUNT(*) FROM Orders WHERE OrderDate < '2024-01-01' GROUP BY CustomerID ORDER BY CustomerID",
    
        // Query with GROUP BY having nested conditions and subquery
        "SELECT CustomerID, COUNT(*) FROM Orders WHERE OrderDate > '2024-01-01' GROUP BY CustomerID HAVING COUNT(*) > (SELECT COUNT(*) FROM Orders WHERE CustomerID = 'C001' AND OrderDate > '2024-01-01')",
    
        // Query with recursive common table expressions and JOIN
        "WITH RECURSIVE ParentChild AS (SELECT EmployeeID, ManagerID FROM Employees WHERE ManagerID IS NULL UNION ALL SELECT e.EmployeeID, e.ManagerID FROM Employees e INNER JOIN ParentChild pc ON e.ManagerID = pc.EmployeeID) SELECT * FROM ParentChild WHERE EmployeeID IN (SELECT EmployeeID FROM Employees WHERE Department = 'Sales')",
    
        // Query with UNION and multiple conditions across tables
        "SELECT ProductID, Name FROM Products WHERE Price > 1000 UNION SELECT ProductID, Name FROM Products WHERE Category = 'Electronics' AND Price < 500 ORDER BY Name DESC",
    
        // Subquery with LIMIT and OFFSET in a SELECT
        "SELECT * FROM (SELECT ProductID, Name FROM Products ORDER BY Price DESC LIMIT 10 OFFSET 5) AS TopProducts",
    
        // Query with multi-layered JOINs and GROUP BY
        "SELECT p.ProductID, SUM(od.Quantity) AS TotalQuantity FROM Products p INNER JOIN OrderDetails od ON p.ProductID = od.ProductID INNER JOIN Orders o ON od.OrderID = o.OrderID WHERE o.OrderDate > '2024-01-01' GROUP BY p.ProductID",
    
        // Query with CROSS JOIN to generate Cartesian Product
        "SELECT p.Name, c.Name FROM Products p CROSS JOIN Categories c WHERE p.Price > 500 AND c.Name = 'Electronics'"
    ];

    public static readonly string[] sqlQueries2 = [
        "SELECT * FROM Orders WHERE OrderID IN (SELECT OrderID FROM Customers WHERE EXISTS (SELECT 1 FROM Employees WHERE EmployeeID = Orders.EmployeeID))",
        "WITH RecursiveCTE AS (SELECT EmployeeID, ManagerID FROM Employees WHERE ManagerID IS NULL UNION ALL SELECT e.EmployeeID, e.ManagerID FROM Employees e INNER JOIN RecursiveCTE r ON e.ManagerID = r.EmployeeID) SELECT * FROM RecursiveCTE",
        "SELECT * FROM Orders WHERE Amount > (SELECT MAX(Amount) FROM Orders WHERE Status = 'Pending') AND OrderDate > '2024-01-01'",
        "SELECT * FROM Products WHERE ProductID IN (SELECT ProductID FROM OrderDetails WHERE OrderID IN (SELECT OrderID FROM Orders WHERE CustomerID = 'C001'))",
        "SELECT DISTINCT OrderID, COUNT(*) OVER (PARTITION BY CustomerID) AS TotalOrders FROM Orders WHERE Amount > 1000",
        "SELECT t1.ProductID, t1.Price FROM Products t1 JOIN (SELECT ProductID, AVG(Price) AS avg_price FROM Products GROUP BY ProductID) t2 ON t1.ProductID = t2.ProductID WHERE t1.Price > t2.avg_price",
        "SELECT p1.ProductID, p2.ProductID FROM Products p1 CROSS JOIN Products p2 WHERE p1.ProductID != p2.ProductID AND p1.Price < p2.Price",
        "SELECT * FROM Orders WHERE EXISTS (SELECT 1 FROM Products WHERE Price > (SELECT AVG(Price) FROM Products WHERE Category = 'Electronics'))",
        "SELECT CASE WHEN EXISTS (SELECT 1 FROM Orders WHERE OrderDate > '2024-01-01') THEN 'Recent Orders Found' ELSE 'No Recent Orders' END",
        "WITH RankedOrders AS (SELECT OrderID, CustomerID, RANK() OVER (PARTITION BY CustomerID ORDER BY Amount DESC) AS Rank FROM Orders) SELECT * FROM RankedOrders WHERE Rank = 1",
        "SELECT * FROM Orders WHERE CustomerID = 'C001' AND EXISTS (SELECT 1 FROM Payments WHERE Payments.OrderID = Orders.OrderID AND Payments.PaymentStatus = 'Completed')",
        "SELECT * FROM Customers WHERE CustomerID IN (SELECT CustomerID FROM Orders WHERE EXISTS (SELECT 1 FROM Payments WHERE Payments.OrderID = Orders.OrderID AND Payments.Amount > 1000))",
        "SELECT * FROM Orders WHERE OrderID IN (SELECT OrderID FROM OrderDetails WHERE ProductID = 'P001') AND Amount > (SELECT AVG(Amount) FROM Orders WHERE Status = 'Shipped')",
        "SELECT * FROM Orders WHERE CustomerID = 'C001' AND (SELECT COUNT(*) FROM Payments WHERE Payments.OrderID = Orders.OrderID) > 2",
        "SELECT * FROM Products WHERE ProductID IN (SELECT ProductID FROM OrderDetails WHERE OrderID IN (SELECT OrderID FROM Orders WHERE OrderDate BETWEEN '2024-01-01' AND '2024-12-31'))",
        "SELECT CustomerID, SUM(Amount) OVER (PARTITION BY CustomerID) AS TotalSpent FROM Orders WHERE OrderDate > '2024-01-01'",
        "SELECT * FROM Products WHERE Price > (SELECT AVG(Price) FROM Products WHERE Category = 'Electronics') AND EXISTS (SELECT 1 FROM OrderDetails WHERE OrderDetails.ProductID = Products.ProductID)",
        "SELECT * FROM Orders WHERE OrderID NOT IN (SELECT OrderID FROM OrderDetails WHERE ProductID IN (SELECT ProductID FROM Products WHERE Category = 'Books'))",
        "WITH TopCustomers AS (SELECT CustomerID, SUM(Amount) AS TotalSpent FROM Orders GROUP BY CustomerID ORDER BY TotalSpent DESC LIMIT 5) SELECT * FROM TopCustomers",
        "SELECT * FROM Employees WHERE Department = 'Sales' AND EXISTS (SELECT 1 FROM Projects WHERE Projects.EmployeeID = Employees.EmployeeID AND Projects.Status = 'Active')",
        "SELECT t1.OrderID, t1.Amount FROM Orders t1 LEFT JOIN (SELECT OrderID, SUM(Amount) AS TotalAmount FROM Payments GROUP BY OrderID) t2 ON t1.OrderID = t2.OrderID WHERE t2.TotalAmount IS NULL",
        "SELECT * FROM Products WHERE ProductID IN (SELECT ProductID FROM OrderDetails WHERE OrderID IN (SELECT OrderID FROM Orders WHERE OrderDate > '2024-01-01' AND Status = 'Completed'))",
        "SELECT * FROM Orders WHERE OrderID = (SELECT TOP 1 OrderID FROM Orders WHERE CustomerID = 'C001' ORDER BY OrderDate DESC)",
        "SELECT * FROM Products WHERE EXISTS (SELECT 1 FROM OrderDetails WHERE OrderDetails.ProductID = Products.ProductID AND OrderDetails.Quantity > 10)",
        "SELECT COUNT(*) FROM (SELECT DISTINCT CustomerID FROM Orders WHERE OrderDate BETWEEN '2024-01-01' AND '2024-12-31') AS UniqueCustomers",
        "SELECT CustomerID, COUNT(*) OVER (PARTITION BY CustomerID ORDER BY OrderID) AS OrderCount, ROW_NUMBER() OVER (ORDER BY OrderID) AS RowNum FROM Orders WHERE OrderDate > '2024-01-01'",
        "SELECT * FROM Products WHERE ProductID = (SELECT ProductID FROM Products WHERE Category = 'Electronics' ORDER BY Price DESC LIMIT 1)",
        "SELECT * FROM Employees WHERE EmployeeID IN (SELECT EmployeeID FROM Employees WHERE Department = 'HR') AND EXISTS (SELECT 1 FROM Employees WHERE ManagerID = Employees.EmployeeID)"
    ];
    public static string[] Sqls { get; set; } = [
        "SELECT e.first_name, e.last_name, d.department_name, s.salary FROM employees e JOIN departments d ON e.department_id = d.department_id JOIN salaries s ON e.employee_id = s.employee_id",
        "INSERT INTO Orders (user_id, total, created_at) VALUES (@userId, @totalAmount, NOW()) WHERE @totalAmount > 0",
        "UPDATE Products SET price = price * @discountRate WHERE category = @category AND #stock > @minStock",
        "DELETE FROM Sessions WHERE user_id = @userId AND last_access < (SELECT DATE_SUB(NOW(), INTERVAL @timeout HOUR))",
        "INSERT INTO Archive SELECT * FROM Orders WHERE status = 'completed' AND order_date < @cutoffDate",
        "SELECT product_id, COUNT(*) as order_count FROM OrderItems WHERE order_id IN (SELECT id FROM Orders WHERE user_id = @userId) GROUP BY product_id HAVING COUNT(*) > @threshold",
        "SELECT name, email FROM Users WHERE id IN (SELECT user_id FROM Orders WHERE total > @minTotal AND order_date > @startDate)",
        "SELECT Users.name, Orders.total FROM Users JOIN Orders ON Users.id = Orders.user_id WHERE Orders.total > @amount AND Users.active = 1",
        "UPDATE Employees SET salary = CASE WHEN department = @dept THEN salary * 1.1 ELSE salary END WHERE hire_date < @hireDate",
        "INSERT INTO Notifications (user_id, message) SELECT user_id, 'Your order has been shipped' FROM Orders WHERE status = 'shipped' AND shipping_date IS NOT NULL",
        "SELECT p.name, SUM(oi.quantity) as total_quantity FROM Products p JOIN OrderItems oi ON p.id = oi.product_id WHERE p.active = 1 GROUP BY p.name HAVING total_quantity > @minQuantity",
        "SELECT * FROM (SELECT user_id, COUNT(*) as order_count FROM Orders GROUP BY user_id) AS UserOrders WHERE order_count > @minOrders",
        "INSERT INTO UserActivity (user_id, activity_type, timestamp) VALUES (@userId, 'login', NOW()) WHERE EXISTS (SELECT 1 FROM Users WHERE id = @userId)",
        "SELECT category, AVG(price) as average_price FROM Products WHERE price < @maxPrice GROUP BY category HAVING COUNT(*) > @minCount",
        "UPDATE Orders o SET o.status = 'completed' FROM Users u WHERE o.user_id = u.id AND u.active = 1 AND o.order_date < @cutoffDate",
        "DELETE FROM Products WHERE id NOT IN (SELECT product_id FROM OrderItems) AND created_at < @olderThanDate",
        "INSERT INTO RecentOrders (user_id, total) SELECT user_id, SUM(total) FROM Orders WHERE order_date > @startDate GROUP BY user_id HAVING SUM(total) > @threshold",
        "SELECT u.name, o.total FROM Users u LEFT JOIN Orders o ON u.id = o.user_id WHERE u.active = 1 AND (o.status IS NULL OR o.status = 'pending')",
        "INSERT INTO Orders (user_id, total) VALUES (@userId, @totalAmount)",
        "INSERT INTO Products (name, price) VALUES (@productName, @productPrice) WHERE NOT EXISTS (SELECT 1 FROM Products WHERE name = @productName)",
        "INSERT INTO Users (name, email, created_at) VALUES (@name, @email, NOW())",
        "INSERT INTO Notifications (user_id, message) SELECT id, @message FROM Users WHERE active = 1",
        "INSERT INTO AuditLog (user_id, action) VALUES (@userId, 'Created order')",
        "INSERT INTO ArchivedOrders SELECT * FROM Orders WHERE order_date < @archiveDate",
        "INSERT INTO Feedback (user_id, comment) VALUES (@userId, @comment) WHERE @comment IS NOT NULL",
        "UPDATE Users SET last_login = NOW() WHERE id = @userId",
        "UPDATE Products SET price = price * (1 - @discountRate) WHERE category = @category",
        "UPDATE Orders SET status = 'shipped' WHERE id = @orderId AND status = 'pending'",
        "UPDATE Sessions SET last_access = NOW() WHERE user_id = @userId AND last_access < NOW() - INTERVAL @timeout MINUTE",
        "UPDATE Employees SET salary = salary + @raiseAmount WHERE department_id = @deptId",
        "UPDATE Transactions SET completed_at = NOW() WHERE id = @transactionId AND completed_at IS NULL",
        "UPDATE Users SET email_verified = 1 WHERE id = @userId AND email IS NOT NULL",
        "UPDATE Orders SET total = (SELECT SUM(price * quantity) FROM OrderItems WHERE order_id = Orders.id) WHERE id = @orderId",
        "UPDATE Products SET stock = stock - @quantity WHERE id = @productId AND stock >= @quantity",
        "UPDATE Users SET name = @newName WHERE id = @userId",
        "DELETE FROM Sessions WHERE user_id = @userId AND last_access < NOW() - INTERVAL @timeout MINUTE",
        "DELETE FROM Users WHERE id = @userId AND NOT EXISTS (SELECT 1 FROM Orders WHERE user_id = @userId)",
        "DELETE FROM Cart WHERE user_id = @userId AND product_id = @productId",
        "DELETE FROM Orders WHERE order_date < @thresholdDate AND status = 'cancelled'",
        "DELETE FROM Products WHERE stock = 0 AND created_at < NOW() - INTERVAL @age MONTH",
        "DELETE FROM Feedback WHERE created_at < NOW() - INTERVAL @days DAY",
        "DELETE FROM UserPreferences WHERE user_id = @userId AND preference_key = @key",
        "DELETE FROM Notifications WHERE user_id = @userId AND read = 1",
        "DELETE FROM ArchivedOrders WHERE order_date < @archiveDate",
        "DELETE FROM Transactions WHERE completed_at < @thresholdDate AND status = 'failed'",
        "SELECT id, name FROM Users WHERE email = @email",
        "SELECT COUNT(*) as total_orders FROM Orders WHERE user_id = @userId AND status = 'completed'",
        "SELECT SUM(quantity) as total_quantity FROM OrderItems WHERE product_id = @productId",
        "SELECT name, price FROM Products WHERE category = @category AND price < @maxPrice",
        "SELECT u.name, COUNT(o.id) as total_orders FROM Users u LEFT JOIN Orders o ON u.id = o.user_id GROUP BY u.name HAVING total_orders > @minOrders",
        "SELECT p.name, SUM(oi.quantity) as total_sold FROM Products p JOIN OrderItems oi ON p.id = oi.product_id GROUP BY p.name ORDER BY total_sold DESC",
        "SELECT user_id, AVG(rating) as average_rating FROM Feedback GROUP BY user_id HAVING average_rating > @threshold",
        "SELECT email FROM Users WHERE id IN (SELECT user_id FROM Orders WHERE order_date > @startDate)",
        "SELECT DISTINCT u.id, u.name FROM Users u JOIN Orders o ON u.id = o.user_id WHERE o.status = 'pending'",
        "SELECT o.id, u.name FROM Orders o JOIN Users u ON o.user_id = u.id WHERE o.total > @minTotal ORDER BY o.order_date DESC",
        "SELECT COUNT(*) as total_users FROM Users WHERE created_at > @startDate",
        "SELECT AVG(price) as average_price FROM Products WHERE category = @category",
        "SELECT MAX(order_date) as last_order_date FROM Orders WHERE user_id = @userId",
        "SELECT MIN(created_at) as first_order_date FROM Orders WHERE user_id = @userId",
        "SELECT category, COUNT(*) as product_count FROM Products GROUP BY category HAVING product_count > @minCount",
        "SELECT user_id, SUM(total) as total_spent FROM Orders GROUP BY user_id HAVING total_spent > @threshold",
        "SELECT p.category, AVG(r.price) as average_price FROM Products p JOIN OrderItems oi ON p.id = oi.product_id GROUP BY p.category",
        "SELECT COUNT(DISTINCT user_id) as unique_users FROM Orders WHERE status = 'completed'",
        "SELECT user_id, COUNT(*) as total_feedback FROM Feedback GROUP BY user_id HAVING total_feedback > @minCount",
        "SELECT o.status, COUNT(*) as order_count FROM Orders o GROUP BY o.status",
        "SELECT name FROM Users WHERE id = (SELECT user_id FROM Orders WHERE id = @orderId)",
        "SELECT total FROM Orders WHERE user_id = @userId AND total > (SELECT AVG(total) FROM Orders)",
        "SELECT email FROM Users WHERE id IN (SELECT user_id FROM Feedback WHERE rating >= @minRating)",
        "SELECT * FROM Products WHERE id NOT IN (SELECT product_id FROM OrderItems)",
        "SELECT COUNT(*) as active_users FROM Users WHERE id IN (SELECT user_id FROM Sessions WHERE last_access > NOW() - INTERVAL @timeout MINUTE)",
        "SELECT * FROM Orders WHERE user_id = @userId AND total > (SELECT MAX(total) FROM Orders WHERE user_id = @userId)",
        "SELECT user_id, COUNT(*) as order_count FROM Orders WHERE total > @minTotal GROUP BY user_id HAVING order_count > @minCount",
        "SELECT name FROM Products WHERE id IN (SELECT product_id FROM OrderItems WHERE order_id = @orderId)",
        "SELECT * FROM Users WHERE EXISTS (SELECT 1 FROM Orders WHERE user_id = Users.id)",
        "SELECT * FROM Feedback WHERE user_id IN (SELECT id FROM Users WHERE active = 1)",
        "INSERT INTO RecentOrders SELECT * FROM Orders WHERE order_date > @cutoffDate",
        "INSERT INTO UserLogs (user_id, activity) SELECT id, 'Logged in' FROM Users WHERE active = 1",
        "INSERT INTO UserActivities (user_id, action) SELECT id, 'Ordered' FROM Orders WHERE order_date > @startDate",
        "INSERT INTO UserPoints (user_id, points) SELECT user_id, SUM(points) FROM Feedback GROUP BY user_id",
        "INSERT INTO DailySales (date, total_sales) SELECT CURRENT_DATE, SUM(total) FROM Orders WHERE order_date = CURRENT_DATE GROUP BY date",
        "INSERT INTO ProductViews (user_id, product_id) SELECT @userId, product_id FROM Products WHERE stock > 0",
        "INSERT INTO DeletedOrders SELECT * FROM Orders WHERE status = 'deleted' AND deleted_at < @olderThanDate",
        "INSERT INTO OrderHistory SELECT * FROM Orders WHERE order_date < @archiveDate",
        "INSERT INTO UserMessages (user_id, message) SELECT id, 'Welcome!' FROM Users WHERE email_verified = 1",
        "INSERT INTO UserRewards (user_id, reward) SELECT id, 'Bonus Points' FROM Users WHERE active = 1",
        "DELETE FROM Orders WHERE total < @minTotal AND created_at < @thresholdDate",
        "DELETE FROM Users WHERE id IN (SELECT user_id FROM Orders WHERE total < @minTotal)",
        "DELETE FROM Products WHERE stock = 0 AND id NOT IN (SELECT product_id FROM OrderItems)",
        "DELETE FROM Feedback WHERE user_id = @userId AND created_at < NOW() - INTERVAL @days DAY",
        "DELETE FROM Notifications WHERE user_id = @userId AND read = 1 AND created_at < NOW() - INTERVAL @days DAY",
        "DELETE FROM ArchivedOrders WHERE created_at < @thresholdDate",
        "DELETE FROM UserPreferences WHERE user_id = @userId AND preference_key = @key AND created_at < NOW() - INTERVAL @days DAY",
        "DELETE FROM Transactions WHERE status = 'failed' AND created_at < @thresholdDate",
        "DELETE FROM Sessions WHERE user_id = @userId AND last_access < NOW() - INTERVAL @timeout MINUTE",
        "DELETE FROM UserActivities WHERE created_at < NOW() - INTERVAL @days DAY",
        "SELECT u.name, COUNT(o.id) as total_orders FROM Users u JOIN Orders o ON u.id = o.user_id WHERE o.status = 'completed' GROUP BY u.id",
        "SELECT p.name, SUM(oi.quantity) as total_sold FROM Products p JOIN OrderItems oi ON p.id = oi.product_id WHERE p.active = 1 GROUP BY p.name",
        "SELECT u.name, AVG(f.rating) as average_rating FROM Users u JOIN Feedback f ON u.id = f.user_id GROUP BY u.name",
        "SELECT o.id, u.name FROM Orders o LEFT JOIN Users u ON o.user_id = u.id WHERE o.total > @minTotal",
        "SELECT p.category, COUNT(*) as total_products FROM Products p JOIN OrderItems oi ON p.id = oi.product_id GROUP BY p.category",
        "SELECT o.id, u.name FROM Orders o INNER JOIN Users u ON o.user_id = u.id WHERE o.status = 'shipped' AND u.active = 1",
        "SELECT u.name, COUNT(DISTINCT o.id) as total_orders FROM Users u LEFT JOIN Orders o ON u.id = o.user_id GROUP BY u.name",
        "SELECT o.id, u.name FROM Orders o LEFT JOIN Users u ON o.user_id = u.id WHERE o.total < @maxTotal ORDER BY o.created_at DESC",
        "SELECT p.name, AVG(oi.quantity) as avg_quantity FROM Products p JOIN OrderItems oi ON p.id = oi.product_id GROUP BY p.name",
        "SELECT u.name, o.total FROM Users u JOIN Orders o ON u.id = o.user_id WHERE o.status = 'pending'",
        "SELECT id FROM Products WHERE price BETWEEN @minPrice AND @maxPrice",
        "SELECT id, name FROM Users WHERE email LIKE @emailPattern",
        "SELECT COUNT(*) as total FROM Orders WHERE created_at > @startDate AND status = 'completed'",
        "SELECT u.id, o.total FROM Users u JOIN Orders o ON u.id = o.user_id WHERE o.created_at > @startDate",
        "SELECT p.name, (SELECT COUNT(*) FROM OrderItems oi WHERE oi.product_id = p.id) as total_orders FROM Products p",
        "SELECT * FROM Users WHERE active = 1 AND created_at > NOW() - INTERVAL @days DAY",
        "SELECT u.id, o.total FROM Users u JOIN Orders o ON u.id = o.user_id WHERE o.total > @minTotal ORDER BY o.created_at DESC",
        "SELECT * FROM Products WHERE category = @category ORDER BY price ASC",
        "SELECT o.id, u.name FROM Orders o JOIN Users u ON o.user_id = u.id WHERE o.created_at BETWEEN @startDate AND @endDate",
        "SELECT SUM(total) as [total sales] FROM Orders WHERE order_date = @today",
        "WITH NewProducts AS (SELECT id, name, price FROM Products WHERE stock > 0) INSERT INTO ArchivedProducts SELECT * FROM NewProducts WHERE created_at < @thresholdDate",
        "WITH UserOrders AS (SELECT user_id, COUNT(*) as order_count FROM Orders GROUP BY user_id) INSERT INTO UserStats (user_id, total_orders) SELECT user_id, order_count FROM UserOrders WHERE order_count > @minCount",
        "WITH ActiveUsers AS (SELECT id FROM Users WHERE active = 1) INSERT INTO Notifications (user_id, message) SELECT id, @message FROM ActiveUsers",
        "WITH RecentSales AS (SELECT product_id, SUM(quantity) as total_sold FROM OrderItems WHERE order_date > @startDate GROUP BY product_id) INSERT INTO ProductSalesHistory (product_id, total_sold) SELECT product_id, total_sold FROM RecentSales",
        "WITH FeedbackCounts AS (SELECT user_id, COUNT(*) as feedback_count FROM Feedback GROUP BY user_id) INSERT INTO UserFeedbackStats (user_id, feedback_count) SELECT user_id, feedback_count FROM FeedbackCounts WHERE feedback_count > @minCount",
        "WITH HighValueOrders AS (SELECT id, user_id FROM Orders WHERE total > @highValue) INSERT INTO PremiumOrders (order_id, user_id) SELECT id, user_id FROM HighValueOrders",
        "WITH RecentProducts AS (SELECT id, name FROM Products WHERE created_at > @recentDate) INSERT INTO ProductLog (product_id, action) SELECT id, 'New Arrival' FROM RecentProducts",
        "WITH OrderSummary AS (SELECT user_id, SUM(total) as total_spent FROM Orders WHERE status = 'completed' GROUP BY user_id) INSERT INTO UserSpending (user_id, total_spent) SELECT user_id, total_spent FROM OrderSummary",
        "WITH CartSummary AS (SELECT user_id, SUM(quantity) as total_items FROM Cart GROUP BY user_id) INSERT INTO UserCartStats (user_id, total_items) SELECT user_id, total_items FROM CartSummary",
        "WITH ExpiredProducts AS (SELECT id FROM Products WHERE expiration_date < @currentDate) DELETE FROM Products WHERE id IN (SELECT id FROM ExpiredProducts)",
        "WITH RecentOrders AS (SELECT id, total FROM Orders WHERE order_date > @cutoffDate) UPDATE RecentOrders SET status = 'review' WHERE status = 'pending'",
        "WITH UserActivity AS (SELECT user_id, MAX(last_login) as last_login FROM Users GROUP BY user_id) UPDATE Users SET status = 'active' WHERE id IN (SELECT user_id FROM UserActivity WHERE last_login > @thresholdDate)",
        "WITH UserTotals AS (SELECT user_id, COUNT(*) as order_count FROM Orders GROUP BY user_id) UPDATE UserStats SET total_orders = (SELECT order_count FROM UserTotals WHERE user_id = UserStats.user_id)",
        "WITH SalaryUpdates AS (SELECT id, salary * (1 + @raisePercentage) as new_salary FROM Employees WHERE performance_rating > @minRating) UPDATE Employees SET salary = (SELECT new_salary FROM SalaryUpdates WHERE id = Employees.id)",
        "WITH InventoryLevels AS (SELECT product_id, stock FROM Products WHERE stock < @reorderLevel) UPDATE Products SET stock = stock + @restockAmount WHERE id IN (SELECT product_id FROM InventoryLevels)",
        "WITH FailedTransactions AS (SELECT id FROM Transactions WHERE status = 'failed') UPDATE Transactions SET status = 'recovered' WHERE id IN (SELECT id FROM FailedTransactions)",
        "WITH OrderAmounts AS (SELECT user_id, SUM(total) as total FROM Orders GROUP BY user_id) UPDATE Users SET total_spent = (SELECT total FROM OrderAmounts WHERE user_id = Users.id)",
        "WITH FeedbackSummary AS (SELECT user_id, AVG(rating) as avg_rating FROM Feedback GROUP BY user_id) UPDATE Users SET average_rating = (SELECT avg_rating FROM FeedbackSummary WHERE user_id = Users.id)",
        "WITH ProductSales AS (SELECT product_id, SUM(quantity) as total_sold FROM OrderItems GROUP BY product_id) UPDATE Products SET total_sold = (SELECT total_sold FROM ProductSales WHERE product_id = Products.id)",
        "WITH UserLimits AS (SELECT user_id, MAX(login_attempts) as max_attempts FROM Users GROUP BY user_id) UPDATE Users SET login_attempts = (SELECT max_attempts FROM UserLimits WHERE user_id = Users.id)",
        "WITH InactiveUsers AS (SELECT id FROM Users WHERE last_login < @thresholdDate) DELETE FROM Users WHERE id IN (SELECT id FROM InactiveUsers)",
        "WITH OldOrders AS (SELECT id FROM Orders WHERE created_at < @thresholdDate) DELETE FROM Orders WHERE id IN (SELECT id FROM OldOrders)",
        "WITH LowStockProducts AS (SELECT id FROM Products WHERE stock < @minStock) DELETE FROM Products WHERE id IN (SELECT id FROM LowStockProducts)",
        "WITH FeedbackRecords AS (SELECT id FROM Feedback WHERE created_at < @thresholdDate) DELETE FROM Feedback WHERE id IN (SELECT id FROM FeedbackRecords)",
        "WITH ExpiredSessions AS (SELECT id FROM Sessions WHERE last_access < @thresholdDate) DELETE FROM Sessions WHERE id IN (SELECT id FROM ExpiredSessions)",
        "WITH DeletedOrders AS (SELECT id FROM Orders WHERE status = 'deleted') DELETE FROM Orders WHERE id IN (SELECT id FROM DeletedOrders)",
        "WITH ArchivedUsers AS (SELECT id FROM Users WHERE created_at < @archiveDate) DELETE FROM Users WHERE id IN (SELECT id FROM ArchivedUsers)",
        "WITH OldProducts AS (SELECT id FROM Products WHERE created_at < @thresholdDate) DELETE FROM Products WHERE id IN (SELECT id FROM OldProducts)",
        "WITH OldNotifications AS (SELECT id FROM Notifications WHERE created_at < @thresholdDate) DELETE FROM Notifications WHERE id IN (SELECT id FROM OldNotifications)",
        "WITH StaleTransactions AS (SELECT id FROM Transactions WHERE status = 'failed' AND created_at < @thresholdDate) DELETE FROM Transactions WHERE id IN (SELECT id FROM StaleTransactions)",
        "INSERT INTO UserFeedback (user_id, comment) SELECT u.id, @comment FROM Users u WHERE u.active = 1 AND NOT EXISTS (SELECT 1 FROM Feedback f WHERE f.user_id = u.id AND f.comment = @comment)",
        "INSERT INTO OrderLog (order_id, status) SELECT o.id, 'Created' FROM Orders o WHERE NOT EXISTS (SELECT 1 FROM OrderLog ol WHERE ol.order_id = o.id)",
        "INSERT INTO UserActions (user_id, action) SELECT u.id, @action FROM Users u WHERE u.email_verified = 1 AND NOT EXISTS (SELECT 1 FROM UserActions ua WHERE ua.user_id = u.id AND ua.action = @action)",
        "INSERT INTO ProductReview (product_id, user_id, review) SELECT p.id, @userId, @review FROM Products p WHERE p.stock > 0 AND NOT EXISTS (SELECT 1 FROM ProductReview pr WHERE pr.product_id = p.id AND pr.user_id = @userId)",
        "INSERT INTO DailySales (date, total_sales) SELECT CURRENT_DATE, SUM(total) FROM Orders WHERE order_date = CURRENT_DATE GROUP BY date HAVING SUM(total) > 0",
        "INSERT INTO NotificationHistory (user_id, message) SELECT user_id, @message FROM Users WHERE active = 1 AND NOT EXISTS (SELECT 1 FROM NotificationHistory nh WHERE nh.user_id = user_id AND nh.message = @message)",
        "INSERT INTO ProductStats (product_id, total_sold) SELECT product_id, SUM(quantity) FROM OrderItems WHERE product_id = @productId GROUP BY product_id HAVING SUM(quantity) > 0",
        "INSERT INTO UserSettings (user_id, setting_key, setting_value) SELECT u.id, @key, @value FROM Users u WHERE NOT EXISTS (SELECT 1 FROM UserSettings us WHERE us.user_id = u.id AND us.setting_key = @key)",
        "INSERT INTO WeeklyReports (user_id, total_sales) SELECT user_id, SUM(total) FROM Orders WHERE order_date >= DATE_SUB(CURRENT_DATE, INTERVAL 7 DAY) GROUP BY user_id",
        "UPDATE Products SET price = price * 1.1 WHERE id IN (SELECT product_id FROM OrderItems WHERE order_date > @startDate)",
        "UPDATE Users SET last_login = NOW() WHERE id IN (SELECT user_id FROM Sessions WHERE last_access > NOW() - INTERVAL @timeout MINUTE)",
        "UPDATE Orders SET status = 'completed' WHERE id IN (SELECT order_id FROM OrderLog WHERE status = 'created')",
        "UPDATE Products SET stock = stock - (SELECT SUM(quantity) FROM OrderItems WHERE product_id = Products.id) WHERE id IN (SELECT product_id FROM OrderItems)",
        "UPDATE Feedback SET processed = 1 WHERE id IN (SELECT id FROM Feedback WHERE rating < @minRating)",
        "UPDATE Transactions SET status = 'reversed' WHERE id IN (SELECT id FROM Transactions WHERE amount < 0 AND created_at < @thresholdDate)",
        "UPDATE ProductStats SET total_sold = (SELECT COUNT(*) FROM OrderItems WHERE product_id = ProductStats.product_id) WHERE product_id IN (SELECT product_id FROM Products)",
        "UPDATE UserStats SET total_orders = (SELECT COUNT(*) FROM Orders WHERE user_id = UserStats.user_id) WHERE user_id IN (SELECT id FROM Users)",
        "UPDATE Notifications SET read = 1 WHERE id IN (SELECT id FROM Notifications WHERE user_id = @userId AND read = 0)",
        "DELETE FROM Feedback WHERE id IN (SELECT id FROM Feedback WHERE user_id = @userId AND created_at < @thresholdDate)",
        "DELETE FROM Notifications WHERE id IN (SELECT id FROM Notifications WHERE user_id = @userId AND created_at < @thresholdDate)",
        "DELETE FROM Orders WHERE id IN (SELECT id FROM Orders WHERE user_id = @userId AND created_at < @thresholdDate)",
        "DELETE FROM Sessions WHERE id IN (SELECT id FROM Sessions WHERE last_access < @thresholdDate)",
        "DELETE FROM Transactions WHERE id IN (SELECT id FROM Transactions WHERE status = 'failed' AND created_at < @thresholdDate)",
        "DELETE FROM ArchivedOrders WHERE id IN (SELECT id FROM ArchivedOrders WHERE order_date < @archiveDate)",
        "DELETE FROM UserSettings WHERE id IN (SELECT id FROM UserSettings WHERE user_id = @userId AND setting_key = @key)",
        "WITH RecentProducts AS (SELECT p.id, p.name FROM Products p WHERE p.stock > 0), UserFeedback AS (SELECT f.user_id, AVG(f.rating) as avg_rating FROM Feedback f GROUP BY f.user_id) SELECT rp.id, rp.name, uf.avg_rating FROM RecentProducts rp JOIN UserFeedback uf ON rp.id = uf.user_id",
        "WITH TopSellingProducts AS (SELECT product_id, SUM(quantity) as total_sold FROM OrderItems GROUP BY product_id ORDER BY total_sold DESC LIMIT 10) SELECT p.name FROM Products p JOIN TopSellingProducts tsp ON p.id = tsp.product_id",
        "WITH OrderTotals AS (SELECT user_id, SUM(total) as total_spent FROM Orders WHERE status = 'completed' GROUP BY user_id) SELECT u.id, u.name, ot.total_spent FROM Users u LEFT JOIN OrderTotals ot ON u.id = ot.user_id WHERE ot.total_spent > @minTotal",
        "WITH OrderCount AS (SELECT user_id, COUNT(*) as order_count FROM Orders WHERE status = 'completed' GROUP BY user_id) SELECT u.id, u.name, oc.order_count FROM Users u JOIN OrderCount oc ON u.id = oc.user_id WHERE oc.order_count > @minCount",
        "WITH UserRecentOrders AS (SELECT o.user_id, COUNT(*) as recent_order_count FROM Orders o WHERE o.order_date > @recentDate GROUP BY o.user_id) SELECT u.id, u.name, uro.recent_order_count FROM Users u JOIN UserRecentOrders uro ON u.id = uro.user_id",
        "WITH AverageRatings AS (SELECT product_id, AVG(rating) as avg_rating FROM Feedback GROUP BY product_id) SELECT p.id, p.name, ar.avg_rating FROM Products p JOIN AverageRatings ar ON p.id = ar.product_id",
        "WITH UserOrders AS (SELECT user_id, COUNT(*) as order_count FROM Orders GROUP BY user_id) SELECT u.id, u.name, u.active, u.last_login, u.created_at FROM Users u LEFT JOIN UserOrders uo ON u.id = uo.user_id WHERE uo.order_count > @minCount",
        "WITH RecentSales AS (SELECT product_id, SUM(quantity) as total_sold FROM OrderItems WHERE order_date > @recentDate GROUP BY product_id) SELECT p.id, p.name FROM Products p JOIN RecentSales rs ON p.id = rs.product_id",
        "WITH ActiveSessions AS (SELECT user_id, COUNT(*) as session_count FROM Sessions GROUP BY user_id) SELECT u.id, u.name, as session_count FROM Users u JOIN ActiveSessions as ON u.id = as.user_id",
        "WITH FeedbackSummary AS (SELECT user_id, AVG(rating) as avg_rating FROM Feedback GROUP BY user_id) SELECT u.id, u.name, fs.avg_rating FROM Users u LEFT JOIN FeedbackSummary fs ON u.id = fs.user_id",
        "SELECT p.id, p.name, (SELECT COUNT(*) FROM OrderItems oi WHERE oi.product_id = p.id) as total_order_items FROM Products p WHERE p.active = 1",
        "SELECT u.id, u.name, (SELECT SUM(total) FROM Orders o WHERE o.user_id = u.id) as total_spent FROM Users u WHERE u.active = 1",
        "SELECT o.id, (SELECT u.name FROM Users u WHERE u.id = o.user_id) as user_name FROM Orders o WHERE o.total > @minTotal",
        "SELECT p.id, p.name, (SELECT AVG(f.rating) FROM Feedback f WHERE f.product_id = p.id) as average_rating FROM Products p",
        "SELECT u.id, u.name, (SELECT COUNT(*) FROM Orders o WHERE o.user_id = u.id) as total_orders FROM Users u WHERE u.active = 1",
        "SELECT p.id, p.name, (SELECT SUM(oi.quantity) FROM OrderItems oi WHERE oi.product_id = p.id) as total_quantity FROM Products p",
        "SELECT o.id, (SELECT u.name FROM Users u WHERE u.id = o.user_id) as user_name, (SELECT SUM(oi.quantity) FROM OrderItems oi WHERE oi.order_id = o.id) as total_quantity FROM Orders o",
        "SELECT u.id, (SELECT COUNT(*) FROM Orders o WHERE o.user_id = u.id) as order_count, (SELECT AVG(f.rating) FROM Feedback f WHERE f.user_id = u.id) as average_feedback FROM Users u",
        "SELECT p.id, (SELECT COUNT(*) FROM OrderItems oi WHERE oi.product_id = p.id) as order_count, (SELECT AVG(f.rating) FROM Feedback f WHERE f.product_id = p.id) as average_rating FROM Products p",
        "SELECT u.id, u.name, (SELECT COUNT(*) FROM Feedback f WHERE f.user_id = u.id) as feedback_count FROM Users u",
        "WITH UserPurchases AS (SELECT user_id, SUM(total) as total_spent FROM Orders GROUP BY user_id) SELECT u.id, u.name, up.total_spent FROM Users u JOIN UserPurchases up ON u.id = up.user_id WHERE up.total_spent > @minTotal",
        "WITH ProductReviews AS (SELECT product_id, COUNT(*) as review_count FROM Feedback GROUP BY product_id) SELECT p.id, p.name, pr.review_count FROM Products p JOIN ProductReviews pr ON p.id = pr.product_id WHERE pr.review_count > @minCount",
        "WITH UserStats AS (SELECT user_id, COUNT(*) as order_count FROM Orders GROUP BY user_id) SELECT u.id, u.name, us.order_count FROM Users u JOIN UserStats us ON u.id = us.user_id",
        "WITH RecentFeedback AS (SELECT user_id, AVG(rating) as average_rating FROM Feedback WHERE created_at > @recentDate GROUP BY user_id) SELECT u.id, u.name, rf.average_rating FROM Users u JOIN RecentFeedback rf ON u.id = rf.user_id",
        "WITH TotalSales AS (SELECT product_id, SUM(total) as total_sold FROM Orders GROUP BY product_id) SELECT p.id, p.name, ts.total_sold FROM Products p JOIN TotalSales ts ON p.id = ts.product_id",
        "WITH UserSessions AS (SELECT user_id, COUNT(*) as session_count FROM Sessions GROUP BY user_id) SELECT u.id, u.name, us.session_count FROM Users u JOIN UserSessions us ON u.id = us.user_id",
        "WITH OrderSummary AS (SELECT user_id, COUNT(*) as order_count FROM Orders GROUP BY user_id) SELECT u.id, u.name, os.order_count FROM Users u LEFT JOIN OrderSummary os ON u.id = os.user_id",
        "WITH ProductSales AS (SELECT product_id, SUM(quantity) as total_quantity FROM OrderItems GROUP BY product_id) SELECT p.id, p.name, ps.total_quantity FROM Products p JOIN ProductSales ps ON p.id = ps.product_id",
        "WITH FeedbackCounts AS (SELECT product_id, COUNT(*) as total_feedback FROM Feedback GROUP BY product_id) SELECT p.id, p.name, fc.total_feedback FROM Products p JOIN FeedbackCounts fc ON p.id = fc.product_id",
        "WITH UserAverageFeedback AS (SELECT user_id, AVG(rating) as avg_rating FROM Feedback GROUP BY user_id) SELECT u.id, u.name, uaf.avg_rating FROM Users u JOIN UserAverageFeedback uaf ON u.id = uaf.user_id",
        "WITH UserOrderStats AS (SELECT user_id, COUNT(*) as total_orders, SUM(total) as total_spent FROM Orders GROUP BY user_id) SELECT u.id, u.name, uos.total_orders, uos.total_spent FROM Users u JOIN UserOrderStats uos ON u.id = uos.user_id",
        "WITH TopUsers AS (SELECT user_id, SUM(total) as total_spent FROM Orders GROUP BY user_id ORDER BY total_spent DESC LIMIT 5) SELECT u.id, u.name FROM Users u JOIN TopUsers tu ON u.id = tu.user_id",
        "WITH ProductFeedback AS (SELECT product_id, COUNT(*) as feedback_count FROM Feedback GROUP BY product_id) SELECT p.id, p.name, pf.feedback_count FROM Products p JOIN ProductFeedback pf ON p.id = pf.product_id",
        "WITH RecentOrders AS (SELECT user_id, COUNT(*) as order_count FROM Orders WHERE order_date > @recentDate GROUP BY user_id) SELECT u.id, u.name, ro.order_count FROM Users u JOIN RecentOrders ro ON u.id = ro.user_id",
        "WITH UserPaymentStats AS (SELECT user_id, COUNT(*) as payment_count FROM Transactions GROUP BY user_id) SELECT u.id, u.name, ups.payment_count FROM Users u JOIN UserPaymentStats ups ON u.id = ups.user_id",
        "WITH FeedbackRatings AS (SELECT product_id, AVG(rating) as avg_rating FROM Feedback GROUP BY product_id) SELECT p.id, p.name, fr.avg_rating FROM Products p JOIN FeedbackRatings fr ON p.id = fr.product_id",
        "WITH OrderItemCounts AS (SELECT order_id, COUNT(*) as item_count FROM OrderItems GROUP BY order_id) SELECT o.id, o.total, o.created_at, o.user_id, oic.item_count FROM Orders o JOIN OrderItemCounts oic ON o.id = oic.order_id",
        "WITH UserStatus AS (SELECT user_id, CASE WHEN MAX(last_login) > @thresholdDate THEN 'active' ELSE 'inactive' END as status FROM Users GROUP BY user_id) SELECT u.id, u.name, us.status FROM Users u JOIN UserStatus us ON u.id = us.user_id",
        "WITH AverageOrderValue AS (SELECT user_id, AVG(total) as avg_order_value FROM Orders GROUP BY user_id) SELECT u.id, u.name, aov.avg_order_value FROM Users u JOIN AverageOrderValue aov ON u.id = aov.user_id",
        "WITH ProductStock AS (SELECT id, stock FROM Products) SELECT p.id, p.name, ps.stock FROM Products p JOIN ProductStock ps ON p.id = ps.id",
        "INSERT INTO ArchivedOrders (SELECT * FROM Orders WHERE order_date < @archiveDate AND NOT EXISTS (SELECT 1 FROM ArchivedOrders ao WHERE ao.id = Orders.id))",
        "UPDATE Users SET status = 'inactive' WHERE id IN (SELECT id FROM Users WHERE last_login < @thresholdDate AND NOT EXISTS (SELECT 1 FROM Orders WHERE user_id = Users.id))",
        "DELETE FROM Products WHERE id IN (SELECT id FROM Products WHERE created_at < @thresholdDate AND NOT EXISTS (SELECT 1 FROM OrderItems WHERE product_id = Products.id))",
        "DELETE FROM Users WHERE id IN (SELECT id FROM Users WHERE last_login < @thresholdDate AND NOT EXISTS (SELECT 1 FROM Orders WHERE user_id = Users.id))",
        "DELETE FROM OrderItems WHERE id IN (SELECT id FROM OrderItems WHERE order_id IN (SELECT id FROM Orders WHERE status = 'canceled'))",
        "SELECT id, name FROM Users WHERE active = 1 UNION SELECT id, name FROM Admins WHERE active = 1",
        "SELECT product_id, product_name FROM Products WHERE stock > 0 UNION SELECT product_id, product_name FROM DiscontinuedProducts WHERE last_stock_date > @cutoffDate",
        "SELECT id, email FROM Customers WHERE country = 'US' UNION SELECT id, email FROM Partners WHERE country = 'US'",
        "SELECT user_id, total_spent FROM Orders WHERE order_date > @date1 UNION ALL SELECT user_id, total_spent FROM PastOrders WHERE order_date > @date2",
        "SELECT category_id, category_name FROM ProductCategories UNION ALL SELECT category_id, category_name FROM ArchivedProductCategories",
        "SELECT employee_id, department_id FROM Employees WHERE status = 'active' UNION SELECT employee_id, department_id FROM Managers WHERE status = 'active'",
        "SELECT id, address FROM Users WHERE city = 'New York' UNION ALL SELECT id, address FROM Employees WHERE city = 'New York'",
        "SELECT id, name, salary FROM Employees WHERE salary > 50000 UNION ALL SELECT id, name, salary FROM Executives WHERE salary > 50000",
        "SELECT order_id, product_id FROM Orders WHERE total > 100 UNION ALL SELECT order_id, product_id FROM Returns WHERE total > 100",
        "SELECT id, name FROM Users WHERE role = 'admin' UNION SELECT id, name FROM Employees WHERE position = 'manager'",
        "UPDATE Orders o JOIN Users u ON o.user_id = u.id SET o.status = 'completed' WHERE u.active = 1 AND #o.order_date < @cutoffDate",
        "SELECT product_id, product_name FROM Products WHERE stock > 0 UNION SELECT product_id, product_name FROM DiscontinuedProducts WHERE last_stock_date > @cutoffDate",
    ];
    public const string CnnStr = "Data Source=localhost;Initial Catalog=Test;Persist Security Info=True;User ID=sa;Password=allo123; TrustServerCertificate=true;";
    public const string QueryStr = $"SELECT TOP(100000) * FROM TestTable";
    public static readonly IParsable<HTMLItem>[] Parsers = [
        new IntParser("Nb", 2),
        new StringParser("Name", 1)
    ];
    public static readonly Dictionary<string, int> Template = ToTemplate(Parsers);
    public static readonly DataTable DT = new Testi().GetDT();
    public static readonly KeyValuePair<string, int>[] Data = [..new Testi().FillList()];
    public static readonly int count = Data.Length;
    public static IDataReader Getreader() {
        var cnn = new SqlConnection(CnnStr);
        using var command = cnn.CreateCommand();
        command.CommandText = QueryStr;
        cnn.Open();
        return command.ExecuteReader(CommandBehavior.CloseConnection);
    }
    public static Dictionary<string, int> ToTemplate<T>(IEnumerable<IParsable<T>> parsers) {
        var dict = new Dictionary<string, int>();
        foreach (var parser in parsers)
            dict[parser.Name] = parser.Index - 1;
        return dict;
    }
    //[Benchmark(Baseline = true)]
    public List<KeyValuePair<string, int>> FillList() {
        using var reader = Getreader();
        var data = new List<KeyValuePair<string, int>>();
        while (reader.Read())
            data.Add(new(reader.GetString(0), reader.GetInt32(1)));
        return data;
    }
    //[Benchmark]
    public KeyValuePair<string, int>[] FillArr() {
        using var reader = Getreader();
        var data = new KeyValuePair<string, int>[count];
        var i = 0;
        while (reader.Read()) {
            data[i] = new KeyValuePair<string, int>(reader.GetString(0), reader.GetInt32(1));
            i++;
        }
        return data;
    }
    //[Benchmark]
    public DataTable GetDT() {
        using var reader = Getreader();
        return DataReaderParser.GetDataTable(Parsers, reader, Template);
    }
    //[Benchmark(Baseline = true)]
    public string WithArr() {
        var sb = new StringBuilder();
        foreach (var d in Data)
            sb.Append("<td>").Append(d.Value).Append("</td>")
              .Append("<td>").Append(d.Key).Append("</td>");
        return sb.ToString();
    }
    //[Benchmark]
    public string WithDTOnly() {
        var sb = new StringBuilder();
        for (int i = 0; i < DT.Count; i++) {
            sb.Append("<td>").Append(DT.Get<int>("Nb", i)).Append("</td>")
              .Append("<td>").Append(DT.Get<string>("Name", i)).Append("</td>");
        }
        return sb.ToString();
    }
    //[Benchmark]
    public static string WithDTCmp() {
        var table = new HTMLElement(Tag.TBL);
        var count = DT.Count;
        for (int i = 0; i < count; i++) {
            var row = new HTMLElement(Tag.TR);
            foreach (var prop in Parsers)
                row.AppendChild(prop.Parse(i, DT));
            table.AppendChild(row);
        }
        return table.ToHTML(HTMLRenderer.NoRenderer);
    }
    [Benchmark(Baseline = true)]
    public string CmpWithArr() {
        using var reader = Getreader();
        var data = new KeyValuePair<string, int>[count];
        var i = 0;
        while (reader.Read()) {
            data[i] = new KeyValuePair<string, int>(reader.GetString(0), reader.GetInt32(1));
            i++;
        }
        var sb = new StringBuilder();
        foreach (var d in data)
            sb.Append("<td>").Append(d.Value).Append("</td>")
              .Append("<td>").Append(d.Key).Append("</td>");
        return sb.ToString();
    }
    [Benchmark]
    public static string CmpWithDT() {
        using var reader = Getreader();
        var dt = DataReaderParser.GetDataTable(Parsers, reader, Template);
        var table = new HTMLElement(Tag.TBL);
        for (int i = 0; i < dt.Count; i++) {
            var row = new HTMLElement(Tag.TR);
            foreach (var prop in Parsers)
                row.AppendChild(prop.Parse(i, dt));
            table.AppendChild(row);
        }
        return table.ToHTML(HTMLRenderer.NoRenderer);
    }
}
*/
/*
| Method   | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
|---------:|---------:|---------:|---------:|------:|----------:|------------:|
| FillList | 51.10 ms | 1.018 ms | 1.912 ms |  1.00 |  13.16 MB |        1.00 |
| FillArr  | 51.14 ms | 1.018 ms | 1.835 ms |  1.00 |  10.69 MB |        0.81 |
| GetDT    | 55.66 ms | 1.188 ms | 3.369 ms |  1.09 |  12.16 MB |        0.92 |

| Method   | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
|--------- |---------:|---------:|---------:|------:|----------:|------------:|
| FillList | 47.69 ms | 1.012 ms | 2.921 ms |  1.01 |  13.16 MB |        1.00 |
| FillArr  | 49.88 ms | 0.970 ms | 1.594 ms |  1.05 |  10.68 MB |        0.81 |
| GetDT    | 38.93 ms | 0.718 ms | 0.855 ms |  0.82 |  12.16 MB |        0.92 |

| Method   | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
|--------- |---------:|---------:|---------:|------:|----------:|------------:|
| FillList | 47.41 ms | 0.732 ms | 0.649 ms |  1.00 |  13.16 MB |        1.00 |
| FillArr  | 50.58 ms | 0.764 ms | 1.020 ms |  1.07 |  10.69 MB |        0.81 |
| GetDT    | 54.99 ms | 1.148 ms | 3.239 ms |  1.16 |  12.16 MB |        0.92 |

| Method     | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
|----------- |---------:|---------:|---------:|------:|----------:|------------:|
| WithArr    | 14.42 ms | 0.288 ms | 0.466 ms |  1.00 |  22.15 MB |        1.00 |
| WithDTOnly | 14.72 ms | 0.291 ms | 0.562 ms |  1.02 |  22.15 MB |        1.00 |
| WithDTCmp  | 15.07 ms | 0.279 ms | 0.518 ms |  1.05 |  22.15 MB |        1.00 |

| Method     | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
|----------- |---------:|---------:|---------:|------:|----------:|------------:|
| WithArr    | 14.39 ms | 0.204 ms | 0.181 ms |  1.00 |  22.15 MB |        1.00 |
| WithDTOnly | 15.27 ms | 0.305 ms | 0.385 ms |  1.06 |  22.15 MB |        1.00 |
| WithDTCmp  | 15.34 ms | 0.296 ms | 0.835 ms |  1.07 |  22.15 MB |        1.00 |

| Method     | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
|----------- |---------:|---------:|---------:|------:|----------:|------------:|
| WithArr    | 14.64 ms | 0.291 ms | 0.427 ms |  1.00 |  22.15 MB |        1.00 |
| WithDTOnly | 16.90 ms | 0.290 ms | 0.271 ms |  1.15 |  22.15 MB |        1.00 |
| WithDTCmp  | 16.93 ms | 0.333 ms | 0.444 ms |  1.16 |  22.15 MB |        1.00 |

| Method     | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
|----------- |---------:|---------:|---------:|------:|----------:|------------:|
| CmpWithArr | 66.41 ms | 1.808 ms | 5.245 ms |  1.01 |  32.83 MB |        1.00 |
| CmpWithDT  | 78.16 ms | 1.851 ms | 5.429 ms |  1.18 |   34.3 MB |        1.04 |

| Method     | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
|----------- |---------:|---------:|---------:|------:|----------:|------------:|
| CmpWithArr | 63.00 ms | 1.248 ms | 2.766 ms |  1.00 |  32.83 MB |        1.00 |
| CmpWithDT  | 69.79 ms | 1.392 ms | 3.643 ms |  1.11 |   34.3 MB |        1.04 |

| Method     | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
|----------- |---------:|---------:|---------:|------:|----------:|------------:|
| CmpWithArr | 66.72 ms | 1.674 ms | 4.883 ms |  1.01 |  32.83 MB |        1.00 |
| CmpWithDT  | 73.31 ms | 2.138 ms | 6.271 ms |  1.10 |   34.3 MB |        1.04 |
*/