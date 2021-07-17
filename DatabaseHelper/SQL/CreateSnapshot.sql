CREATE DATABASE [{@SnapshotName}] ON ( 
      NAME = [{@DatabaseDataName}], 
      FILENAME = [{@SnapshotFilename}]
) AS SNAPSHOT OF [{@DatabaseName}];