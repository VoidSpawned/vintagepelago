from dataclasses import dataclass
from Options import Choice, OptionGroup, PerGameCommonOptions, Range, Toggle, DefaultOnToggle

class Casual(Toggle):
    """
        Start with a Temporal Gear, Copper Pickaxe, and Linen Sack.
    """
    display_name = "Casual"

class Prices(Range):
    """
        Price for AP items from traders. 
        Higher prices will require more rusty gear grinding.
        Bought AP items will restock to vanilla trades on trader restock)
    """
    display_name = "Price for the traders AP items"
    range_start = 1
    range_end = 100
    default = 10

class LoreProgression(Range):
    """
        Amount of lore checks that can try to have progression checks before we fill the rest with non-progression.
    """
    display_name = "Amount of lore discovery checks that can contain progression items"
    range_start = 0
    range_end = 101
    default = 0

class VictoryCondition(Choice):
    """
        0 = Must reach Iron_Age. (use an iron anvil)
        1 = Must reach Steel to win.
        2 = Must kill Chapter 1 boss.
        3 = Must kill Chapter 2 boss.
        4 = Must deliver Chapter 2 boss item to Tobias.
    """
    display_name = "Victory Condition"
    option_iron = 0
    option_steel = 1
    option_chapter1 = 2
    option_chapter2 = 3
    option_lens = 4
    default = 0

#class TemporalChance(Range):
#    """
#    Percentage chance of Temporal Gear in place of Filler item
#    """
#    display_name = "Temporal Chance"
#
#    range_start = 0
#    range_end = 100
#    default = 2

class TraderProgression(Toggle):
    """
    Progression items can appear for sale or as quest rewards.
    Keep in mind that this can make getting these items as tedious / grindy.
    """
    display_name = "Trader Progression"

#class DungeonProgression(Toggle):
#    """
#    Progression items can appear in chests or vessels.
#    This can make progression very rng dependant.
#    """
#    display_name = "Dungeon Progression"

@dataclass
class VintageOptions(PerGameCommonOptions):
    casual: Casual
    victory: VictoryCondition
    prices: Prices
    lore_progression: LoreProgression
#    temporal_chance: TemporalChance
    trader_progression: TraderProgression
#    dungeon_progression: DungeonProgression

option_groups = [
        OptionGroup(
            "Quick option",
            [Casual])#, TemporalChance])
        ]
option_presets = {
        "Original": {
            "casual": False,
#            "temporal_chance": 2,
            "victory": 1,
            "lore_progression": 40,
            "trader_progression": False,
#            "dungeon_progression": False
            },
        "Spicy": {
            "casual": False,
#            "temporal_chance": 2,
            "victory": 4,
            "lore_progression": 101,
            "trader_progression": True,
#            "dungeon_progression": True
            },
        "Quicker game": {
            "casual": 0,
#            "temporal_Chance": 30,
            "victory": 0,
            "lore_progression": 0,
            "trader_progression": False,
#            "dungeon_progression": False
            }
        }
