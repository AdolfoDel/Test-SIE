# Test SIE
Este es un sistema de gestión de datos para Usuarios y Vehículos, desarrollado en C# (Windows Forms) y conectado a una base de datos SQL Server.

Características Principales
Conexión a Base de Datos: Utiliza ADO.NET (SqlConnection, SqlDataAdapter) para interactuar con la base de datos local.

Visualización y Edición de Datos: Muestra los datos de USUARIOS1 y VEHICULOS en un DataGridView. Los cambios realizados directamente en la cuadrícula se guardan automáticamente en la base de datos (mediante la implementación del CellEndEdit y SqlDataAdapter.Update).

Módulos de Formulario: Permite la inserción de nuevos usuarios y vehículos a través de formularios, con validaciones (nombre/apellido para usuarios, VIN para vehículos) y verificación de existencia de ID de propietario.
