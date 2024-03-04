select * from ROL
/*
insert into ROL (Descripcion) values ('administrador')

insert into ROL (Descripcion) values ('empleado')
*/

select *  from USUARIO
/*
insert into USUARIO (Documento, NombreCompleto, Correo, Clave, IdRol, Estado)values ('1234','pepe','pepe@gmail.com','12345',1,1)
insert into USUARIO (Documento, NombreCompleto, Correo, Clave, IdRol, Estado)values
('20','EMPLEADO','@gmail.com','123456',2,1)
*/


select IdUsuario,Documento, NombreCompleto,Correo, Clave, Estado from usuario


select p.IdRol, NombreMenu from PERMISO p 
inner join ROL r on r.IdRol= p.IdRol
inner join USUARIO u on u.IdRol = r.IdRol
where u.IdUsuario = 1
/*
insert into PERMISO(IdRol,NombreMenu) values
(1,'menuUsuarios'),
(1,'menuMantenedor'),
(1,'menuVentas'),
(1,'menuCompras'),
(1,'menuClientes'),
(1,'menuProveedores'),
(1,'menuReportes'),
(1,'menuAcercaDe')


insert into PERMISO(IdRol, NombreMenu) values
(2,'menuVentas'),
(2,'menuCompras'),
(2,'menuClientes'),
(2,'menuProveedores'),
(2,'menuAcercaDe')

*/



