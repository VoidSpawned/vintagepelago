from collections.abc import Mapping
from typing import Any
from worlds.AutoWorld import World
from . import items, locations, options, regions, rules, web_world

class VintageWorld(World):
    """
    Vintage Story is Harvest Moon ++ for adults
    """
    
    game = "Vintage Story"
    web = web_world.VintageWebWorld()
    options_dataclass = options.VintageOptions
    options: options.VintageOptions

    location_name_to_id = locations.LOCATION_NAME_TO_ID
    item_name_to_id = items.ITEM_NAME_TO_ID

    origin_region_name = "Stone Age"

    def create_regions(self) -> None:
        regions.create_and_connect_regions(self)
        locations.create_all_locations(self)

    def set_rules(self) -> None:
        rules.set_all_rules(self)

    def create_items(self) -> None:
        items.create_all_items(self)

    def create_item(self, name: str) -> items.VintageItem:
        return items.create_item_with_correct_classification(self, name)

    def get_filler_item_name(self) -> str:
        return items.get_random_filler_item_name(self)

    def fill_slot_data(self) -> Mapping[str, Any]:
        return self.options.as_dict("casual", "steel")#, "temporal_chance", "trader_progression")
