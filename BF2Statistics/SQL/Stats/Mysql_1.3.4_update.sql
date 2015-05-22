CREATE TABLE  `player_history` (
	`id` int(8) unsigned NOT NULL default '0',
	`timestamp` int(10) unsigned NOT NULL default '0',
	`time` int(10) unsigned NOT NULL default '0',
	`score` int(10) unsigned NOT NULL default '0',
	`cmdscore` int(10) unsigned NOT NULL default '0',
	`skillscore` int(10) unsigned NOT NULL default '0',
	`teamscore` int(10) unsigned NOT NULL default '0',
	`kills` int(10) unsigned NOT NULL default '0',
	`deaths` int(10) unsigned NOT NULL default '0',
	`rank` tinyint(2) unsigned NOT NULL default '0',
	PRIMARY KEY  (`id`, `timestamp`),
	KEY (`score`)
);