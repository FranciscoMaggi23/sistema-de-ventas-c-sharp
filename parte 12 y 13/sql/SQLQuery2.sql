---mantenedor de usuario---

--recibe valores/parametros que van a ser ingresados en cada uno de nuestros campos que tenemos en las tablas

CREATE PROC SP_REGISTRARUSUARIO(
@Documento varchar (50),
@NombreCompleto varchar (100),
@Correo varchar (100),
@Clave varchar (100),
@IdRol int,
@Estado bit,
--parametros de salida
@IdUsuarioResultado int output,
@Mensaje varchar (500) output
)
as
begin
     set @IdUsuarioResultado =0
	 set @Mensaje =''

	 if not exists(select * from USUARIO where Documento =@Documento)
	 begin
		  insert into USUARIO(Documento, NombreCompleto, Correo, Clave, IdRol, Estado) values
		  (@Documento, @NombreCompleto,@Correo,@Clave,@IdRol,@Estado)                                 
           
		   set @IdUsuarioResultado = SCOPE_IDENTITY()
		   
      end
	  else
		   set @Mensaje = 'no se puede repetir el documento para mas de un usuario'	

end;

--declare @idusuariogenerado int
--declare @mensaje varchar (500)

--exec SP_REGISTRARUSUARIO '123', 'pruebas','test@gmail.com','456',2,1, @idusuariogenerado output, @mensaje output

--select @idusuariogenerado
--select @mensaje
go

--editar usuario
CREATE PROC SP_EDITARUSUARIO(
@IdUsuario int,
@Documento varchar (50),
@NombreCompleto varchar (100),
@Correo varchar (100),
@Clave varchar (100),
@IdRol int,
@Estado bit,
@Respuesta bit output,
@Mensaje varchar (500) output
)
as
begin
     set @Respuesta =0
	 set @Mensaje =''

	 if NOT EXISTS(select * from USUARIO where Documento = @Documento and IdUsuario != @IdUsuario)          
	 begin
		  update USUARIO set
		  Documento = @Documento, 
		  NombreCompleto = @NombreCompleto, 
		  Correo =@Correo, 
		  Clave =@Clave, 
		  IdRol = @IdRol, 
		  Estado = @Estado
		  where IdUsuario = @IdUsuario
		  
		  set @Respuesta = 1
		   
      end
	  else
		   set @Mensaje = 'no se puede repetir el documento para mas de un usuario'	

end


--editar
--declare @respuesta bit
--declare @mensaje varchar (500)

--exec SP_EDITARUSUARIO 3, '20', 'pruebas 3','test@gmail.com','456',2,0, @respuesta output, @mensaje output

--select @respuesta

--select @mensaje

--select * from USUARIO


--select * from Usuario where Documento = 123 and IdUsuario != 2

go

--eliminar usuario
create PROC SP_ELIMINARUSUARIO(
@IdUsuario int,
@Respuesta bit output,
@Mensaje varchar (500) output
)
as
begin
     set @Respuesta =0
	 set @Mensaje =''
	 declare @pasoregla bit = 1


	 if EXISTS (select * from COMPRA C inner join USUARIO U
	 ON U.IdUsuario = C.IdUsuario where U.IdUsuario= @IdUsuario
	 )
	 BEGIN
	 	   set @pasoregla = 0
		   set @Respuesta =0
		   set @Mensaje = @Mensaje + 'no se puede repetir el documento para mas de un usuario'
	 end


	 if EXISTS(select * from VENTA V inner join USUARIO U
	 ON U.IdUsuario = V.IdUsuario where U.IdUsuario= @IdUsuario
	 )          
	 begin
		  set @pasoregla = 0
		  set @Respuesta = 0
		  set @Mensaje = @Mensaje + 'no se puede repetir el documento para mas de un usuario'
	 end
	 
	 if(@pasoregla = 1)
	 begin
		  delete from USUARIO where IdUsuario = @IdUsuario
		  set @Respuesta = 1
	 end

end



--declare @respuesta bit
--declare @mensaje varchar (500)

--exec SP_ELIMINARUSUARIO 4, @respuesta output, @mensaje output

--select @respuesta

--select @mensaje

--select * from USUARIO


go


/*---------------procedimientos para categoria---------------*/

--procedimiento para guardar Categoria
create proc SP_RegistrarCategoria(
@Descripcion varchar(50),
@Estado bit,
@Resultado bit output,
@Mensaje varchar (500) output
)as
begin
	 SET @Resultado =0
	 --validamos q no exista una categoria con la misma descripcion
	 if not exists (select * from CATEGORIA where Descripcion = @Descripcion)
	 begin
	 --si la condicion se cumple insertamos una nueva categoria
		  insert into CATEGORIA(Descripcion,Estado) values (@Descripcion, @Estado)
		  set @Resultado = SCOPE_IDENTITY()
	 end
	 else
		 set @mensaje ='no se puede repetir el documento para mas de un usuario'
end	 

go


--procedimiento para modificar categoria--
create proc SP_EditarCategoria(
@IdCategoria int,
@Estado bit,
@Descripcion varchar(50),
@Resultado bit output,
@Mensaje varchar (500) output
)as
begin
	 SET @Resultado =1
	 --validamos q no exista una categoria con la misma descripcion
	 if not exists (select * from CATEGORIA where Descripcion = @Descripcion and IdCategoria != @IdCategoria)
		update CATEGORIA set 
		Descripcion =@Descripcion,
		Estado = @Estado
		where IdCategoria = @IdCategoria
	 else
	 begin
		  set @Resultado = 0
		  set @Mensaje = 'no se puede repetir el documento para mas de un usuario'
	 end
end	 

go

--procedimiento para eliminar categoria--
create proc SP_EliminarCategoria(
@IdCategoria int,
@Resultado bit output,
@Mensaje varchar (500) output
)as
begin
	 SET @Resultado =1
	 --validamos si existe una asociacion entre una categoria y un producto
	 if not exists (select * from CATEGORIA c inner join PRODUCTO p
	 on p.IdCategoria = c.IdCategoria where c.IdCategoria = @IdCategoria
	 )
	 begin
	 --si no existe esa relacion, eliminamos la categoria. el top 1 es para eliminar solo una fila
		delete top(1) from CATEGORIA where IdCategoria = @IdCategoria
	 end	
	 else
	 begin
		  set @Resultado = 0
		  set @Mensaje = 'la categoria se encuentra relacionada a un producto'
	 end
end	 

go

select IdProducto, Codigo, Nombre, p.Descripcion, c.IdCategoria,c.Descripcion[DescripcionCategoria] ,Stock, PrecioCompra, PrecioVenta, p.Estado from PRODUCTO p               
inner join CATEGORIA c on c.IdCategoria = p.IdCategoria
/*---------------procedimientos para Productos---------------*/

create proc SP_RegistrarProducto(
--entrada
@Codigo varchar(20),
@Nombre varchar(30),
@Descripcion varchar(30),
@IdCategoria int,
@Estado bit,
--salida
@Resultado int output,
@Mensaje varchar(500) output
)as
begin
	set @Resultado =0
	--si existe un producto ya registrado con el mismo codigo
	if not exists (select * from PRODUCTO where Codigo = @Codigo)
	begin
	--sino existe insertamos. 
	--las otras columnas stock preciocompra precioventa se autocompletan con 0
		insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Estado) values (@Codigo,@Nombre,@Descripcion,@IdCategoria,@Estado)
		set @Resultado = SCOPE_IDENTITY()
	end
	else
	--si ya existe el producto...
		set @Mensaje = 'ya existe un producto con el mismo codigo'
end

go

--modificar producto--

create procedure SP_EditarProducto(
@IdProducto int,
@Codigo varchar(20),
@Nombre varchar(30),
@Descripcion varchar(30),
@IdCategoria int,
@Estado bit,
--salida
@Resultado bit output,
@Mensaje varchar(500) output
)as
begin
	set @Resultado =1
	if not exists (select * from PRODUCTO where Codigo=@Codigo and IdProducto != @IdProducto)
	--si se cumple la condicion, actualizamos el producto
		update PRODUCTO set
		Codigo=@Codigo,
		Nombre = @Nombre,
		Descripcion = @Descripcion,
		IdCategoria = @IdCategoria,
		Estado = @Estado
		where IdProducto = @IdProducto
	else
	begin
		set @Resultado = 0
		set @Mensaje = 'ya existe un producto con el mismo codigo'
	end
end

go


--procedimiento eliminar producto--

create procedure SP_EliminarProducto(
@IdProducto int,
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
	set @Respuesta = 0
	set @Mensaje = ''
	declare @pasoreglas bit = 1

	if exists (select * from DETALLE_COMPRA dc
	inner join PRODUCTO p on p.IdProducto = dc.IdProducto
	where p.IdProducto = @IdProducto
	)
	begin
		set @pasoreglas = 0
		set @Respuesta = 0
		set @Mensaje = @Mensaje + 'no se puede eliminar porque se encuentra relacionado con una compra\n'
	end
	
	if exists (select * from DETALLE_VENTA dv
	inner join PRODUCTO p on p.IdProducto = dv.IdProducto
	where p.IdProducto = @IdProducto
	)
	begin 
		set @pasoreglas = 0
		set @Respuesta = 0
		set @Mensaje = @Mensaje + 'no se puede eliminar porque se encuentra relacionado con una venta\n'
	end

	if(@pasoreglas = 1)
	begin
		delete from PRODUCTO where IdProducto = @IdProducto
		set @Respuesta = 1
	end
end


--insert into PRODUCTO (Codigo, Nombre, Descripcion, IdCategoria) values ('2','gaseosa','1litro',5)




/*----------------PROCEDIMIENTO PARA CLIENTE----------------------*/

create proc SP_RegistrarCliente(
@Documento varchar (50),
@NombreCompleto varchar (50),
@Correo varchar (50),
@Telefono varchar (50),
@Estado bit,
@Resultado int output,
@Mensaje varchar (500) output
)as
begin
	set @Resultado = 0 
	Declare @IDPERSONA int
	--verificamos que el documento del cliente no este ya en nuestra tabla
	if not exists (select * from CLIENTE where Documento = @Documento)
	begin
		insert into CLIENTE(Documento,NombreCompleto,Correo,Telefono,Estado) values (
		@Documento,@NombreCompleto,@Correo,@Telefono,@Estado)

		set @Resultado = SCOPE_IDENTITY()
	end
	else
		set @Mensaje = 'el numero de documento ya existe'
end
go


--editar cliente--

create proc SP_EditarCliente(
@IdCliente int,
@Documento varchar (50),
@NombreCompleto varchar (50),
@Correo varchar (50),
@Telefono varchar (50),
@Estado bit,
@Resultado bit output,
@Mensaje varchar (500) output
)as
begin
	set @Resultado = 1
	Declare @IDPERSONA int
	--verificamos que el documento del cliente y el IdCliente no este ya en nuestra tabla
	if not exists (select * from CLIENTE where Documento = @Documento and IdCliente != @IdCliente)
	begin
		update CLIENTE set 
		Documento = @Documento,
		NombreCompleto = @NombreCompleto,
		Correo = @Correo,
		Telefono = @Telefono,
		Estado = @Estado
		where IdCliente = @IdCliente
	end
	else
	begin
		set @Resultado = 0
		set @Mensaje = 'el numero de documento ya existe'
	end
end

go


/*----------PROCEDIMIENTOS PARA PROVEEDORES----------*/

create proc SP_RegistrarProveedores(
@Documento varchar (50),
@RazonSocial varchar (50),
@Correo varchar (50),
@Telefono varchar (50),
@Estado bit,
@Resultado int output,
@Mensaje varchar (500) output
)as
begin
	set @Resultado = 0
	Declare @IDPERSONA int
	if not exists (select * from PROVEEDOR where Documento = @Documento)
	begin
		insert into PROVEEDOR(Documento,RazonSocial,Correo,Telefono,Estado) values
		(@Documento, @RazonSocial, @Correo, @Telefono, @Estado)
		--devolvemos el id generado por ese proveedor
		set @Resultado = SCOPE_IDENTITY()
	end
	else
		set @Mensaje = 'el numero de documento ya existe'
end

go


--editar proveedores--

create proc SP_EditarProveedores(
@IdProveedor int,
@Documento varchar (50),
@RazonSocial varchar (50),
@Correo varchar (50),
@Telefono varchar (50),
@Estado bit,
@Resultado bit output,
@Mensaje varchar (500) output
)as
begin
	set @Resultado = 1
	Declare @IDPERSONA int
	if not exists (select * from PROVEEDOR where Documento = @Documento and IdProveedor != @IdProveedor)
	begin
		update PROVEEDOR set
		Documento = @Documento,
		RazonSocial = @RazonSocial,
		Correo = @Correo,
		Telefono = @Telefono,
		Estado = @Estado 
		where IdProveedor = @IdProveedor
	end
	else
	begin
		set @Resultado = 0
		set @Mensaje = 'el numero de documento ya existe'
	end
end

go


----eliminar proveedor----

create proc SP_EliminarProveedor(
@IdProveedor int,
@Resultado bit output,
@Mensaje varchar (500) output
)as
begin
	set @Resultado = 1
	if not exists (select * from PROVEEDOR p inner join COMPRA c on p.IdProveedor = c.IdProveedor
	where p.IdProveedor = @IdProveedor
	)
	begin
		delete top(1) from PROVEEDOR where IdProveedor = @IdProveedor
	end
	else
	begin
		set @Resultado = 0
		set @Mensaje = 'el proveedor se encuentra relacionado a una compra'
	end
end


/*---------- creamos tabla negocio ----------*/
create table NEGOCIO(
IdNegocio int primary key, --solo va haber un solo registro por eso no ponemos identity
Nombre varchar(60),
RUC varchar (60),
Direccion varchar (60),
Logo varbinary(max) NULL
)


--el logo lo cargamos desde el sistema
insert into NEGOCIO (IdNegocio, Nombre, RUC, Direccion) values (1, 'Codigo estudiante', '101010', 'av. codigo 123')

select * from NEGOCIO


--update NEGOCIO set Nombre = @nombre, RUC = @ruc, Direccion = @direccion where IdNegocio =1

--update NEGOCIO set Logo = null where IdNegocio = 1























