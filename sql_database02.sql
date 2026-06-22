USE PizzaProduction;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ImportCustomers')
    DROP PROCEDURE sp_ImportCustomers;
GO

CREATE PROCEDURE sp_ImportCustomers
    @JsonPath NVARCHAR(255)
AS
BEGIN
    DECLARE @JSON NVARCHAR(MAX);
    
    SELECT @JSON = BulkColumn 
    FROM OPENROWSET(BULK @JsonPath, SINGLE_BLOB) AS j;
    
    TRUNCATE TABLE Заказчики;
    
    INSERT INTO Заказчики (id, наименование, инн, адрес, телефон, продавец, покупатель)
    SELECT 
        id,
        name,
        inn,
        addres,
        phone,
        salesman,
        buyer
    FROM OPENJSON(@JSON)
    WITH (
        id NVARCHAR(20) '$.id',
        name NVARCHAR(100) '$.name',
        inn NVARCHAR(20) '$.inn',
        addres NVARCHAR(200) '$.addres',
        phone NVARCHAR(20) '$.phone',
        salesman BIT '$.salesman',
        buyer BIT '$.buyer'
    );
END;
GO

-- Очистка перед вставкой
DELETE FROM Спецификация;
DELETE FROM Материалы;
DELETE FROM Продукция;
GO

-- Сброс счетчиков
DBCC CHECKIDENT ('Продукция', RESEED, 0);
DBCC CHECKIDENT ('Материалы', RESEED, 0);
DBCC CHECKIDENT ('Спецификация', RESEED, 0);
GO

INSERT INTO Продукция (код, наименование, ед_изм, цена_продажи) VALUES
('НФ-00000002', 'Пицца "Пеперони" 33см.', 'шт', 850),
('НФ-00000003', 'Пицца "Маргарита" 33см.', 'шт', 710),
('НФ-00000004', 'Пицца "Гавайская" 30см.', 'шт', 420),
('НФ-00000005', 'Пицца "Морская" 30см.', 'шт', 640);
GO

INSERT INTO Материалы (код, наименование, ед_изм, цена) VALUES
('НФ-00000009', 'Куриное яйцо', 'шт', 85),
('НФ-00000010', 'Майонез', 'кг', 210),
('НФ-00000011', 'Бекон', 'кг', 200),
('НФ-00000012', 'Тесто', 'шт', 400),
('НФ-00000013', 'Сыр чеддер', 'шт', 960),
('НФ-00000014', 'Сыр пармезан', 'шт', 890),
('НФ-00000015', 'Томаты черри', 'кг', 350);
GO

INSERT INTO Спецификация (продукция_id, материал_id, количество)
SELECT 
    p.id, 
    m.id, 
    CASE m.наименование
        WHEN 'Куриное яйцо' THEN 2.0
        WHEN 'Майонез' THEN 0.03
        WHEN 'Бекон' THEN 0.2
        WHEN 'Тесто' THEN 0.5
        WHEN 'Сыр чеддер' THEN 0.15
        WHEN 'Сыр пармезан' THEN 0.1
        WHEN 'Томаты черри' THEN 0.2
    END
FROM Продукция p
CROSS JOIN Материалы m
WHERE p.наименование = 'Пицца "Пеперони" 33см.';
GO