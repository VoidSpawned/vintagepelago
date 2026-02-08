from __future__ import annotations
from BaseClasses import CollectionState, ItemClassification
from worlds.generic.Rules import add_rule, set_rule, add_item_rule
from typing import TYPE_CHECKING
if TYPE_CHECKING:
    from .world import VintageWorld

"""
    The general idea is that anvils represent progression through the ages.
    The goal is to get the steel anvil.
    As such, we will not send any anvils and we'll gate tools behind ages.
"""


def set_all_rules(world: VintageWorld) -> None:
    set_all_entrance_rules(world)
    set_all_location_rules(world)
    set_completion_condition(world)
    set_all_item_rules(world)

def set_all_entrance_rules(world: VintageWorld) -> None:
    #filler_to_stone = world.get_entrance("Filler to Stone")
    stone_to_copper = world.get_entrance("Stone to Copper")
    copper_to_bronze = world.get_entrance("Copper to Bronze")
    bronze_to_iron = world.get_entrance("Bronze to Iron")
    #iron_to_steel = world.get_entrance("Iron to Steel")

    set_rule(stone_to_copper, lambda state: state.has("Crucible", world.player))

    set_rule(copper_to_bronze, lambda state: state.has_any(["Ore Bomb", "Prospecting Pick", "Black Bronze Pickaxe", "Iron Pickaxe", "Steel Pickaxe"], world.player))
    
    set_rule(bronze_to_iron, lambda state: state.has_any(["Iron Pickaxe", "Steel Pickaxe"], world.player)
             and state.has("Quern", world.player))
                    #lambda = "i'm gonna make a one time function"
                    #so because it has to be a function in slot 2, we make one with one statement

def set_all_location_rules(world: VintageWorld) -> None:
    #steel_age = world.get_location("Steel Age")
    #add_rule(steel_age, lambda state: state.has("Barrel"), world.player)
    pass

def set_completion_condition(world: VintageWorld) -> None:
    world.multiworld.completion_condition[world.player] = lambda state: state.has("Victory", world.player)

def set_all_item_rules(world: VintageWorld) -> None:
    block_progression_items(world)

def block_progression_items(world:VintageWorld) -> None:
    for location in world.location_names: 
        if not "Achievement" in location: #achievements should always progress
            add_item_rule(world.multiworld.get_location(location, world.player), lambda item: ItemClassification.progression not in item.classification)
