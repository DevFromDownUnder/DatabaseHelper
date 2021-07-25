SELECT 
    OD.name AS          DatabaseName, 
    OD.database_id AS   DatabaseId
FROM SYS.databases AS OD 
WHERE 
    OD.source_database_id IS NULL 
    AND OD.is_distributor = 0 
ORDER BY 
    OD.name