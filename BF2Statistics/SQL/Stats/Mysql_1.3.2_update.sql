ALTER TABLE  `army`
	ADD COLUMN `time9` int(10) unsigned NOT NULL default '0',
	ADD COLUMN `win9` int(10) unsigned NOT NULL default '0',
	ADD COLUMN `loss9` int(10) unsigned NOT NULL default '0',
	ADD COLUMN `score9` int(10) unsigned NOT NULL default '0',
	ADD COLUMN `best9` int(10) unsigned NOT NULL default '0',
	ADD COLUMN `worst9` int(10) unsigned NOT NULL default '0',
	ADD COLUMN `brnd9` int(10) unsigned NOT NULL default '0';

CREATE TABLE `servers` (
	`id` int(11) unsigned NOT NULL AUTO_INCREMENT,
	`ip` varchar(15) NOT NULL default '',
	`prefix` varchar(30) NOT NULL default '',
	`name` varchar(100) default NULL,
	`port` int(6) unsigned default '0',
	`queryport` int(6) unsigned NOT NULL default '0',
	`lastupdate` datetime NOT NULL default '0000-00-00 00:00:00',
	PRIMARY KEY  (`id`),
	UNIQUE KEY `ip-prefix-unq` (`ip`,`prefix`)
);

ALTER TABLE  `player`
	ADD COLUMN `mode0` int(10) unsigned NOT NULL default '0',
	ADD COLUMN `mode1` int(10) unsigned NOT NULL default '0',
	ADD COLUMN `mode2` int(10) unsigned NOT NULL default '0';