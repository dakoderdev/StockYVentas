-- =========================================
-- CREACIÓN DE BASE DE DATOS
-- =========================================
DROP DATABASE IF EXISTS ElectrodomesticosDB;
CREATE DATABASE ElectrodomesticosDB;
USE ElectrodomesticosDB;

-- =========================================
-- TABLA: Sucursal
-- =========================================
CREATE TABLE Sucursal (
    IdSucursal INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);

-- =========================================
-- TABLA: Producto (tabla base)
-- =========================================
CREATE TABLE Producto (
    IdProducto INT AUTO_INCREMENT PRIMARY KEY,
    Codigo INT NOT NULL UNIQUE,
    Nombre VARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL,
    TipoProducto ENUM('Televisor', 'Heladera', 'Lavarropas') NOT NULL,
    IdSucursal INT NOT NULL,
    FOREIGN KEY (IdSucursal) REFERENCES Sucursal(IdSucursal)
);

-- =========================================
-- TABLA: Televisor
-- =========================================
CREATE TABLE Televisor (
    IdProducto INT PRIMARY KEY,
    Pulgadas INT NOT NULL,
    TipoPantalla VARCHAR(50) NOT NULL,
    FOREIGN KEY (IdProducto) REFERENCES Producto(IdProducto)
        ON DELETE CASCADE
);

-- =========================================
-- TABLA: Heladera
-- =========================================
CREATE TABLE Heladera (
    IdProducto INT PRIMARY KEY,
    CapacidadLitros INT NOT NULL,
    Tipo VARCHAR(50) NOT NULL,
    FOREIGN KEY (IdProducto) REFERENCES Producto(IdProducto)
        ON DELETE CASCADE
);

-- =========================================
-- TABLA: Lavarropas
-- =========================================
CREATE TABLE Lavarropas (
    IdProducto INT PRIMARY KEY,
    CargaKg INT NOT NULL,
    Tipo VARCHAR(50) NOT NULL,
    FOREIGN KEY (IdProducto) REFERENCES Producto(IdProducto)
        ON DELETE CASCADE
);

-- =========================================
-- TABLA: Venta
-- =========================================
CREATE TABLE Venta (
    IdVenta INT AUTO_INCREMENT PRIMARY KEY,
    Fecha DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IdSucursal INT NOT NULL,
    FOREIGN KEY (IdSucursal) REFERENCES Sucursal(IdSucursal)
);

-- =========================================
-- TABLA: DetalleVenta
-- =========================================
CREATE TABLE DetalleVenta (
    IdDetalle INT AUTO_INCREMENT PRIMARY KEY,
    IdVenta INT NOT NULL,
    IdProducto INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (IdVenta) REFERENCES Venta(IdVenta)
        ON DELETE CASCADE,
    FOREIGN KEY (IdProducto) REFERENCES Producto(IdProducto)
);

-- =========================================
-- DATOS INICIALES
-- =========================================

-- Sucursales
INSERT INTO Sucursal (Nombre) VALUES 
('Centro'),
('Norte');