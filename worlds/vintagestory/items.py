from __future__ import annotations
from BaseClasses import Item, ItemClassification
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from .world import VintageWorld

#every item needs a name, an id, and a classification

item_list = []
ITEM_NAME_TO_ID = {}
DEFAULT_ITEM_CLASSIFICATIONS = {}

##TODO: Add Traps 

progression_list = {
        #Stone Age
        "Cookpot",
        "Bowl",
        "Barrel",
        "Quern",
        "Crucible",
        "Hunter Backpack",
        #Copper Age
        "Copper Pickaxe",
        "Copper Saw",
        "Copper Hammer",
        "Chest",
        "Linen Sack",
        #Bronze
        "Black Bronze Pickaxe",
        #Iron
        "Iron Pickaxe",
        "Steel Pickaxe",
        "Forlorn Hope Estoc", #blade-forlorn-iron
        }

useful_list = {
        #Stone Age
        #Copper Age,
        #Iron Age
        #Any
        "Bucket",
        "Copper Shears", #shears-copper
        "Copper Scythe", #scythe-copper
        }

filler_list = {
        "Prospecting Pick",
        "Fire Clay", 
        "Storage Vessel",
        "Crock",
        "Blue Clay",
        "Red Clay",
        "Copper Chisel",
        "Lime",
        "Flax Seeds",
        "Coal",
       # "Pie", no attributed items yet
        "Temporal Gear",
        "Rusty Gear",
        "Ore Bomb",
        "Candle", #candle
        "Copper Ingot", #ingot-copper
        "Gold Nugget", #nugget-nativegold 
        "Silver Nugget", #nugget-nativesilver
        "Honeycomb", #honeycomb
        "Flax Twine", #flaxtwine
        "Leather", #leather-normal-plain
        "Lackey Hat", #clothes-head-lackey-hat
        "Nadiyan Beekeeper Hood", #clothes-nadiya-head-beekeeper
        "Sheepskull Mask", #clothes-face-sheepskull
        "Pillory", #clothes-neck-pillory
        "Fortune Teller Hip Scarf", #clothes-waist-fortune-teller-hip-scarf
        "Tophat", #clothes-head-tophat
        "Bamboo Cone Hat", #clothes-head-bamboo-conehat
        "Large Bamboo Cone Hat", #clothes-head-bamboo-conehat-large
        "Alchemist Hat", #clothes-head-alchemist
        "Fortune Teller Scarf", #clothes-head-fortune-tellers-scarf
        }

def def_item(name, classification = ItemClassification.filler):
    item_list.append([name, len(item_list)+1, classification])

for each in progression_list:
    def_item(each, ItemClassification.progression)
for each in useful_list:
    def_item(each, ItemClassification.useful)
for each in filler_list:
    def_item(each)
for each in item_list:
    ITEM_NAME_TO_ID[each[0]] = each[1]
    DEFAULT_ITEM_CLASSIFICATIONS[each[0]] = each[2]


class VintageItem(Item):
    game: str = "Vintage Story"

def get_random_filler_item_name(world: VintageWorld) -> str:
   return world.random.choice(list(filler_list))

def create_item_with_correct_classification(world: VintageWorld, name: str) -> VintageItem:
    classification = DEFAULT_ITEM_CLASSIFICATIONS[name]
    
    return VintageItem(name, classification, ITEM_NAME_TO_ID[name], world.player)

def create_all_items(world: VintageWorld) -> None:
    #exactly as many items as there are locations
    itempool: list[Item] = []
    for each in progression_list:
        itempool.append(world.create_item(each))

    number_of_items = len(itempool)
    number_of_unfilled_locations = len(world.multiworld.get_unfilled_locations(world.player))
    needed_number_of_filler_items = number_of_unfilled_locations - number_of_items

    itempool += [world.create_filler() for _ in range(needed_number_of_filler_items)]

    world.multiworld.itempool += itempool

    if world.options.casual:
        starting_gear = world.create_item("Temporal Gear")
        starting_pick = world.create_item("Copper Pickaxe")
        starting_bag = world.create_item("Linen Sack")
        world.push_precollected(starting_gear)
        world.push_precollected(starting_pick)
        world.push_precollected(starting_bag)
