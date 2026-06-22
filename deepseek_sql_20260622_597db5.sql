USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'PizzaProduction')
BEGIN
    ALTER DATABASE PizzaProduction SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PizzaProduction;
END
GO

CREATE DATABASE PizzaProduction;
GO

USE PizzaProduction;
GO

CREATE TABLE Пользователи (
    id INT IDENTITY(1,1) PRIMARY KEY,
    логин NVARCHAR(50) UNIQUE NOT NULL,
    пароль NVARCHAR(255) NOT NULL,
    роль NVARCHAR(20) NOT NULL CHECK (роль IN ('Администратор', 'Пользователь')),
    заблокирован BIT DEFAULT 0,
    дата_создания DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Продукция (
    id INT IDENTITY(1,1) PRIMARY KEY,
    код NVARCHAR(20) UNIQUE NOT NULL,
    наименование NVARCHAR(100) NOT NULL,
    ед_изм NVARCHAR(10) NOT NULL,
    цена_продажи DECIMAL(10,2) NOT NULL DEFAULT 0
);
GO

CREATE TABLE Материалы (
    id INT IDENTITY(1,1) PRIMARY KEY,
    код NVARCHAR(20) UNIQUE NOT NULL,
    наименование NVARCHAR(100) NOT NULL,
    ед_изм NVARCHAR(10) NOT NULL,
    цена DECIMAL(10,2) NOT NULL DEFAULT 0
);
GO

CREATE TABLE Спецификация (
    id INT IDENTITY(1,1) PRIMARY KEY,
    продукция_id INT NOT NULL,
    материал_id INT NOT NULL,
    количество DECIMAL(10,3) NOT NULL,
    FOREIGN KEY (продукция_id) REFERENCES Продукция(id) ON DELETE CASCADE,
    FOREIGN KEY (материал_id) REFERENCES Материалы(id) ON DELETE CASCADE,
    CONSTRAINT UC_Спецификация UNIQUE (продукция_id, материал_id)
);
GO

CREATE TABLE Заказчики (
    id NVARCHAR(20) PRIMARY KEY,
    наименование NVARCHAR(100) NOT NULL,
    инн NVARCHAR(20) NULL,
    адрес NVARCHAR(200) NULL,
    телефон NVARCHAR(20) NULL,
    продавец BIT DEFAULT 0,
    покупатель BIT DEFAULT 0
);
GO

CREATE TABLE Заказы (
    id INT IDENTITY(1,1) PRIMARY KEY,
    заказчик_id NVARCHAR(20) NOT NULL,
    номер_заказа NVARCHAR(20) NOT NULL,
    дата_заказа DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (заказчик_id) REFERENCES Заказчики(id)
);
GO

CREATE TABLE Заказы_Строки (
    id INT IDENTITY(1,1) PRIMARY KEY,
    заказ_id INT NOT NULL,
    продукция_id INT NOT NULL,
    количество INT NOT NULL,
    цена DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (заказ_id) REFERENCES Заказы(id) ON DELETE CASCADE,
    FOREIGN KEY (продукция_id) REFERENCES Продукция(id)
);
GO