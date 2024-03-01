




CREATE PROC SP_REGISTRARUSUARIO(
@Documento varchar (50),
@NombreCompleto varchar (100),
@Correo varchar (100),
@Clave varchar (100),
@IdRol int,
@Estado bit,
@IdUsuarioResultado int output,
@Mensaje varchar (500) output
)
as
begin
     set @IdUsuarioResultado =0
	 set @Mensaje =''

	 if not exists(select * from Usuario where Documento =@Documento)
	 begin
		  insert into USUARIO(Documento, NombreCompleto, Correo, Clave, IdRol, Estado) values
		  (@Documento, @NombreCompleto,@Correo,@Clave,@IdRol,@Estado)                                 
           
		   set @IdUsuarioResultado = SCOPE_IDENTITY()
		   
      end
	  else
		   set @Mensaje = 'no se puede repetir el documento para mas de un usuario'	

end;

go
--declare @idusuariogenerado int
--declare @mensaje varchar (500)

--exec SP_REGISTRARUSUARIO '123', 'pruebas','test@gmail.com','456',2,1, @idusuariogenerado output, @mensaje output

--select @idusuariogenerado
--select @mensaje


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

	 if NOT EXISTS(select * from Usuario where Documento = @Documento and IdUsuario != @IdUsuario)          
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

