CREATE DATABASE [{@SnapshotName}] ON ( 
      NAME = [{@SnapshotDataName}], 
      FILENAME = [{@SnapshotFolder} + '\\' + {@SnapshotFilename}]
) AS SNAPSHOT OF [{@SourceDatabaseName}];