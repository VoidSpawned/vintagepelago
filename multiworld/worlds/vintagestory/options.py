from dataclasses import dataclass
from Options import Choice, OptionGroup, PerGameCommonOptions, Range, Toggle, DefaultOnToggle

class Casual(Toggle):
    """
        Start with a Temporal Gear, Copper Pickaxe, and Linen Sack
    """
    display_name = "Casual"

class Steel(DefaultOnToggle):
    """
        Must reach Steel to win
    """
    display_name = "Steel Required"

#class TemporalChance(Range):
#    """
#    Percentage chance of Temporal Gear in place of Filler item
#    """
#    display_name = "Temporal Chance"
#
#    range_start = 0
#    range_end = 100
#    default = 2

#class TraderProgression(Toggle):
#    """
#    Progression items can appear for sale or as quest rewards
#    """
#    display_name = "Trader Progression"

#class DungeonProgression(Toggle):
#    """
#    Progression items can appear in chests or vessels
#    """
#    display_name = "Dungeon Progression"

@dataclass
class VintageOptions(PerGameCommonOptions):
    casual: Casual
    steel: Steel
#    temporal_chance: TemporalChance
#    trader_progression: TraderProgression
#    dungeon_progression: DungeonProgression

option_groups = [
        OptionGroup(
            "Baby Options",
            [Casual])#, TemporalChance])
        ]
option_presets = {
        "Original": {
            "casual": False,
#            "temporal_chance": 2,
            "steel": True,
#            "trader_progression": True,
#            "dungeon_progression": False
            },
        "Spicy": {
            "casual": False,
#            "temporal_chance": 2,
            "steel": True,
#            "trader_progression": True,
#            "dungeon_progression": True
            },
        "Little Baby Wah Wah": {
            "casual": True,
#            "temporal_Chance": 30,
            "steel": False,
#            "trader_progression": False,
#            "dungeon_progression": False
            }
        }
