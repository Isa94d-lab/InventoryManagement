# InventoryManagement

## Descripción 📌
InventoryManagement es una aplicación de consola desarrollada en C# que implementa un sistema de gestión de inventario y flujo de caja, siguiendo la arquitectura hexagonal y los principios SOLID.
Permite administrar productos, compras, ventas, empleados, clientes y proveedores a través de interfaces intuitivas, además de ofrecer funcionalidades avanzadas como planes promocionales y seguimiento de movimientos de caja.

## Estructura del Proyecto 🏗️
El proyecto sigue una estructura clara basada en la arquitectura hexagonal:
```bash
InventoryManagementSystem/
├── Application/
│   ├── Config/
│   └── Services/UI/
├── Domain/
│   ├── Entities/
│   ├── Factory/
│   └── Ports/
├── Infrastructure/
│   ├── Configuration/
│   ├── MySql/
│   └── Repositories/
├── db/
│   ├── ddl.sql
│   └── dml.sql
├── Program.cs
└── README.md
```

## Funcionalidades Principales ✅
- Gestion de productos, compras y ventas.
- Administracion de empleados, cliente y proveedores.
- Control flujo de caja.
- Interfaces intuitivas.
- Conexion con base de datos MySql.

## Historial de Commits 📈
<img src="https://github.com/user-attachments/assets/dfc3f42f-44b9-468d-91be-d8e29480dc5f">

## Tecnologias Utilizadas 👾
- Lenguaje: C#
- Base de datos: MySql
- ORM: Conexión directa con MySqlConnector

## Instalación 📥
### 🔧 Requisitos Previos
- Tener instalado [Git](https://git-scm.com/)
- Tener instalado [MYSQL](https://dev.mysql.com/downloads/installer/)
- Un editor de código como Visual Studio Code, Visual Studio, o el de tu preferencia.
- SDK de .NET

### 🚀 Pasos de ejecución

1. Clonar Repositorio

```bash git clone https://github.com/Isa94d-lab/InventoryManagement.git ```

2. Ir al directorio del repositorio
```bash cd InventoryManagement ```

3. Instalar la libreria necesaria para la conexión a la base de datos
```bash dotnet add package MySqlConnector ```

### Configuración Base de Datos:

- Abre tu editor de texto y asegúrate de tener instalada la extensión SQL.

- Agrega una nueva conexión usando tus credenciales de MySQL:

<img src="https://github.com/user-attachments/assets/fb01eec1-e270-49ac-b861-901a6a8b0230" height="70px">

- Ingresa el usuario y contraseña establecidos durante la instalación:

<img src="https://github.com/user-attachments/assets/73680b8d-5c34-4995-add3-7f235c13e2d9">

- Ejecuta los scripts para crear la base de datos y poblarla:
    - [DDL: Estructura](./db/ddl.sql)
    - [DML: Inserción de datos](./db/dml.sql)

<img src="https://github.com/user-attachments/assets/a5c9439f-3505-4e74-b1e9-310f52bb938a" height="100px">

### Ejecución:
```
dotnet run
```

## Colaboradores ✒️
### Isabella Stefphani Galvis <br>
### Laura Mariana Vargas