medal_data = (
	('1031406_1', 'kcb', 1, object_stat('weapons', 'kills', WEAPON_TYPE_KNIFE, 7)),#stop
	('1031406_2', 'kcb', 1, f_and(has_medal('1031406_1'), f_plus(global_stat('wkl-9'), object_stat('weapons', 'kills', WEAPON_TYPE_KNIFE), 50))),#stop
	('1031406_3', 'kcb', 1, f_and(has_medal('1031406_2'), f_plus(global_stat('wkl-9'), object_stat('weapons', 'kills', WEAPON_TYPE_KNIFE), 100))),#stop
	('1031619_1', 'pcb', 1, object_stat('weapons', 'kills', WEAPON_TYPE_PISTOL, 5)),#stop
	('1031619_2', 'pcb', 1, f_and(has_medal('1031619_1'), global_stat('wkl-5', 50), object_stat('weapons', 'kills', WEAPON_TYPE_PISTOL, 7))),#stop
	('1031619_3', 'pcb', 1, f_and(has_medal('1031619_2'), f_plus(global_stat('wkl-5'), object_stat('weapons', 'kills', WEAPON_TYPE_PISTOL), 500), object_stat('weapons', 'kills', WEAPON_TYPE_PISTOL, 18))),#stop
	('1031119_1', 'Acb', 1, object_stat('kits', 'kills', KIT_TYPE_ASSAULT, 5)),#stop
	('1031119_2', 'Acb', 1, f_and(has_medal('1031119_1'), global_stat('ktm-1', 54000), object_stat('kits', 'kills', KIT_TYPE_ASSAULT, 20))),#stop
	('1031119_3', 'Acb', 1, f_and(has_medal('1031119_2'), global_stat('ktm-1', 360000), object_stat('kits', 'kills', KIT_TYPE_ASSAULT, 40))),#stop
	('1031120_1', 'Atcb', 1, object_stat('kits', 'kills', KIT_TYPE_AT, 10)),#stop
	('1031120_2', 'Atcb', 1, f_and(has_medal('1031120_1'), global_stat('ktm-0', 54000), object_stat('kits', 'kills', KIT_TYPE_AT, 20))),#stop
	('1031120_3', 'Atcb', 1, f_and(has_medal('1031120_2'), global_stat('ktm-0', 360000), object_stat('kits', 'kills', KIT_TYPE_AT, 40))),#stop
	('1031109_1', 'Sncb', 1, object_stat('kits', 'kills', KIT_TYPE_SNIPER, 10)),#stop
	('1031109_2', 'Sncb', 1, f_and(has_medal('1031109_1'), global_stat('ktm-6', 54000), object_stat('kits', 'kills', KIT_TYPE_SNIPER, 15))),#stop
	('1031109_3', 'Sncb', 1, f_and(has_medal('1031109_2'), global_stat('ktm-6', 360000), object_stat('kits', 'kills', KIT_TYPE_SNIPER, 35))),#stop
	('1031115_1', 'Socb', 1, object_stat('kits', 'kills', KIT_TYPE_SPECOPS, 5)),#stop
	('1031115_2', 'Socb', 1, f_and(has_medal('1031115_1'), global_stat('ktm-4', 54000), object_stat('kits', 'kills', KIT_TYPE_SPECOPS, 20))),#stop
	('1031115_3', 'Socb', 1, f_and(has_medal('1031115_2'), global_stat('ktm-4', 360000), object_stat('kits', 'kills', KIT_TYPE_SPECOPS, 40))),#stop
	('1031121_1', 'Sucb', 1, object_stat('kits', 'kills', KIT_TYPE_SUPPORT, 10)),#stop
	('1031121_2', 'Sucb', 1, f_and(has_medal('1031121_1'), global_stat('ktm-5', 54000), object_stat('kits', 'kills', KIT_TYPE_SUPPORT, 20))),#stop
	('1031121_3', 'Sucb', 1, f_and(has_medal('1031121_2'), global_stat('ktm-5', 360000), object_stat('kits', 'kills', KIT_TYPE_SUPPORT, 40))),#stop
	('1031105_1', 'Ecb', 1, object_stat('kits', 'kills', KIT_TYPE_ENGINEER, 10)),#stop
	('1031105_2', 'Ecb', 1, f_and(has_medal('1031105_1'), global_stat('ktm-2', 54000), object_stat('kits', 'kills', KIT_TYPE_ENGINEER, 20))),#stop
	('1031105_3', 'Ecb', 1, f_and(has_medal('1031105_2'), global_stat('ktm-2', 360000), object_stat('kits', 'kills', KIT_TYPE_ENGINEER, 40))),#stop
	('1031113_1', 'Mcb', 1, object_stat('kits', 'kills', KIT_TYPE_MEDIC, 10)),#stop
	('1031113_2', 'Mcb', 1, f_and(has_medal('1031113_1'), global_stat('ktm-3', 54000), object_stat('kits', 'kills', KIT_TYPE_MEDIC, 20))),#stop
	('1031113_3', 'Mcb', 1, f_and(has_medal('1031113_2'), global_stat('ktm-3', 360000), object_stat('kits', 'kills', KIT_TYPE_MEDIC, 40))),#stop
	('1032415_1', 'Eob', 1, f_plus(object_stat('weapons', 'kills', WEAPON_TYPE_C4), f_plus(object_stat('weapons', 'kills', WEAPON_TYPE_ATMINE), object_stat('weapons', 'kills', WEAPON_TYPE_CLAYMORE)), 5)),#stop
	('1032415_2', 'Eob', 1, f_and(has_medal('1032415_1'), global_stat('wkl-11', 50), f_plus(object_stat('weapons', 'kills', WEAPON_TYPE_C4), f_plus(object_stat('weapons', 'kills', WEAPON_TYPE_ATMINE), object_stat('weapons', 'kills', WEAPON_TYPE_CLAYMORE)), 20))),#stop
	('1032415_3', 'Eob', 1, f_and(has_medal('1032415_2'), global_stat('wkl-11', 300), f_plus(object_stat('weapons', 'kills', WEAPON_TYPE_C4), f_plus(object_stat('weapons', 'kills', WEAPON_TYPE_ATMINE), object_stat('weapons', 'kills', WEAPON_TYPE_CLAYMORE)), 30))),#stop
	('1190601_1', 'Fab', 1, player_stat('heals', 5)),#stop
	('1190601_2', 'Fab', 1, f_and(has_medal ('1190601_1'),global_stat ('ktm-3', 54000),player_stat ('heals', 10))),#stop
	('1190601_3', 'Fab', 1, f_and(has_medal('1190601_2'), global_stat('heal', 750), global_stat('ktm-3', 360000), player_stat('heals', 20))),#stop
	('1190507_1', 'Eb', 1, player_stat('repairs', 5)),#stop
	('1190507_2', 'Eb', 1, f_and(has_medal('1190507_1'), global_stat('ktm-2', 54000), player_stat('repairs', 10))),#stop
	('1190507_3', 'Eb', 1, f_and(has_medal('1190507_2'), global_stat('rpar', 250), global_stat('ktm-2', 360000), player_stat('repairs', 25))),#stop
	('1191819_1', 'Rb', 1, player_stat('ammos', 5)),#stop
	('1191819_2', 'Rb', 1, f_and(has_medal('1191819_1'), global_stat('ktm-5', 54000), player_stat('ammos', 10))),#stop
	('1191819_3', 'Rb', 1, f_and(has_medal('1191819_2'), global_stat('rsup', 500), global_stat('ktm-5', 360000), player_stat('ammos', 25))),#stop
	('1190304_1', 'Cb', 1, player_stat('cmdScore', 40)),#stop
	('1190304_2', 'Cb', 1, f_and(has_medal('1190304_1'), global_stat('cdsc', 1000), player_stat('timeAsCmd', 1500))),#stop
	('1190304_3', 'Cb', 1, f_and(has_medal('1190304_2'), global_stat('cdsc', 10000), player_stat('timeAsCmd', 1800))),#stop
	('1220118_1', 'Ab', 1, object_stat('vehicles', 'rtime', VEHICLE_TYPE_ARMOR, 600)),#stop
	('1220118_2', 'Ab',	1, f_and(has_medal ('1220118_1'),global_stat ('vtm-0', 360000),object_stat ('vehicles', 'kills', VEHICLE_TYPE_ARMOR, 12))),#stop
	('1220118_3', 'Ab', 1, f_and(has_medal('1220118_2'), global_stat('vtm-0', 1440000), object_stat('vehicles', 'kills', VEHICLE_TYPE_ARMOR, 24))),#stop
	('1222016_1', 'Tb', 1, object_stat('vehicles', 'rtime', VEHICLE_TYPE_TRANSPORT, 600)),#stop
	('1222016_2', 'Tb', 1, f_and(has_medal('1222016_1'), global_stat('vtm-4', 90000), global_stat('dsab', 200), object_stat('vehicles', 'roadKills', VEHICLE_TYPE_TRANSPORT, 5))),#stop
	('1222016_3', 'Tb', 1, f_and(has_medal('1222016_2'), global_stat('vtm-4', 270000), global_stat('dsab', 2000), object_stat('vehicles', 'roadKills', VEHICLE_TYPE_TRANSPORT, 11))),#stop
	('1220803_1', 'Hb', 1, object_stat('vehicles', 'rtime', VEHICLE_TYPE_HELICOPTER, 900)),#stop
	('1220803_2', 'Hb', 1, f_and(has_medal('1220803_1'), global_stat('vtm-3', 180000), object_stat('vehicles', 'kills', VEHICLE_TYPE_HELICOPTER, 12))),#stop
	('1220803_3', 'Hb', 1, f_and(has_medal('1220803_2'), global_stat('vtm-3', 540000), object_stat('vehicles', 'kills', VEHICLE_TYPE_HELICOPTER, 24))),#stop
	('1220122_1', 'Avb', 1, object_stat('vehicles', 'rtime', VEHICLE_TYPE_AVIATOR, 600)),#stop
	('1220122_2', 'Avb', 1, f_and(has_medal('1220122_1'), global_stat('vtm-1', 180000), object_stat('vehicles', 'kills', VEHICLE_TYPE_AVIATOR, 12))),#stop
	('1220122_3', 'Avb', 1, f_and(has_medal('1220122_2'), global_stat('vtm-1', 540000), object_stat('vehicles', 'kills', VEHICLE_TYPE_AVIATOR, 24))),#stop
	('1220104_1', 'adb', 1, object_stat('vehicles', 'rtime', VEHICLE_TYPE_AIRDEFENSE, 600)),#stop
	('1220104_2', 'adb', 1, f_and(has_medal ('1220104_1'),object_stat ('vehicles', 'kills', VEHICLE_TYPE_AIRDEFENSE, 10))),#stop
	('1220104_3', 'adb', 1, f_and(has_medal('1220104_2'), object_stat('vehicles', 'kills', VEHICLE_TYPE_AIRDEFENSE, 20))),#stop
	('1031923_1', 'Swb', 1, object_stat('vehicles', 'rtime', VEHICLE_TYPE_GRNDDEFENSE, 300)),#stop
	('1031923_2', 'Swb', 1, f_and(has_medal('1031923_1'), object_stat('vehicles', 'kills', VEHICLE_TYPE_GRNDDEFENSE, 10))),#stop
	('1031923_3', 'Swb', 1, f_and(has_medal('1031923_2'), object_stat('vehicles', 'kills', VEHICLE_TYPE_GRNDDEFENSE, 20))),#stop
	('2191608', 'ph', 0, f_and(player_stat('kills', 5), player_stat('deaths', 20), f_div(player_stat('deaths'), player_stat('kills'), 4))),#stop
	('2191319', 'Msm', 2, f_and(global_stat_multiple_times('time', 900000, '2191319'), global_stat_multiple_times('heal', 1000, '2191319'), global_stat_multiple_times('rpar', 1000, '2191319'), global_stat_multiple_times('rsup', 1000, '2191319'))),#stop
	('2190303', 'Cam', 2, f_and(global_stat_multiple_times('time', 900000, '2190303'), global_stat_multiple_times('kill', 25000, '2190303'), global_stat('bksk', 25), player_stat('timePlayed', 1980))),#stop
	('2190309', 'Acm', 2, f_and(global_stat_multiple_times('vtm-1', 360000, '2190309'), global_stat_multiple_times('vkl-1', 5000, '2190309'), object_stat('vehicles', 'kills', VEHICLE_TYPE_AVIATOR, 25))),#stop
	('2190318', 'Arm', 2, f_and(global_stat_multiple_times('vtm-0', 360000, '2190318'), global_stat_multiple_times('vkl-0', 5000, '2190318'), object_stat('vehicles', 'kills', VEHICLE_TYPE_ARMOR, 25))),#stop
	('2190308', 'Hcm', 2, f_and(global_stat_multiple_times('vtm-3', 360000, '2190308'), global_stat_multiple_times('vkl-3', 5000, '2190308'), object_stat('vehicles', 'kills', VEHICLE_TYPE_HELICOPTER, 30))),#stop
	('2190703', 'gcm', 2, f_and(global_stat_multiple_times('time', 900000, '2190703'), player_stat('kills', 27), f_not(f_plus(player_stat('TKs'), f_plus(player_stat('teamDamages'), player_stat('teamVehicleDamages')), 1)))),#stop
	('2020903', 'Cim', 1, f_and(global_stat('time', 720000), has_medal('1031406_1'), has_medal('1031619_1'), has_medal('1031119_1'), has_medal('1031120_1'), has_medal('1031109_1'), has_medal('1031115_1'), has_medal('1031121_1'), has_medal('1031105_1'), has_medal('1031113_1'))),#stop
	('2020913', 'Mim', 1, f_and(global_stat('time', 1080000), has_medal('2020903'), has_medal('1031406_2'), has_medal('1031619_2'), has_medal('1031119_2'), has_medal('1031120_2'), has_medal('1031109_2'), has_medal('1031115_2'), has_medal('1031121_2'), has_medal('1031105_2'), has_medal('1031113_2'))),#stop
	('2020919', 'Sim', 1, f_and(global_stat('time', 1440000), has_medal('2020913'), has_medal('1031406_3'), has_medal('1031619_3'), has_medal('1031119_3'), has_medal('1031120_3'), has_medal('1031109_3'), has_medal('1031115_3'), has_medal('1031121_3'), has_medal('1031105_3'), has_medal('1031113_3'))),#stop
	('2021322', 'Mvm', 2, f_and(global_stat_multiple_times('time', 900000, '2021322'), global_stat_multiple_times('dsab', 5000, '2021322'), global_stat_multiple_times('dfcp', 1000, '2021322'), global_stat_multiple_times('twsc', 30000, '2021322'))),#stop
	('2020419', 'Dsm', 2, f_and(global_stat_multiple_times('tcdr', 360000, '2020419'), global_stat_multiple_times('tsql', 360000, '2020419'), global_stat_multiple_times('tsqm', 360000, '2020419'), player_stat('rplScore', 45))),#stop
	('3240301', 'Car', 1, f_and(global_stat('bksk', 10), player_stat('kills', 18))),#stop
	('3211305', 'Mur', 1, f_and(player_stat('timeInSquad', 1560), player_stat('rplScore', 40))),#stop
	('3150914', 'Ior', 1, f_and(global_stat('twsc', 250), player_stat('timeAsSql', 1500))),#stop
	('3151920', 'Sor', 1, f_and(player_stat('timeAsCmd', 1680), player_stat('cmdScore', 50))),#stop
	('3190409', 'Dsr', 1, f_and(global_stat('tsqm', 36000), global_stat('tsql', 36000), global_stat('tcdr', 36000), player_stat('rplScore', 15))),#stop
	('3242303', 'Wcr', 1, f_and(global_stat('tcdr', 360000), global_stat('wins', 200), global_stat('cdsc', 25000))),#stop
	('3212201', 'Vur', 1, f_and(global_stat('tsqm', 90000), global_stat('tsql', 90000), player_stat('rplScore', 45))),#stop
	('3241213', 'Lmr', 1, f_and(global_stat('time', 720000), global_stat('bksk', 10), global_stat('wdsk', 8), player_stat('rplScore', 50))),#stop
	('3190318', 'Csr', 1, f_and(f_plus(player_stat('driverSpecials'), player_stat('driverAssists'), 13), player_stat('kills', 5))),#stop
	('3190118', 'Arr', 1, f_and(object_stat('vehicles', 'rtime', VEHICLE_TYPE_ARMOR, 1200), object_stat('vehicles', 'kills', VEHICLE_TYPE_ARMOR, 19))),#stop
	('3190105', 'Aer', 1, f_and(object_stat('vehicles', 'rtime', VEHICLE_TYPE_AVIATOR, 900), object_stat('vehicles', 'kills', VEHICLE_TYPE_AVIATOR, 19))),#stop
	('3190803', 'Hsr', 1, f_and(object_stat('vehicles', 'rtime', VEHICLE_TYPE_HELICOPTER, 900), object_stat('vehicles', 'kills', VEHICLE_TYPE_HELICOPTER, 19))),#stop
	('3040109', 'Adr', 1, f_and(object_stat('vehicles', 'rtime', VEHICLE_TYPE_AIRDEFENSE, 180), object_stat('vehicles', 'kills', VEHICLE_TYPE_AIRDEFENSE, 11))),#stop
	('3040718', 'Gdr', 1, f_and(object_stat('vehicles', 'rtime', VEHICLE_TYPE_GRNDDEFENSE, 180), object_stat('vehicles', 'kills', VEHICLE_TYPE_GRNDDEFENSE, 5))),#stop
	('3240102', 'Ar', 1, f_and(object_stat('vehicles', 'rtime', VEHICLE_TYPE_PARACHUTE, 10))),#stop
	('3240703', 'gcr', 1, f_and(global_stat('time', 180000), player_stat('kills', 14), f_not(f_plus(player_stat('TKs'), f_plus(player_stat('teamDamages'), player_stat('teamVehicleDamages')), 1)))),#stop
)
rank_data = (
	(1, 'rank', f_plus(global_stat('scor'), player_stat('score'), 150)),#stop
	(2, 'rank', f_and(has_rank(1), f_plus(global_stat('scor'), player_stat('score'), 500))),#stop
	(3, 'rank', f_and(has_rank(2), f_plus(global_stat('scor'), player_stat('score'), 800))),#stop
	(4, 'rank', f_and(has_rank(3), f_plus(global_stat('scor'), player_stat('score'), 2500))),#stop
	(5, 'rank', f_and(has_rank(4), f_plus(global_stat('scor'), player_stat('score'), 5000))),#stop
	(6, 'rank', f_and(has_rank(5), f_plus(global_stat('scor'), player_stat('score'), 8000))),#stop
	(7, 'rank', f_and(has_rank(6), f_plus(global_stat('scor'), player_stat('score'), 20000))),#stop
	(8, 'rank', f_and(has_rank(6), f_plus(global_stat('scor'), player_stat('score'), 20000), has_medal('1031406_1'), has_medal('1031619_1'), has_medal('1031119_1'), has_medal('1031120_1'), has_medal('1031109_1'), has_medal('1031115_1'), has_medal('1031121_1'), has_medal('1031105_1'), has_medal('1031113_1'))),#stop
	(9, 'rank', f_and(f_or(has_rank(7), has_rank(8)), f_plus(global_stat('scor'), player_stat('score'), 50000))),#stop
	(10, 'rank', f_and(f_or(has_rank(7), has_rank(8)), f_plus(global_stat('scor'), player_stat('score'), 50000), has_medal('1220118_1'), has_medal('1222016_1'), has_medal('1220803_1'), has_medal('1220122_1'), has_medal('1220104_1'), has_medal('1031923_1'))),#stop
	(11, 'rank', f_and(f_plus(global_stat('scor'), player_stat('score'), 50000), has_medal('6666666'))),#stop
	(12, 'rank', f_and(f_or(has_rank(9), has_rank(10), has_rank(11)), f_plus(global_stat('scor'), player_stat('score'), 60000))),#stop
	(13, 'rank', f_and(has_rank(12), f_plus(global_stat('scor'), player_stat('score'), 75000))),#stop
	(14, 'rank', f_and(has_rank(13), f_plus(global_stat('scor'), player_stat('score'), 90000))),#stop
	(15, 'rank', f_and(has_rank(14), f_plus(global_stat('scor'), player_stat('score'), 115000))),#stop
	(16, 'rank', f_and(has_rank(15), f_plus(global_stat('scor'), player_stat('score'), 125000))),#stop
	(17, 'rank', f_and(has_rank(16), f_plus(global_stat('scor'), player_stat('score'), 150000))),#stop
	(18, 'rank', f_and(has_rank(17), f_plus(global_stat('scor'), player_stat('score'), 180000), has_medal('1220118_2'), has_medal('1222016_2'), has_medal('1220803_2'), has_medal('1220122_2'), has_medal('1220104_2'), has_medal('1031923_2'), global_stat('time', 3888000))),#stop
	(19, 'rank', f_and(has_rank(18), f_plus(global_stat('scor'), player_stat('score'), 180000), has_medal('1031406_2'), has_medal('1031619_2'), has_medal('1031119_2'), has_medal('1031120_2'), has_medal('1031109_2'), has_medal('1031115_2'), has_medal('1031121_2'), has_medal('1031105_2'), has_medal('1031113_2'), global_stat('time', 4500000))),#stop
	(20, 'rank', f_and(has_rank(19), f_plus(global_stat('scor'), player_stat('score'), 200000), global_stat('time', 5184000))),#stop
	(21, 'rank', f_and(has_rank(20), f_plus(global_stat('scor'), player_stat('score'), 200000), has_medal('6666667'))),#stop
)#end