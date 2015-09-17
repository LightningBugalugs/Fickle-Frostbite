IF (NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = '{DatabaseName}'))
BEGIN
	CREATE DATABASE [{DatabaseName}]
END
