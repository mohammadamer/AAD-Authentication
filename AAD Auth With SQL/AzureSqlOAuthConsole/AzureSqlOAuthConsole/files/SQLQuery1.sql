--Creating a db role
CREATE ROLE AzureSqlOAuthRole AUTHORIZATION [dbo]
GO


-- Grant access rights to a specific schema in the database
GRANT 
    SELECT, 
    VIEW DEFINITION 
ON SCHEMA:: [dbo]
    TO AzureSqlOAuthRole
GO

-- Need to run using Active Directory accounts otherwise you will get this error
--Principal '' could not be created. 
--Only connections established with Active Directory accounts can create other Active Directory users.
--Name of the App Registration that you should create it on azure "AzureSqlOAuth"
create user [AzureSqlOAuth] from EXTERNAL PROVIDER
GO


--Adding the app to be member of the role "AzureSqlOAuthRole"
ALTER ROLE AzureSqlOAuthRole ADD MEMBER [AzureSqlOAuth]
GO






SELECT p.NAME
    ,m.NAME
    FROM sys.database_role_members rm
    JOIN sys.database_principals p
    ON rm.role_principal_id = p.principal_id
    JOIN sys.database_principals m
    ON rm.member_principal_id = m.principal_id 
    GO
    SELECT DISTINCT pr.principal_id, pr.name, pr.type_desc, 
    pr.authentication_type_desc, pe.state_desc, pe.permission_name
    FROM sys.database_principals AS pr
    JOIN sys.database_permissions AS pe
    ON pe.grantee_principal_id = pr.principal_id;


-- Reference for the Queries
--https://stackoverflow.com/questions/51642423/azure-sql-database-user-not-inheriting-role-permissions