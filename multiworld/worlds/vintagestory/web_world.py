from BaseClasses import Tutorial
from worlds.AutoWorld import WebWorld
from .options import option_groups, option_presets

class VintageWebWorld(WebWorld):
    game = "Vintage Story"
    theme = "stone"

    setup_en = Tutorial(
            "Vintage Story Archipelago", #title
            "A Guide to VSA", #description
            "English", #language
            "setup_en.md", #filepath
            "setup/en", #link
            ["Crabsoft"], #authors
            )

    tutorials = [setup_en]

    option_groups = option_groups
    option_presets = option_presets
