


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
declare @respuesta bit
declare @mensaje varchar (500)

exec SP_EDITARUSUARIO 3, '20', 'pruebas 3','test@gmail.com','456',2,0, @respuesta output, @mensaje output

select @respuesta

select @mensaje

select * from USUARIO


--select * from Usuario where Documento = 123 and IdUsuario != 2

go

--eliminar usuario
CREATE PROC SP_ELIMINARUSUARIO(
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



declare @respuesta bit
declare @mensaje varchar (500)

exec SP_EDITARUSUARIO 3, '20', 'pruebas 3','test@gmail.com','456',2,0, @respuesta output, @mensaje output

select @respuesta

select @mensaje

select * from USUARIO










