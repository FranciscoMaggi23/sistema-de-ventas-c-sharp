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


select IdCategoria, Descripcion, Estado from CATEGORIA


insert into CATEGORIA(Descripcion,Estado) values ('Lacteos',1)
insert into CATEGORIA(Descripcion,Estado) values ('Embutidos',1)
insert into CATEGORIA(Descripcion,Estado) values ('Enlatados',1)


--update CATEGORIA set Estado =1

select IdCliente, Documento, NombreCompleto, Correo, Telefono, Estado from CLIENTE


select * from COMPRA where NumeroDocumento='00001'
select * from DETALLE_COMPRA where IdCompra = 1


--detalle de la compra--
select c.IdCompra,
u.NombreCompleto,
pr.Documento, pr.RazonSocial,
c.TipoDocumento, c.NumeroDocumento,
c.MontoTotal,convert(char(10),c.FechaRegistro,103)[FechaRegistro]
from COMPRA c inner join USUARIO u on
u.IdUsuario=c.IdUsuario inner join PROVEEDOR pr on pr.IdProveedor = c.IdProveedor
where c.NumeroDocumento = '00001'


--productos relacionados a una compra--

select p.Nombre, dc.PrecioCompra, dc.Cantidad, dc.MontoTotal
from DETALLE_COMPRA dc
inner join PRODUCTO p on p.IdProducto = dc.IdProducto
where dc.IdCompra = 1

select * from PRODUCTO
update PRODUCTO set Stock = 45 where IdProducto=1
--update PRODUCTO set Stock = Stock-@cantidad where IdProducto = @idproducto

select v.IdVenta, u.NombreCompleto, v.DocumentoCliente,
v.NombreCliente, v.TipoDocumento, v.NumeroDocumento,
v.MontoPago, v.MontoCambio, v.MontoTotal,convert(char(10), v.FechaRegistro,103)[FechaRegistro]
from VENTA v inner join
USUARIO u on u.IdUsuario = v.IdUsuario where
v.NumeroDocumento = '00001'


select p.Nombre, dv.PrecioVenta, dv.Cantidad, dv.SubtTotal
from DETALLE_VENTA dv inner join 
PRODUCTO p on p.IdProducto =dv.IdProducto
where dv.IdVenta = 1

















