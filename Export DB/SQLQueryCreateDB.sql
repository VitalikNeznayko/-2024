CREATE DATABASE candy_store;
USE candy_store;

CREATE TABLE Clients(
	id_client INT IDENTITY(1,1) PRIMARY KEY,
	first_name VARCHAR(50),
	last_name VARCHAR(50),
	email VARCHAR(60),
	phone_number VARCHAR(15),
	address_client VARCHAR(100),
	city VARCHAR(50),
	postal_code VARCHAR(10)
);

CREATE TABLE Orders(
	id_order INT IDENTITY(1,1) PRIMARY KEY,
	id_client INT,
	order_date DATETIME,
	total_cost decimal(10, 2),
	order_status VARCHAR(20),
	FOREIGN KEY(id_client) REFERENCES Clients(id_client)
);

CREATE TABLE Shipments(
	id_shipment INT IDENTITY(1,1) PRIMARY KEY,
	id_order INT,
	shipment_date DATETIME,
	shipment_method VARCHAR(50),
	tracking_number VARCHAR(100),
	FOREIGN KEY(id_order) REFERENCES Orders(id_order)
);

CREATE TABLE Categories(
	id_category INT IDENTITY(1,1) PRIMARY KEY,
	category_name VARCHAR(50)
);

CREATE TABLE Suppliers(
	id_supplier INT IDENTITY(1,1) PRIMARY KEY,
	supplier_name VARCHAR(100),
	contact_person VARCHAR(50),
	phone_number VARCHAR(15),
	supplier_address VARCHAR(100)
);

CREATE TABLE Products(
	id_product INT IDENTITY(1,1) PRIMARY KEY,
	product_name VARCHAR(100),
	id_category INT,
	id_supplier INT,
	price DECIMAL(10,2),
	quantity INT,
	product_description TEXT
	FOREIGN KEY(id_category) REFERENCES Categories(id_category),
	FOREIGN KEY(id_supplier) REFERENCES Suppliers(id_supplier)
);

CREATE TABLE Reviews(
	id_review INT IDENTITY(1,1) PRIMARY KEY,
	id_client INT,
	id_product INT,
	rating INT,
	review_text TEXT,
	review_date DATETIME,
	FOREIGN KEY(id_client) REFERENCES Clients(id_client),
	FOREIGN KEY(id_product) REFERENCES Products(id_product)
);

CREATE TABLE Order_Items(
	id_order_item INT IDENTITY(1,1) PRIMARY KEY,
	id_order INT,
	id_product INT,
	quantity INT,
	total_amount DECIMAL(10,2)
	FOREIGN KEY(id_order) REFERENCES Orders(id_order),
	FOREIGN KEY(id_product) REFERENCES Products(id_product)
);