
create database Asignar
on
(
	name = 'Asignar',
	filename = 'C:\Asignar\Asignar.mdf',
	size = 50mb,
	maxsize = 500mb,
	filegrowth = 5%
)
log on
(
	name = 'AsignarLog',
	filename = 'C:\Asignar\Asignar.mdl',
	size = 10mb,
	maxsize = 100mb,
	filegrowth = 5%
)
GO

use Asignar
GO
