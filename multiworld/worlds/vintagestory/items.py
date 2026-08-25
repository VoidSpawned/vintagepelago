from __future__ import annotations
import random
from BaseClasses import Item, ItemClassification
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from .world import VintageWorld

#every item needs a name, an id, and a classification

item_list = []
ITEM_NAME_TO_ID = {}
DEFAULT_ITEM_CLASSIFICATIONS = {}

traps_list = [
    "Enemy Trap",
    "Drunk Trap",
    "Psychedelic Trap",
    "Nutrition Trap",
    "Hunger Trap",
    "Damage Trap"
    ]

progression_list = {
    #Stone Age
    "Cookpot",
    "Bowl",
    "Barrel",
    "Quern",
    "Crucible",
    #Copper Age
    "Copper Pickaxe",
    "Copper Saw",
    "Copper Hammer",
    "Chest",
    #Bronze
    "Black Bronze Pickaxe",
    "Map to Archives", #locatormap-resonancearchive
    "Tamed Elk",
    "Borax",
    "Bellow",
    #Iron
    "Iron Pickaxe",
    "Steel Pickaxe",
    "Forlorn Hope Estoc", #blade-forlorn-iron
    "Map to Lazaret", #letter-lazaret
    "Map to Devastation", #locatormap-devastationarea
    "Map to Tobias cave", #locatormap-cavetobias
    }

useful_list = {
    #Stone Age
    "Hunter Backpack",
    #Copper Age
    "Linen Sack",
    #Iron Age
    "leather Backpack",
    #Any
    "Bucket",
    "Copper Shears", #shears-copper
    "Copper Scythe", #scythe-copper
    #Traders
    "Agriculture Trader Item",
    "Artisan Trader Item",
    "Building Trader Item",
    "Clothing Trader Item",
    "Commodities Trader Item",
    "Furniture Trader Item",
    "Luxuries Trader Item",
    "Survival Trader Item",
    "Treasure Trader Item",
    "Bauxite Stones",
    }

trait_list = [
    "Focused",
    "Resourceful",
    "Fleetfooted",
    "Forager",
    "Pilferer",
    "Furtive",
    "Precise",
    "Technical",
    "Soldier",
    "Hardy",
    "Mender",
    "Bowyer",
    "Clothier",
    "Merciless",
    "Improviser",
    "Tinkerer"
    ]

filler_list = {
    "Prospecting Pick",
    "Fire Clay", 
    "Storage Vessel",
    "Crock",
    "Blue Clay",
    "Red Clay",
    "Copper Chisel",
    "Lime",
    "Coal",
    "Olivine",
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
    }

filler_with_clothes_list = {
    "Prospecting Pick",
    "Fire Clay",
    "Storage Vessel",
    "Crock",
    "Blue Clay",
    "Red Clay",
    "Copper Chisel",
    "Lime",
    "Coal",
    "Olivine",
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

def_item("Victory", ItemClassification.progression)
for each in progression_list:
    def_item(each, ItemClassification.progression)
for each in useful_list:
    def_item(each, ItemClassification.useful)
for each in traps_list:
    def_item(each, ItemClassification.trap)
for each in trait_list:
    def_item(each, ItemClassification.useful)
for each in filler_with_clothes_list:
    def_item(each)
for each in item_list:
    ITEM_NAME_TO_ID[each[0]] = each[1]
    DEFAULT_ITEM_CLASSIFICATIONS[each[0]] = each[2]


class VintageItem(Item):
    game: str = "Vintage Story"

def get_random_filler_item_name(world: VintageWorld) -> str:
    if world.options.clothingfill:
        return world.random.choice(list(filler_with_clothes_list))
    else:
        return world.random.choice(list(filler_list))

def create_item_with_correct_classification(world: VintageWorld, name: str) -> VintageItem:
    classification = DEFAULT_ITEM_CLASSIFICATIONS[name]
    
    return VintageItem(name, classification, ITEM_NAME_TO_ID[name], world.player)

def create_all_items(world: VintageWorld) -> None:
    #exactly as many items as there are locations
    itempool: list[Item] = []
    for each in progression_list:
        itempool.append(world.create_item(each))
    for each in trait_list:
        itempool.append(world.create_item(each))

    victory_item = create_item_with_correct_classification(world, "Victory")

    if world.options.victory == 0:
        victory_loc = world.get_location("Achievement: Iron Age")
    elif world.options.victory == 1:
        victory_loc = world.get_location("Achievement: Steel Age")
    elif world.options.victory == 2:
        victory_loc = world.get_location("Achievement: Mechanical Menace")
    elif world.options.victory == 3:
        victory_loc = world.get_location("Achievement: Lens Got")
    elif world.options.victory == 4:
        victory_loc = world.get_location("Achievement: Lens Delivered")

    victory_loc.place_locked_item(victory_item)

    number_of_items = len(itempool)
    number_of_unfilled_locations = len(world.multiworld.get_unfilled_locations(world.player))
    needed_number_of_filler_items = number_of_unfilled_locations - number_of_items

    for _ in range(needed_number_of_filler_items):
        r = random.randint(1, 100)
        if r < world.options.trapchance.value + 1:
            trapnumber = random.randint(1, 6)
            if trapnumber == 1 and world.options.traplist.value.__contains__("Enemy"):
                itempool.append(world.create_item(traps_list[0]))
            elif trapnumber == 2 and world.options.traplist.value.__contains__("Drunk"):
                itempool.append(world.create_item(traps_list[1]))
            elif trapnumber == 3 and world.options.traplist.value.__contains__("Psychedelic"):
                itempool.append(world.create_item(traps_list[2]))
            elif trapnumber == 4 and world.options.traplist.value.__contains__("Nutrition"):
                itempool.append(world.create_item(traps_list[3]))
            elif trapnumber == 5 and world.options.traplist.value.__contains__("Hunger"):
                itempool.append(world.create_item(traps_list[4]))
            elif trapnumber == 6 and world.options.traplist.value.__contains__("Damage"):
                itempool.append(world.create_item(traps_list[5]))
            else:
                itempool.append(world.create_filler())
        else:
            itempool.append(world.create_filler())

    world.multiworld.itempool += itempool

    if world.options.casual:
        starting_gear = world.create_item("Temporal Gear")
        starting_pick = world.create_item("Copper Pickaxe")
        starting_bag = world.create_item("Linen Sack")
        world.push_precollected(starting_gear)
        world.push_precollected(starting_pick)
        world.push_precollected(starting_bag)
