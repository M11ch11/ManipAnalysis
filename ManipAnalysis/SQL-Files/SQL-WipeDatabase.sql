CREATE PROCEDURE [dbo].[wipeDatabase] AS

-- variable to object name
DECLARE @name  varchar(1000);

-- variable to hold object type
DECLARE @xtype varchar(20);

-- variable to hold sql string
DECLARE @sqlstring nvarchar(4000);
DECLARE SPViews_cursor CURSOR FOR
	SELECT QUOTENAME(ROUTINE_SCHEMA) + '.' + QUOTENAME(ROUTINE_NAME) AS name, ROUTINE_TYPE AS xtype
	FROM INFORMATION_SCHEMA.ROUTINES
	UNION
	SELECT QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME) AS name, 'VIEW' AS xtype
	FROM INFORMATION_SCHEMA.VIEWS;

OPEN SPViews_cursor;
FETCH NEXT FROM SPViews_cursor INTO @name, @xtype;

WHILE @@FETCH_status = 0
BEGIN
	-- test object type IF it is a stored procedure
	IF @xtype = 'PROCEDURE'
	BEGIN
		SET @sqlstring = 'drop procedure ' + @name;
		EXEC sp_executesql @sqlstring;
		SET @sqlstring = ' ';
	END

	-- test object type IF it is a function
	IF @xtype = 'FUNCTION'
	BEGIN
		SET @sqlstring = 'drop FUNCTION ' + @name;
		EXEC sp_executesql @sqlstring;
		SET @sqlstring = ' ';
	END
	
	-- test object type IF it is a view
	IF @xtype = 'VIEW'
		BEGIN
		SET @sqlstring = 'drop view ' + @name;
		EXEC sp_executesql @sqlstring;
		SET @sqlstring = ' ';
	END
	
	-- get NEXT record
	FETCH NEXT FROM SPViews_cursor INTO @name, @xtype;
END

CLOSE SPViews_cursor;
DEALLOCATE SPViews_cursor;

GO

EXECUTE [dbo].[wipeDatabase];
GO