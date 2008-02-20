CREATE TABLE `regions` (
  `uuid` varchar(36) NOT NULL,
  `regionHandle` bigint(20) unsigned NOT NULL,
  `regionName` varchar(32) default NULL,
  `regionRecvKey` varchar(128) default NULL,
  `regionSendKey` varchar(128) default NULL,
  `regionSecret` varchar(128) default NULL,
  `regionDataURI` varchar(255) default NULL,
  `serverIP` varchar(64) default NULL,
  `serverPort` int(10) unsigned default NULL,
  `serverURI` varchar(255) default NULL,
  `locX` int(10) unsigned default NULL,
  `locY` int(10) unsigned default NULL,
  `locZ` int(10) unsigned default NULL,
  `eastOverrideHandle` bigint(20) unsigned default NULL,
  `westOverrideHandle` bigint(20) unsigned default NULL,
  `southOverrideHandle` bigint(20) unsigned default NULL,
  `northOverrideHandle` bigint(20) unsigned default NULL,
  `regionAssetURI` varchar(255) default NULL,
  `regionAssetRecvKey` varchar(128) default NULL,
  `regionAssetSendKey` varchar(128) default NULL,
  `regionUserURI` varchar(255) default NULL,
  `regionUserRecvKey` varchar(128) default NULL,
  `regionUserSendKey` varchar(128) default NULL, `regionMapTexture` varchar(36) default NULL,
  `serverHttpPort` int(10) default NULL, `serverRemotingPort` int(10) default NULL,
  PRIMARY KEY  (`uuid`),
  KEY `regionName` (`regionName`),
  KEY `regionHandle` (`regionHandle`),
  KEY `overrideHandles` (`eastOverrideHandle`,`westOverrideHandle`,`southOverrideHandle`,`northOverrideHandle`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=FIXED COMMENT='Rev. 1';
