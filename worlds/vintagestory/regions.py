from __future__ import annotations
from BaseClasses import Entrance, Region
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from .world import VintageWorld

def create_and_connect_regions(world: VintageWorld) -> None:
    create_all_regions(world)
    connect_regions(world)

def create_all_regions(world: VintageWorld) -> None:
    #filler = Region("Filler", world.player, world.multiworld)
    traders = Region("Traders", world.player, world.multiworld)
    stone_age = Region("Stone Age", world.player, world.multiworld)
    copper_age = Region("Copper Age", world.player, world.multiworld)
    bronze_age = Region("Bronze Age", world.player, world.multiworld)
    iron_age = Region("Iron Age", world.player, world.multiworld)

    regions = [stone_age, copper_age, bronze_age, iron_age, traders]

    world.multiworld.regions += regions

def connect_regions(world: VintageWorld) -> None:
    #filler = world.get_region("Filler")
    traders = world.get_region("Traders")
    stone_age = world.get_region("Stone Age")
    copper_age = world.get_region("Copper Age")
    bronze_age = world.get_region("Bronze Age")
    iron_age = world.get_region("Iron Age")

    #filler.connect(stone_age, "Filler to Stone")
    traders.connect(stone_age, "Trader to Stone")
    stone_age.connect(traders, "Stone to Trader")
    stone_age.connect(copper_age, "Stone to Copper")
    #stone_age.connect(filler, "Stone to Filler")
    copper_age.connect(bronze_age, "Copper to Bronze")
    bronze_age.connect(iron_age, "Bronze to Iron")
    #iron_age.connect(iron_age, "Iron to Steel")
    ##Can add a rule here??

