--Procedure
--Details
ALTER PROCEDURE GetClientTotalIncome
AS
BEGIN
    SELECT Clients.last_name AS LastName, 
	Clients.first_name AS FirstName, 
	SUM(Order_Items.quantity * Products.price) AS TotalIncome
    FROM Orders
    INNER JOIN Clients ON Clients.id_client = Orders.id_client
    INNER JOIN Order_Items ON Orders.id_order = Order_Items.id_order
    INNER JOIN Products ON Order_Items.id_product = Products.id_product
    GROUP BY Clients.last_name, Clients.first_name;
END;
EXEC GetClientTotalIncome;

ALTER PROCEDURE GetOrdersWithHighestAverageCheck
AS
BEGIN
    SELECT TOP 5 
        o.id_order AS IdOrder, 
        c.first_name + ' ' + c.last_name AS ClientName, 
        AVG(oi.Quantity * p.Price) AS AverageCheck
    FROM Order_Items oi
    JOIN Orders o ON oi.id_order = o.id_order
    JOIN Products p ON oi.id_product = p.id_product
    JOIN Clients c ON o.id_client = c.id_client
    GROUP BY o.id_order, c.first_name, c.last_name
    ORDER BY AverageCheck DESC;
END
EXEC GetOrdersWithHighestAverageCheck
--Statistics
--1
ALTER PROCEDURE GetTotalSoldByRatingForCategory
    @chooseRating INT
AS
BEGIN
    SELECT 
		Reviews.rating AS ChooseRating,
        Categories.category_name AS CategoryName, 
        SUM(Order_Items.quantity) AS TotalSold
    FROM Products
    INNER JOIN Order_Items ON Products.id_product = Order_Items.id_product
    INNER JOIN Categories ON Products.id_category = Categories.id_category
    INNER JOIN Reviews ON Products.id_product = Reviews.id_product
    WHERE Reviews.rating = @chooseRating
    GROUP BY Categories.category_name, Reviews.rating;
END;

--2
ALTER PROCEDURE GetTotalSoldByRatingForProducts
    @chooseRating INT
AS
BEGIN
    SELECT 
        Reviews.rating AS ChooseRating,
        Products.product_name AS ProductName, 
        SUM(Order_Items.quantity) AS TotalSold,
        SUM(Order_Items.quantity * Products.price) AS TotalAmount
    FROM Products
    INNER JOIN Order_Items ON Products.id_product = Order_Items.id_product
    INNER JOIN Reviews ON Products.id_product = Reviews.id_product
    WHERE Reviews.rating = @chooseRating
    GROUP BY Products.product_name, Reviews.rating
    ORDER BY TotalSold DESC;
END;
EXEC GetTotalSoldByRatingForProducts  @chooseRating = 5;

--3
ALTER PROCEDURE GetTopSoldProductsByCategory
    @Year INT
AS
BEGIN
    SELECT TOP 5 
        Categories.category_name AS CategoryName, 
        Products.product_name AS ProductName, 
        SUM(Order_Items.quantity * Products.price) AS TotalAmount
    FROM Order_Items
    INNER JOIN Products ON Order_Items.id_product = Products.id_product
    INNER JOIN Categories ON Products.id_category = Categories.id_category
    INNER JOIN Orders ON Orders.id_order = Order_Items.id_order
    WHERE YEAR(Orders.order_date) = @Year
    GROUP BY Categories.category_name, Products.product_name
    ORDER BY TotalAmount DESC;
END;
EXEC GetTopSoldProductsByCategory @Year = 2024;

--4
ALTER PROCEDURE GetDetailedRevenueByDateRange
    @StartDate DATETIME,
    @EndDate DATETIME
AS
BEGIN
    SELECT 
        SUM(o.total_cost) AS TotalRevenue,
        COUNT(o.id_order) AS TotalOrders, 
        AVG(o.total_cost) AS AverageOrderValue,
        c.category_name AS CategoryName, 
        SUM(oi.Quantity * p.Price) AS RevenueByCategory 
    FROM Orders o
    JOIN Order_Items oi ON o.id_order = oi.id_order
    JOIN Products p ON oi.id_product = p.id_product
    JOIN Categories c ON p.id_category = c.id_category
    WHERE o.order_date BETWEEN @StartDate AND @EndDate
    GROUP BY c.category_name
    ORDER BY RevenueByCategory DESC;
END

EXEC GetDetailedRevenueByDateRange '2024-01-01', '2024-12-31';
--Triger
--task4
CREATE TRIGGER trg_UpdateProductStockAfterPurchase
ON Order_Items
AFTER INSERT
AS
BEGIN
    DECLARE @product_id INT, @quantity INT;

    SELECT @product_id = id_product, @quantity = quantity
    FROM INSERTED;

    UPDATE Products
    SET Products.quantity = Products.quantity - @quantity
    WHERE id_product = @product_id;
END;

SELECT id_product, product_name, quantity
FROM Products;

INSERT INTO Order_Items (id_order, id_product, quantity)
VALUES (10, 1, 3);

SELECT id_product, product_name, quantity
FROM Products;

--task5
CREATE TRIGGER trg_CalculateOrderTotal
ON Order_Items
AFTER INSERT
AS
BEGIN
    DECLARE @order_id INT;

    SELECT @order_id = id_order FROM INSERTED;

    UPDATE oi
    SET oi.total_amount = oi.quantity * p.price
    FROM Order_Items oi
    INNER JOIN Products p ON oi.id_product = p.id_product
    WHERE oi.id_order = @order_id;

    UPDATE Orders
    SET total_cost = (SELECT SUM(oi.quantity * p.price) 
                      FROM Order_Items oi
                      INNER JOIN Products p ON oi.id_product = p.id_product
                      WHERE oi.id_order = @order_id)
    WHERE id_order = @order_id;
END;

SELECT * FROM Order_Items WHERE id_order = 10;
SELECT * FROM Orders WHERE id_order = 10;

INSERT INTO Order_Items (id_order, id_product, quantity)
VALUES (10, 1, 3);

SELECT * FROM Order_Items WHERE id_order = 10;
SELECT * FROM Orders WHERE id_order = 10;
--task6
ALTER TRIGGER trg_CheckProductStockBeforeOrder
ON Order_Items
AFTER INSERT
AS
BEGIN
    -- Перевірка на наявність перевищення замовленої кількості над доступним запасом
     IF EXISTS (
        SELECT 1
        FROM INSERTED i
        JOIN Products p ON i.id_product = p.id_product
        WHERE p.quantity - i.quantity < 0
    )
    BEGIN
        -- Виведення повідомлення про помилку
        THROW 50001, 'Помилка! Недостатня кількість продукту на складі.', 1;

        -- Відкат транзакції
        ROLLBACK TRANSACTION;
    END
END;


INSERT INTO Order_Items (id_order, id_product, quantity)
VALUES (8, 12, 5)