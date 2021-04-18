SELECT 
    CAST(0 AS BIT) AS   [Create], 
    OD.name AS          DatabaseName, 
    OD.database_id AS   DatabaseId, 
    OS.name AS          DatabaseDataName, 
    OS.physical_name AS DatabaseFilename 
FROM SYS.databases AS OD 
JOIN SYS.master_files AS OS ON OS.database_id = OD.database_id 
WHERE 
    OD.source_database_id IS NULL 
    AND OS.type = 0 
    AND OD.is_distributor = 0 
    AND OD.name NOT IN ('master', 'tempdb', 'model', 'msdb') 
ORDER BY 
    OD.name