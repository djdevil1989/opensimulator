CREATE TABLE `inventoryfolders` (
  `folderID` varchar(36) NOT NULL default '',
  `agentID` varchar(36) default NULL,
  `parentFolderID` varchar(36) default NULL,
  `folderName` varchar(64) default NULL,
  `type` smallint NOT NULL default 0,
  `version` int NOT NULL default 0,
  PRIMARY KEY  (`folderID`),
  KEY `owner` (`agentID`),
  KEY `parent` (`parentFolderID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Rev. 2';
