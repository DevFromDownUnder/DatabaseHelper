SELECT 
    CAST(0 AS BIT) AS   [Restore], 
    SD.name AS          SnapshotName, 
    SD.database_id AS   SnapshotDatabaseId, 
    MS.physical_name AS SnapshotFilename, 
    OD.name AS          SourceName, 
    OD.database_id AS   SourceDatabaseId, 
    OS.physical_name AS SourceFilename 
FROM SYS.databases AS SD 
JOIN SYS.databases AS OD ON od.database_id = SD.source_database_id 
JOIN SYS.master_files AS MS ON MS.database_id = SD.database_id 
JOIN SYS.master_files AS OS ON OS.database_id = OD.database_id 
WHERE 
    SD.source_database_id IS NOT NULL 
    AND (MS.type = 1 OR OS.type = 1) 
ORDER BY 
    SD.name, 
    OD.name;