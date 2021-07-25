DECLARE @p_strDBNameTo SYSNAME = '{@DatabaseName}';
DECLARE @p_strFQNRestoreFileName VARCHAR(255) = '{@Filename}';

DECLARE 
	@v_strDBFilename VARCHAR(100),
	@v_strDBLogFilename VARCHAR(100),
	@v_strDBDataFile VARCHAR(100),
	@v_strDBLogFile VARCHAR(100),
	@v_strExecSQL NVARCHAR(1000),
	@v_strExecSQL1 NVARCHAR(1000),
	@v_strMoveSQL NVARCHAR(4000),
	@v_strREPLACE NVARCHAR(50),
	@v_strTEMP NVARCHAR(1000),
	@v_strListSQL NVARCHAR(4000),
	@v_strServerVersion NVARCHAR(20),
	@v_strDataPath NVARCHAR(1000);

SET @v_strDataPath = (SELECT SUBSTRING(physical_name, 1, CHARINDEX(N'master.mdf', LOWER(physical_name)) - 1) FROM master.sys.master_files WHERE database_id = 1 AND file_id = 1)
SET @v_strREPLACE = ''   
IF exists (select name from sys.databases where name = @p_strDBNameTo)
	SET @v_strREPLACE = ', REPLACE'

SET @v_strListSQL = ''
SET @v_strListSQL = @v_strListSQL + 'IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ''##FILE_LIST''))'
SET @v_strListSQL = @v_strListSQL + 'BEGIN'
SET @v_strListSQL = @v_strListSQL + '   DROP TABLE ##FILE_LIST '
SET @v_strListSQL = @v_strListSQL + 'END '

SET @v_strListSQL = @v_strListSQL + 'CREATE TABLE ##FILE_LIST ('
SET @v_strListSQL = @v_strListSQL + '   LogicalName VARCHAR(64),'
SET @v_strListSQL = @v_strListSQL + '   PhysicalName VARCHAR(130),'
SET @v_strListSQL = @v_strListSQL + '   [Type] VARCHAR(1),'
SET @v_strListSQL = @v_strListSQL + '   FileGroupName VARCHAR(64),'
SET @v_strListSQL = @v_strListSQL + '   Size DECIMAL(20, 0),'
SET @v_strListSQL = @v_strListSQL + '   MaxSize DECIMAL(25,0),'
SET @v_strListSQL = @v_strListSQL + '   FileID bigint,'
SET @v_strListSQL = @v_strListSQL + '   CreateLSN DECIMAL(25,0),'
SET @v_strListSQL = @v_strListSQL + '   DropLSN DECIMAL(25,0),'
SET @v_strListSQL = @v_strListSQL + '   UniqueID UNIQUEIDENTIFIER,'
SET @v_strListSQL = @v_strListSQL + '   ReadOnlyLSN DECIMAL(25,0),'
SET @v_strListSQL = @v_strListSQL + '   ReadWriteLSN DECIMAL(25,0),'
SET @v_strListSQL = @v_strListSQL + '   BackupSizeInBytes DECIMAL(25,0),'
SET @v_strListSQL = @v_strListSQL + '   SourceBlockSize INT,'
SET @v_strListSQL = @v_strListSQL + '   filegroupid INT,'
SET @v_strListSQL = @v_strListSQL + '   loggroupguid UNIQUEIDENTIFIER,'
SET @v_strListSQL = @v_strListSQL + '   differentialbaseLSN DECIMAL(25,0),'
SET @v_strListSQL = @v_strListSQL + '   differentialbaseGUID UNIQUEIDENTIFIER,'
SET @v_strListSQL = @v_strListSQL + '   isreadonly BIT,'
SET @v_strListSQL = @v_strListSQL + '   ispresent BIT'

SELECT @v_strServerVersion = CAST(SERVERPROPERTY ('PRODUCTVERSION') AS NVARCHAR)

IF @v_strServerVersion LIKE '10.%' OR 
   @v_strServerVersion LIKE '11.%' OR 
   @v_strServerVersion LIKE '12.%' OR 
   @v_strServerVersion LIKE '13.%' OR 
   @v_strServerVersion LIKE '14.%' OR 
   @v_strServerVersion LIKE '15.%' OR 
   @v_strServerVersion LIKE '16.%' 
	BEGIN
		SET @v_strListSQL = @v_strListSQL + ', TDEThumbpr varbinary(32)'
		--PRINT @v_strServerVersion
	END

IF @v_strServerVersion LIKE '13.%' OR 
   @v_strServerVersion LIKE '14.%' OR 
   @v_strServerVersion LIKE '15.%' OR 
   @v_strServerVersion LIKE '16.%' 
	BEGIN
		SET @v_strListSQL = @v_strListSQL + ', SnapshotURL nvarchar(360)'
		--PRINT @v_strServerVersion
	END

SET @v_strListSQL = @v_strListSQL + ')'

EXEC (@v_strListSQL)

INSERT INTO ##FILE_LIST EXEC ('RESTORE FILELISTONLY FROM DISK = ''' + @p_strFQNRestoreFileName + '''')

DECLARE curFileLIst CURSOR FOR 
	SELECT 'MOVE N''' + LogicalName + ''' TO N''' + @v_strDataPath + @p_strDBNameTo, [Type]
	  FROM ##FILE_LIST

SET @v_strMoveSQL = ''

DECLARE @v_type VARCHAR(1)

OPEN curFileList 
FETCH NEXT FROM curFileList into @v_strTEMP, @v_type
WHILE @@Fetch_Status = 0
BEGIN
	IF @v_type = 'D'
		SET @v_strMoveSQL = @v_strMoveSQL + @v_strTEMP + '.mdf'' , '
	ELSE
		SET @v_strMoveSQL = @v_strMoveSQL + @v_strTEMP + '.ldf'' , '
	FETCH NEXT FROM curFileList into @v_strTEMP, @v_type
END

CLOSE curFileList
DEALLOCATE curFileList

PRINT 'Killing active connections to the "' + @p_strDBNameTo + '" database'

-- Create the sql to kill the active database connections
SET @v_strExecSQL = ''
SELECT   @v_strExecSQL = @v_strExecSQL + 'kill ' + CONVERT(CHAR(10), spid) + ' '
FROM     master.dbo.sysprocesses
WHERE    DB_NAME(dbid) = @p_strDBNameTo AND DBID <> 0 AND spid <> @@spid

EXEC (@v_strExecSQL)

PRINT 'Restoring "' + @p_strDBNameTo + '" database from "' + @p_strFQNRestoreFileName + '" with '
PRINT '  data file "' + @v_strDBDataFile + '" located at "' + @v_strDBFilename + '"'
PRINT '  log file "' + @v_strDBLogFile + '" located at "' + @v_strDBLogFilename + '"'

SET @v_strExecSQL = 'RESTORE DATABASE [' + @p_strDBNameTo + ']'
SET @v_strExecSQL = @v_strExecSQL + ' FROM DISK = ''' + @p_strFQNRestoreFileName + ''''
SET @v_strExecSQL = @v_strExecSQL + ' WITH FILE = 1,'
SET @v_strExecSQL = @v_strExecSQL + @v_strMoveSQL
SET @v_strExecSQL = @v_strExecSQL + ' REPLACE'
SET @v_strExecSQL = @v_strExecSQL + @v_strREPLACE


PRINT '-QUERY USED----------------'
PRINT @v_strExecSQL
PRINT '---------------------------'
PRINT ''

DROP TABLE ##FILE_LIST
