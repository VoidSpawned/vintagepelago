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
    "Achievement: Going Fishing",
    "Achievement: Got Fish",
    # "Achievement: Casting", #couldn't find a method to detect casting
    "Achievement: Charcoal",
    "Achievement: Defeat a Bear",
    "Achievement: Defeat a Shiver",
    "Achievement: Carrot Propagation",
    "Achievement: Flax Propagation",
    "Achievement: Onion Propagation",
    "Achievement: Spelt Propagation",
    "Achievement: Turnip Propagation",
    "Achievement: Parsnip Propagation",
    "Achievement: Rice Propagation",
    "Achievement: Rye Propagation",
    "Achievement: Soybean Propagation",
    "Achievement: Cabbage Propagation",
    "Achievement: Pumpkin Propagation",
    "Achievement: Sunflower Propagation",
    "Achievement: Fennel Propagation",
    "Achievement: Licorice Propagation",
    "Achievement: Carrot Farmer",
    "Achievement: Flax Farmer",
    "Achievement: Onion Farmer",
    "Achievement: Spelt Farmer",
    "Achievement: Turnip Farmer",
    "Achievement: Parsnip Farmer",
    "Achievement: Rice Farmer",
    "Achievement: Rye Farmer",
    "Achievement: Soybean Farmer",
    "Achievement: Cabbage Farmer",
    "Achievement: Pumpkin Farmer",
    "Achievement: Sunflower Farmer",
    "Achievement: Fennel Farmer",
    "Achievement: Licorice Farmer",
    "Achievement: Summer",
    "Achievement: Fall",
    "Achievement: Winter",
    "Achievement: One Year"
    ]

southern_crops_list = [
    "Achievement: Pineapple Propagation Hot Crop",
    "Achievement: Cassava Propagation Hot Crop",
    "Achievement: Amaranth Propagation Hot Crop",
    "Achievement: Peanut Propagation Hot Crop",
    "Achievement: Pineapple Farmer Hot Crop", 
    "Achievement: Cassava Farmer Hot Crop",
    "Achievement: Amaranth Farmer Hot Crop", 
    "Achievement: Peanut Farmer Hot Crop",
    ]

copper_age_list = [
    "Achievement: Copper Age",
    "Achievement: Pie",
    "Copper Bounty", #get any bountiful Copper chunk
    "Tin Bounty", #get any bountiful Tin chunk
    "Bismuth Bounty", #get any Bismuth bountiful chunk
    "Zinc Bounty", #get any Zinc bountiful chunk
    "Lead Bounty", #get any Lead bountiful chunk
    "Achievement: Quernal Sanders",
    "Achievement: Write a Book", 
    "Achievement: Lanterns",
    "Achievement: Cave Cleaning", #Destroy a locust nest
    "Achievement: Hare Rancher",
    "Achievement: Pig Rancher",
    "Achievement: Goat Rancher",
    "Achievement: Sheep Rancher",
    "Achievement: Chicken Rancher",
    ]
bronze_age_list = [
    "Achievement: Bronze Age",
    "Silver Bounty", #get any Silver bountiful chunk
    "Gold Bounty", #get any Gold bountiful chunk
    "Iron Bounty",  #get any Iron bountiful chunk
    "Achievement: Against the Storm",
    ]

story_first_list = [
    "Achievement: Mechanical Menace",
    "Achievement: Glider Schematics",
    ]

story_second_list = [
    "Achievement: Lens Got",
    "Achievement: Lens Delivered",
    ]

iron_age_list = [
    "Achievement: Iron Age",
    "Ilmenite Bounty",  #get any Ilmenite bountiful chunk
    "Chromite Bounty",  #get any Chromite bountiful chunk
    "Nickel Bounty",  #get any Nickel bountiful chunk
    "Achievement: Automation",
    "Achievement: Refractory", #get any type of refractory brick
    "Achievement: Steel Age" #steel anvil isn't real, it can't hurt you... So we detect a steel ingot instead
    ]

filler_list = [

    # "Chest", #probably don't even want these
    # "Vessel" #give this 4 or 5 slots marked as filler and reuse
    ]

lore_list = [
    "lore-1",
    "lore-2",
    "lore-3",
    "lore-4",
    "lore-5",
    "lore-6",
    "lore-7",
    "lore-8",
    "lore-9",
    "lore-10",
    "lore-11",
    "lore-12",
    "lore-13",
    "lore-14",
    "lore-15",
    "lore-16",
    "lore-17",
    "lore-18",
    "lore-19",
    "lore-20",
    "lore-21",
    "lore-22",
    "lore-23",
    "lore-24",
    "lore-25",
    "lore-26",
    "lore-27",
    "lore-28",
    "lore-29",
    "lore-30",
    "lore-31",
    "lore-32",
    "lore-33",
    "lore-34",
    "lore-35",
    "lore-36",
    "lore-37",
    "lore-38",
    "lore-39",
    "lore-40",
    "lore-41",
    "lore-42",
    "lore-43",
    "lore-44",
    "lore-45",
    "lore-46",
    "lore-47",
    "lore-48",
    "lore-49",
    "lore-50",
    "lore-51",
    "lore-52",
    "lore-53",
    "lore-54",
    "lore-55",
    "lore-56",
    "lore-57",
    "lore-58",
    "lore-59",
    "lore-60",
    "lore-61",
    "lore-62",
    "lore-63",
    "lore-64",
    "lore-65",
    "lore-66",
    "lore-67",
    "lore-68",
    "lore-69",
    "lore-70",
    "lore-71",
    "lore-72",
    "lore-73",
    "lore-74",
    "lore-75",
    "lore-76",
    "lore-77",
    "lore-78",
    "lore-79",
    "lore-80",
    "lore-81",
    "lore-82",
    "lore-83",
    "lore-84",
    "lore-85",
    "lore-86",
    "lore-87",
    "lore-88",
    "lore-89",
    "lore-90",
    "lore-91",
    "lore-92",
    "lore-93",
    "lore-94",
    "lore-95",
    "lore-96",
    "lore-97",
    "lore-98",
    "lore-99",
    "lore-100",
    "lore-101",
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

location_list = lore_list + stone_age_list + copper_age_list + bronze_age_list + southern_crops_list + story_first_list + story_second_list + iron_age_list + traders_list

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
    story_first_age = world.get_region("Story1 Age")
    story_second_age = world.get_region("Story2 Age")
    iron_age = world.get_region("Iron Age")
    
    #filler_locations = get_location_names_with_ids(filler_list+traders_list)
    #filler.add_locations(filler_locations, VintageLocation)
    traders_locations = get_location_names_with_ids(traders_list)
    traders.add_locations(traders_locations, VintageLocation)

    stone_age_locations = get_location_names_with_ids(stone_age_list)
    stone_age.add_locations(stone_age_locations, VintageLocation)

    copper_age_locations = get_location_names_with_ids(copper_age_list)
    copper_age.add_locations(copper_age_locations, VintageLocation)

    if world.options.cropsplus:
        southern_crop_locations = get_location_names_with_ids(southern_crops_list)
        copper_age.add_locations(southern_crop_locations, VintageLocation)

    bronze_age_locations = get_location_names_with_ids(bronze_age_list)
    bronze_age.add_locations(bronze_age_locations, VintageLocation)
    lore_locations = get_location_names_with_ids(lore_list)
    bronze_age.add_locations (lore_locations, VintageLocation)

    story_first_locations = get_location_names_with_ids(story_first_list)
    story_first_age.add_locations(story_first_locations, VintageLocation)
    story_second_locations = get_location_names_with_ids(story_second_list)
    story_second_age.add_locations(story_second_locations, VintageLocation)

    iron_age_locations = get_location_names_with_ids(iron_age_list)
    iron_age.add_locations(iron_age_locations, VintageLocation)

def create_events(world: VintageWorld) -> None:
    pass
