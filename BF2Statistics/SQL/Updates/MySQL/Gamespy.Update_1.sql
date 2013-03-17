CREATE TABLE `_version` (
  `dbver` int(4) NOT NULL DEFAULT 0,
  `dbdate` int(10) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`dbver`)
) DEFAULT CHARSET=latin1;

INSERT INTO _version VALUES(2, 0);