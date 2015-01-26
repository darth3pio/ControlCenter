""" 
	Generated With BF2 Statistics Medal Data Editor
	All Excess line breaks removed from medal definitions

	DONOT remove any #stop or #end tags. Doing so will break the
	Medal Data Editor from being able to parse this file.
"""

from bf2.stats.constants import *
from bf2 import g_debug

globalKeysNeeded = {}

# criteria functions

def player_score (player_attr, value=None):
	if value == None:
		def _player_score (player):
			return getattr (player.score, player_attr)
	else:
		def _player_score (player):
			return getattr (player.score, player_attr) >= value
	return _player_score

def player_stat (player_attr, value):
	def _player_stat (player):
		return getattr (player.stats, player_attr) >= value
	return _player_stat

def global_stat (stat_key, value=None):
	globalKeysNeeded[stat_key] = 1
	if value == None:
		def _global_stat (player):
			if stat_key in player.medals.globalKeys: return player.medals.globalKeys[stat_key]
			else: return 0
	else:
		def _global_stat (player):
			return stat_key in player.medals.globalKeys and player.medals.globalKeys[stat_key] >= value
	return _global_stat

def object_stat (object_type, item_attr, item_type, value=None):
	if value == None:
		def _object_stat (player):
			return getattr (getattr (player.stats, object_type)[item_type], item_attr)
	else:
		def _object_stat (player):
			return getattr (getattr (player.stats, object_type)[item_type], item_attr) >= value
	return _object_stat

def has_medal (id, level=1):
	def _has_medal (player):
		return id in player.medals.roundMedals and player.medals.roundMedals[id] >= level
	return _has_medal

def has_rank (rank):
	def _has_rank (player):
		return player.score.rank == rank
	return _has_rank

def times_awarded(id, player):
	if id in player.medals.roundMedals:
		return player.medals.roundMedals[id] # TIMES awarded, not level
	else:
		return 0

def global_stat_multiple_times (stat_key, value, id):
	globalKeysNeeded[stat_key] = 1
	def _global_stat_multiple_times (player):
		new_time_value = (value * (times_awarded(id, player)+1))
		return stat_key in player.medals.globalKeys and player.medals.globalKeys[stat_key] >= new_time_value
	return _global_stat_multiple_times


# logical functions

def f_and (*arg_list):
	def _f_and (player):
		res = True
		for f in arg_list:
			res = res and f(player)
		return res
	return _f_and

def f_or (*arg_list):
	def _f_or (player):
		res = True
		for f in arg_list:
			res = res or f(player)
		return res
	return _f_or

def f_not (f):
	def _f_not (player):
		return not f(player)
	return _f_not

def f_plus(a, b, value=None):
	if value == None:
		def _f_plus (player):
			return a(player) + b(player)
	else:
		def _f_plus (player):
			return a(player) + b(player) >= value
	return _f_plus

def f_div(a, b, value=None):
	if value == None:
		def _f_div (player):
			denominator = b(player)
			if denominator == 0: return a(player)+1
			else: return a(player) / denominator
	else:
		def _f_div (player):
			denominator = b(player)
			
			if denominator == 0: 
				return a(player)+1
			else: 
				return a(player) / denominator >= value

	return _f_div


# medal definitions

