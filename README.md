<img width="444" height="451" alt="{FD0057B4-0351-44C8-80A9-4DFCED706D7D}" src="https://github.com/user-attachments/assets/8b698f46-46c3-470d-8032-a21dfd56265c" />

Таблицы:
- Пользователи (id, логин, пароль, роль, заблокирован, дата_создания)
- Продукция (id, код, наименование, ед_изм, цена)
- Материалы (id, код, наименование, ед_изм, цена)
- Спецификация (id, продукция_id, материал_id, количество)
- Заказчики (id, наименование, инн, адрес, телефон, продавец, покупатель)
- Заказы (id, заказчик_id, номер_заказа, дата_заказа)
- Заказы_Строки (id, заказ_id, продукция_id, количество, цена)

- Users (id, login, password, role, blocked, create_date)
- Product (id, code, name, unit, price)
- Material (id, code, name, unit,price)
- Specification (id, product_id, material_id, unit)
- Customers (id, name, inn, adress, phone, seller, buyer)
- Order (id, customers_id, number_id, order_date)
- Order_line (id, order_id, product_id, unit, price)

Связи:
- Продукция 1 : M Спецификация
- Материалы 1 : M Спецификация
- Заказчики 1 : M Заказы
- Заказы 1 : M Заказы_Строки
- Продукция 1 : M Заказы_Строки
