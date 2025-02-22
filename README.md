# TaskManager

Proyecto de Gestion de Tareas del curso C# Avanzado del ITLA

API diseñada para el manejo de tareas

Etapa 1 - 25/01/2025

Configuracion de archivos del proyecto, validacion de entrada y salida, configuracion de la Base de Datos,
Añadir comentarios a cada clase para saber su funcionamiento, creacion de los primeros endpoints

Etapa 2 - 30/01/2025

Agregacion de delegados para validacion de tareas y calculo de tiempo restante, asi como un endpoint para verificar el tiempo restante de una tarea

Etapa 3 - 07/02/2025

Creacion de la Fabrica para manejo de creacion de tareas, tanto de baja como alta prioridad, 
Agregacion de 2 endpoints para la creacion de tareas modular, solo requiriendo una descripcion y los demas datos vayan ya integrados,
Asi como reestructuracion a la clase ITask para futuras expansiones, para no seguir dependiendo de ICommonsProcess

Etapa 4 - 15/02/2025 

Implementacion de Rx.Net a TaskServices para el manejo de tareas de creacion, actualizacion y borrado, creando la clase ReactiveTaskQueue
para los procesos necesarios
