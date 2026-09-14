# Goji2 : Electric Boogaloo

*Ultima actualización 14 de Septiembre

Goji es una plataforma de gestion de proyectos orientados a Waterfall y Agile.

Goji2 propone una arquitectura mas limpia implementando un Delegate layer y testeos unitarios para poder validar el sistema. 

- API rest con Controller -> Delegate -> Repo
- Implementacion limpia de interfaces para User, Task y Project
- Repositorio basado en SqLite

todo:
- Migrar db a postgres en contenedor
- Agregar autenticacion de usuarios
- Agregar personalizacion de proyectos en base a necesidad
- Agregar sistema de alertas para deadlines

