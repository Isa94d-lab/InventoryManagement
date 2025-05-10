# InventoryManagement

## Descripción 📌
Este proyecto en C# implementa un sistema de gestión de inventario y flujo de caja utilizando una arquitectura hexagonal. La aplicación permite gestionar productos, compras, ventas, empleados, clientes y proveedores a traves de interfaces intuitivas ademas incluyendo funcionalidades avanzadas como planes promocionales y seguimiento de movimientos de caja.

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
- Gestion de productos, compras y ventas
- Administracion de empleados, cliente y proveedores
- Control flujo de caja
- Interfaces intuitivas
- Conexion con base de datos MySql

## Historial de Commits 📈
<img src="https://github.com/user-attachments/assets/dfc3f42f-44b9-468d-91be-d8e29480dc5f">

## Tecnologias Utilizadas 👾
- Lenguaje: C#
- Base de datos: MySql
- ORM: Conexión directa con MySqlConnector

## Instalacion 📥
1. Prerrequisitos
- Git: Necesitaras Git para clonar el repositorio. Descargalo desde [git](https://git-scm.com/) 
- Un editor de texto como VSCode o cualquier otro de tu preferencia
- MYSQL: Es necesario instalar el programa MYSQL. Descargalo desde [MYSQL](https://dev.mysql.com/downloads/installer/)

2. Codigos en la terminal para instalar el proyecto

- Clonar Repositorio
```bash git clone https://github.com/Isa94d-lab/InventoryManagement.git ```
- Ir al directorio del repositorio
```bash cd InventoryManagement ```

- Instalar la libreria necesaria
```bash dotnet add package MySqlConnector ```

4. Pasos despues de la clonacion del repositorio

- 1. Instalar la extension "SQL" en tu editor de texto <br>
- 2. Abrir la extension anterior y dar click en la opcion de agregar conexion <br>
<img src="https://github.com/user-attachments/assets/fb01eec1-e270-49ac-b861-901a6a8b0230" height="70px">

- 3. Se mostrara la siguiente pagina en la que sera necesario ingresa el Username y Password ingresados durante la instalacion del programa MYSQL. <br>
<img src="https://github.com/user-attachments/assets/73680b8d-5c34-4995-add3-7f235c13e2d9">

- 4. Ejecutar los siguientes 2 archivos ddl.sql(Base de datos) y dml.sql(Insercion de datos) ya sea manualmente o en la terminal.  <br>
<img src="https://github.com/user-attachments/assets/a5c9439f-3505-4e74-b1e9-310f52bb938a" height="100px">

- 5. Ejecutar archivo principal (Program.cs) <br>

## Colaboradores ✒️
### Isabella Stefphani Galvis <br>
### Laura Mariana Vargas