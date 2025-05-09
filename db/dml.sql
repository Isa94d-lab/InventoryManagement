
INSERT INTO pais (nombre) VALUES 
('Colombia'),
('México'),
('Estados Unidos'),
('España'),
('Argentina');


INSERT INTO region (nombre, pais_id) VALUES 
('Cundinamarca', 1),
('Antioquia', 1),
('Jalisco', 2),
('California', 3),
('Madrid', 4),
('Buenos Aires', 5);


INSERT INTO ciudad (nombre, region_id) VALUES 
('Bogotá', 1),
('Medellín', 2),
('Guadalajara', 3),
('Los Angeles', 4),
('Madrid', 5),
('Buenos Aires', 6);


INSERT INTO empresa (id, nombre, ciudad_id, fecha_registro) VALUES 
('E001', 'TechnoSoft', 1, '2022-01-15'),
('E002', 'Innovatech', 2, '2022-02-20'),
('E003', 'Global Services', 3, '2022-03-10'),
('E004', 'DataSystems', 4, '2022-04-05'),
('E005', 'Soluciones Integradas', 5, '2022-05-12');

INSERT INTO facturacion (fechaResolucion, numInicio, numFinal, factura_actual) VALUES 
('2022-01-01', 1000, 2000, 1000),
('2022-02-01', 2001, 3000, 2001),
('2022-03-01', 3001, 4000, 3001),
('2022-04-01', 4001, 5000, 4001),
('2022-05-01', 5001, 6000, 5001);

INSERT INTO plan (nombre, fecha_inicio, fecha_fin, descuento) VALUES 
('Plan Básico', '2022-01-01', '2022-12-31', 5.00),
('Plan Premium', '2022-01-01', '2022-12-31', 10.00),
('Plan Empresarial', '2022-01-01', '2022-12-31', 15.00),
('Plan VIP', '2022-01-01', '2022-12-31', 20.00),
('Plan Temporal', '2022-06-01', '2022-08-31', 7.50);

INSERT INTO producto (id, nombre, stock, stockMin, stockMax, fecha_creacion, fecha_actualizacion, codigo_barra) VALUES 
('P001', 'Laptop HP', 50, 10, 100, '2022-01-10', '2022-05-01', 'HPLP12345'),
('P002', 'Monitor Dell', 30, 5, 50, '2022-01-15', '2022-05-01', 'DLMN54321'),
('P003', 'Teclado Logitech', 100, 20, 200, '2022-01-20', '2022-05-01', 'LGTC98765'),
('P004', 'Mouse Inalámbrico', 80, 15, 150, '2022-01-25', '2022-05-01', 'MSIN45678'),
('P005', 'Disco Duro 1TB', 40, 8, 80, '2022-02-01', '2022-05-01', 'HDDTB78901');

INSERT INTO plan_producto (plan_id, producto_id) VALUES 
(1, 'P001'),
(1, 'P002'),
(2, 'P001'),
(2, 'P003'),
(3, 'P004'),
(3, 'P005'),
(4, 'P001'),
(4, 'P002'),
(4, 'P003'),
(5, 'P005');

INSERT INTO tipoMovCaja (nombre, tipoMovimiento) VALUES 
('Venta', 'Entrada'),
('Pago a Proveedor', 'Salida'),
('Adelanto Cliente', 'Entrada'),
('Pago de Servicios', 'Salida'),
('Pago de Nómina', 'Salida');

INSERT INTO tipo_documento (descripcion) VALUES 
('Cédula de Ciudadanía'),
('Pasaporte'),
('Tarjeta de Identidad'),
('Cédula de Extranjería'),
('DNI');

INSERT INTO tipo_tercero (descripcion) VALUES 
('Cliente'),
('Proveedor'),
('Empleado'),
('Cliente-Proveedor'),
('Distribuidor');

INSERT INTO tercero (id, nombre, apellidos, email, tipo_documento_id, tipo_tercero_id, ciudad_id) VALUES 
('T001', 'Juan', 'Pérez', 'juan.perez@email.com', 1, 1, 1),
('T002', 'María', 'Gómez', 'maria.gomez@email.com', 1, 2, 2),
('T003', 'Carlos', 'Rodríguez', 'carlos.rodriguez@email.com', 2, 3, 3),
('T004', 'Ana', 'Martínez', 'ana.martinez@email.com', 3, 4, 4),
('T005', 'Pedro', 'López', 'pedro.lopez@email.com', 5, 5, 5),
('T006', 'Laura', 'Díaz', 'laura.diaz@email.com', 1, 1, 1),
('T007', 'Roberto', 'Sánchez', 'roberto.sanchez@email.com', 2, 2, 2);

INSERT INTO cliente (tercero_id, fecha_nacimiento, fecha_compra) VALUES 
('T001', '1985-05-10', '2022-01-20'),
('T004', '1990-08-15', '2022-02-15'),
('T006', '1978-12-03', '2022-03-05'),
('T005', '1995-04-22', '2022-04-10'),
('T007', '1982-07-30', '2022-05-08');

INSERT INTO proveedor (tercero_id, descuento, dia_pago) VALUES 
('T002', 5.0, 15),
('T004', 7.5, 20),
('T005', 10.0, 25),
('T007', 8.0, 10),
('T003', 6.0, 5);

INSERT INTO eps (nombre) VALUES 
('Salud Total'),
('Sura EPS'),
('Compensar'),
('Nueva EPS'),
('Sanitas');

INSERT INTO arl (nombre) VALUES 
('Positiva'),
('Sura ARL'),
('Colmena'),
('Seguros Bolívar'),
('Liberty');

INSERT INTO tercero_telefono (numero, tercero_id, tipo_telefono) VALUES 
('3101234567', 'T001', 'Movil'),
('6013214567', 'T001', 'Fijo'),
('3209876543', 'T002', 'Movil'),
('3154567890', 'T003', 'Movil'),
('6024567890', 'T004', 'Fijo'),
('3112345678', 'T005', 'Movil');

INSERT INTO empleado (tercero_id, fecha_ingreso, salario_base, eps_id, arl_id) VALUES 
('T003', '2022-01-15', 2500000, 1, 1),
('T001', '2022-02-01', 2800000, 2, 2),
('T004', '2022-02-15', 3000000, 3, 3),
('T005', '2022-03-01', 2700000, 4, 4),
('T007', '2022-03-15', 3200000, 5, 5);

INSERT INTO movimientoCaja (fecha, tipoMovimiento_id, valor, concepto, tercero_id) VALUES 
('2022-01-20', 1, 500000, 'Venta de productos', 'T001'),
('2022-02-05', 2, 300000, 'Pago a proveedor', 'T002'),
('2022-03-10', 3, 200000, 'Adelanto de cliente', 'T004'),
('2022-04-15', 4, 150000, 'Pago de servicios', 'T003'),
('2022-05-20', 5, 2500000, 'Pago de nómina', 'T003');

INSERT INTO compra (terceroProveedor_id, fecha, terceroEmpleado_id, DocCompra) VALUES 
('T002', '2022-01-10', 'T003', 'COMP-001'),
('T004', '2022-02-15', 'T003', 'COMP-002'),
('T005', '2022-03-20', 'T003', 'COMP-003'),
('T007', '2022-04-25', 'T003', 'COMP-004'),
('T002', '2022-05-30', 'T003', 'COMP-005');

INSERT INTO detalle_compra (fecha, producto_id, cantidad, valor, compra_id) VALUES 
('2022-01-10', 'P001', 10, 1500000, 1),
('2022-01-10', 'P002', 5, 400000, 1),
('2022-02-15', 'P003', 20, 200000, 2),
('2022-03-20', 'P004', 15, 150000, 3),
('2022-04-25', 'P005', 8, 640000, 4),
('2022-05-30', 'P001', 5, 750000, 5);

INSERT INTO venta (factura_id, fecha, terceroEmpleado_id, terceroCliente_id) VALUES 
(1000, '2022-01-20', 'T003', 'T001'),
(1001, '2022-02-25', 'T003', 'T004'),
(1002, '2022-03-30', 'T003', 'T006'),
(1003, '2022-04-15', 'T003', 'T005'),
(1004, '2022-05-20', 'T003', 'T001');

INSERT INTO detalle_venta (factura_id, producto_id, cantidad, valor) VALUES 
(1000, 'P001', 2, 350000),
(1000, 'P002', 1, 100000),
(1001, 'P003', 5, 75000),
(1002, 'P004', 3, 45000),
(1003, 'P005', 2, 200000),
(1004, 'P001', 1, 175000);

INSERT INTO producto_proveedor (tercero_id, producto_id) VALUES 
('T002', 'P001'),
('T002', 'P002'),
('T004', 'P003'),
('T005', 'P004'),
('T007', 'P005'),
('T004', 'P001'),
('T007', 'P002');