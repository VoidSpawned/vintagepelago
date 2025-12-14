from __future__ import annotations
from BaseClasses import ItemClassification, Location
from . import items
from typing import TYPE_CHECKING
if TYPE_CHECKING:
    from .world import VintageWorld
##TODO: Traders, dungeon slots
stone_age_list = [
        "Achievement: Stone Age",
        "Achievement: Cook a Meal",
        "Achievement: Farming",
       # "Achievement: Casting", #couldn't find a method to detect casting
        "Achievement: Charcoal",
        "Achievement: Defeat a Bear",
        "Achievement: Defeat a Shiver", 
        "Achievement: Summer",
        "Achievement: Fall",
        "Achievement: Winter",
        "Achievement: One Year"
        ]

copper_age_list = [
        "Achievement: Copper Age",
        "Achievement: Pie",
        "Achievement: Quernal Sanders",
        "Achievement: Write a Book", 
        "Achievement: Lanterns"
        ]
bronze_age_list = [
        "Achievement: Bronze Age",
        "Achievement: Against the Storm"
        ]

iron_age_list = [
        "Achievement: Iron Age",
        "Achievement: Automation",
        #"Achievement: Steel Age" #steel anvil isn't real, what to detect?
        ]

filler_list = [

       # "Chest", #probably don't even want these
       # "Vessel" #give this 4 or 5 slots marked as filler and reuse
        ]

traders_base = [
        "Agriculture Trader",
        "Artisan Trader",
        "Building Trader",
        "Clothing Trader",
        "Commodities Trader",
        "Furniture Trader",
        "Luxuries Trader",
        "Survival Trader",
        "Treasure Trader",
        ]

traders_list = []

for name in traders_base:
    for i in range(1, 17):
        traders_list.append(f"{name} {i}")

location_list = stone_age_list + copper_age_list + bronze_age_list + iron_age_list + traders_list

LOCATION_NAME_TO_ID = {name: i+1 for i, name in enumerate(location_list)}

class VintageLocation(Location):
    game = "Vintage Story"

def get_location_names_with_ids(location_names: list[str]) -> dict[str, int | None]:
    return {location_name: LOCATION_NAME_TO_ID[location_name] for location_name in location_names}

def create_all_locations(world: VintageWorld) -> None:
    create_regular_locations(world)
    create_events(world)

def create_regular_locations(world: VintageWorld) -> None:
    #filler = world.get_region("Filler")
    traders = world.get_region("Traders")
    stone_age = world.get_region("Stone Age")
    copper_age = world.get_region("Copper Age")
    bronze_age = world.get_region("Bronze Age")
    iron_age = world.get_region("Iron Age")
    
    #filler_locations = get_location_names_with_ids(filler_list+traders_list)
    #filler.add_locations(filler_locations, VintageLocation)
    traders_locations = get_location_names_with_ids(traders_list)
    traders.add_locations(traders_locations, VintageLocation)
    stone_age_locations = get_location_names_with_ids(stone_age_list)
    stone_age.add_locations(stone_age_locations, VintageLocation)
    copper_age_locations = get_location_names_with_ids(copper_age_list)
    copper_age.add_locations(copper_age_locations, VintageLocation)
    bronze_age_locations = get_location_names_with_ids(bronze_age_list)
    bronze_age.add_locations(bronze_age_locations, VintageLocation)
    iron_age_locations = get_location_names_with_ids(iron_age_list)
    iron_age.add_locations(iron_age_locations, VintageLocation)

def create_events(world: VintageWorld) -> None:
    steel_age = world.get_region("Iron Age")
    steel_age.add_event("Victory", "Victory", location_type=VintageLocation, item_type=items.VintageItem)
