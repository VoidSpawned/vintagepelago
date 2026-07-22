using Archipelago.MultiClient.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
//
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace Multiworld
{
    public class MultiworldSystem : ModSystem
    {
        ICoreClientAPI capi;
        ICoreServerAPI sapi;
		IPlayer Player;
		Infranet infranet = new Infranet();
		APClient ap_client = new APClient();


		public Dictionary<string, string> UseBlockAchievements = new Dictionary<string, string> 
		{
			{"quern-granite","Quernal Sanders"},
			{"quern-andesite","Quernal Sanders"},
			{"quern-basalt","Quernal Sanders"},
			{"quern-peridotite","Quernal Sanders"},
			{"anvil-copper","Copper Age"},
			{"anvil-tinbronze","Bronze Age"},
			{"anvil-bismuthbronze","Bronze Age"},
			{"anvil-blackbronze","Bronze Age"},
			{"anvil-iron","Iron Age"},
			//{"anvil-steel","Steel Age"} no such thing in survival!
		};

		public Dictionary<string, string> ItemPickupAchievements = new Dictionary<string, string>
		{
			{"claypot-blue-fired","Stone Age"},
			{"claypot-fire-fired","Stone Age"},
			{"claypot-black-fired","Stone Age"},
			{"claypot-brown-fired","Stone Age"},
			{"claypot-cream-fired","Stone Age"},
            {"claypot-earthyorange-fired","Stone Age"},
			{"claypot-gray-fired","Stone Age"},
			{"claypot-orange-fired","Stone Age"},
			{"claypot-red-fired","Stone Age"},
			{"claypot-tan-fired","Stone Age"},
			{"bowl-blue-meal","Cook a Meal"},
			{"bowl-fire-meal","Cook a Meal"},
			{"bowl-black-meal","Cook a Meal"},
			{"bowl-brown-meal","Cook a Meal"},
			{"bowl-cream-meal","Cook a Meal"},
            {"bowl-earthyorange-meal","Cook a Meal"},
			{"bowl-gray-meal","Cook a Meal"},
			{"bowl-orange-meal","Cook a Meal"},
			{"bowl-red-meal","Cook a Meal"},
			{"bowl-tan-meal","Cook a Meal"},
			{"wateringcan-blue-fired","Farming"},
			{"wateringcan-fire-fired","Farming"},
			{"wateringcan-black-fired","Farming"},
			{"wateringcan-brown-fired","Farming"},
			{"wateringcan-cream-fired","Farming"},
            {"wateringcan-earthyorange-fired","Farming"},
			{"wateringcan-gray-fired","Farming"},
			{"wateringcan-orange-fired","Farming"},
			{"wateringcan-red-fired","Farming"},
			{"wateringcan-tan-fired","Farming"},
			{"charcoal","Charcoal"},
		//	{"","Casting"},		//no good method for detecing the pour
			{"pie-perfect","Pie"},
			{"lantern-up","Lanterns"},
			{"windmillrotor-wood-north","Automation"},
            {"windmillrotor-metal-north","Automation"},
            {"inkandquill", "Write a Book"},
			{"fishingpole-simple-wood" , "Going Fishing"},
            {"fishingpole-simple-bamboo" , "Going Fishing"},
            {"fishraw-reef-clown-common-adult" , "Got Fish"},
            {"fishraw-saltwater-haddock-common-adult" , "Got Fish"},
            {"fishraw-freshwater-carp-common-adult" , "Got Fish"},
            {"fishraw-freshwater-walleye-common-adult" , "Got Fish"},
            {"fishraw-saltwater-mahi-mahi-common-adult" , "Got Fish"},
            {"fishraw-saltwater-coelacanth-common-adult" , "Got Fish"},
            {"fishraw-reef-tang-banded-adult" , "Got Fish"},
            {"fishraw-reef-angel-bicolor-adult" , "Got Fish"},
            {"fishraw-reef-clown-black-adult" , "Got Fish"},
            {"fishraw-freshwater-crappie-black-adult" , "Got Fish"},
            {"fishraw-freshwater-piranha-black-adult" , "Got Fish"},
            {"fishraw-saltwater-grouper-black-adult" , "Got Fish"},
            {"fishraw-freshwater-sheatfish-black-adult" , "Got Fish"},
            {"fishraw-reef-butterfly-blackwedged-adult" , "Got Fish"},
            {"fishraw-reef-butterfly-copperband-adult" , "Got Fish"},
            {"fishraw-reef-wrasse-creole-adult" , "Got Fish"},
            {"fishraw-reef-puffer-longspine-adult" , "Got Fish"},
            {"fishraw-reef-tang-powderblue-adult" , "Got Fish"},
            {"fishraw-reef-trigger-titan-adult" , "Got Fish"},
            {"fishraw-reef-clown-yellowstripe-adult" , "Got Fish"},
            {"fishraw-saltwater-pollock-alaska-adult" , "Got Fish"},
            {"fishraw-saltwater-herring-atlantic-adult" , "Got Fish"},
            {"fishraw-saltwater-mackerel-atlantic-adult" , "Got Fish"},
            {"fishraw-saltwater-wreckfish-atlantic-adult" , "Got Fish"},
            {"fishraw-saltwater-sturgeon-atlantic-adult" , "Got Fish"},
            {"fishraw-saltwater-gurnard-cape-adult" , "Got Fish"},
            {"fishraw-saltwater-perch-pacific-adult" , "Got Fish"},
            {"fishraw-saltwater-bream-sea-adult" , "Got Fish"},
            {"fishraw-saltwater-hake-silver-adult" , "Got Fish"},
            {"fishraw-freshwater-trout-brown-adult" , "Got Fish"},
            {"fishraw-freshwater-perch-european-adult" , "Got Fish"},
            {"fishraw-freshwater-trout-rainbow-adult" , "Got Fish"},
			{"fishraw-freshwater-chub-river-adult" , "Got Fish"},
			{"fishraw-freshwater-alewife-shad-adult" , "Got Fish"},
			{"fishraw-freshwater-crappie-white-adult" , "Got Fish"},
			{"fishraw-freshwater-sheatfish-white-adult" , "Got Fish"},
			{"fishraw-freshwater-perch-yellow-adult" , "Got Fish"},
			{"fishraw-saltwater-wolf-bering-adult" , "Got Fish"},
			{"fishraw-saltwater-barracuda-great-adult" , "Got Fish"},
			{"fishraw-saltwater-salmon-pink-adult" , "Got Fish"},
			{"fishraw-freshwater-piranha-red-adult" , "Got Fish"},
			{"fishraw-saltwater-snapper-red-adult" , "Got Fish"},
			{"fishraw-freshwater-tilapia-red-adult" , "Got Fish"},
			{"fishraw-saltwater-tuna-skipjack-adult" , "Got Fish"},
			{"fishraw-freshwater-catfish-blue-adult" , "Got Fish"},
			{"fishraw-freshwater-pickerel-chain-adult" , "Got Fish"},
			{"fishraw-freshwater-catfish-channel-adult" , "Got Fish"},
			{"fishraw-freshwater-salmon-coho-adult" , "Got Fish"},
			{"fishraw-freshwater-carp-grass-adult" , "Got Fish"},
			{"fishraw-freshwater-bass-largemouth-adult" , "Got Fish"},
			{"fishraw-freshwater-tilapia-nile-adult" , "Got Fish"},
			{"fishraw-freshwater-bass-smallmouth-adult" , "Got Fish"},
			{"fishraw-saltwater-amberjack-yellowtail-adult" , "Got Fish"},
			{"fishraw-freshwater-pike-northern-adult" , "Got Fish"},
			{"fishraw-freshwater-arapaima-arapaima-adult" , "Got Fish"},
			{"fishraw-freshwater-arapaima-gigas-adult" , "Got Fish"},
			{"fishraw-freshwater-carp-common-juvenile" , "Got Fish"},
			{"fishraw-freshwater-walleye-common-juvenile" , "Got Fish"},
			{"fishraw-saltwater-mahi-mahi-common-juvenile" , "Got Fish"},
			{"fishraw-saltwater-coelacanth-common-juvenile" , "Got Fish"},
			{"fishraw-saltwater-grouper-black-juvenile" , "Got Fish"},
			{"fishraw-freshwater-sheatfish-black-juvenile" , "Got Fish"},
			{"fishraw-saltwater-wreckfish-atlantic-juvenile" , "Got Fish"},
			{"fishraw-saltwater-sturgeon-atlantic-juvenile" , "Got Fish"},
			{"fishraw-freshwater-sheatfish-white-juvenile" , "Got Fish"},
			{"fishraw-saltwater-wolf-bering-juvenile" , "Got Fish"},
			{"fishraw-saltwater-barracuda-great-juvenile" , "Got Fish"},
			{"fishraw-saltwater-salmon-pink-juvenile" , "Got Fish"},
			{"fishraw-saltwater-snapper-red-juvenile" , "Got Fish"},
			{"fishraw-saltwater-tuna-skipjack-juvenile" , "Got Fish"},
			{"fishraw-freshwater-catfish-blue-juvenile" , "Got Fish"},
			{"fishraw-freshwater-pickerel-chain-juvenile" , "Got Fish"},
			{"fishraw-freshwater-catfish-channel-juvenile" , "Got Fish"},
			{"fishraw-freshwater-salmon-coho-juvenile" , "Got Fish"},
			{"fishraw-freshwater-carp-grass-juvenile" , "Got Fish"},
			{"fishraw-freshwater-bass-largemouth-juvenile" , "Got Fish"},
			{"fishraw-freshwater-bass-smallmouth-juvenile" , "Got Fish"},
			{"fishraw-saltwater-amberjack-yellowtail-juvenile" , "Got Fish"},
			{"fishraw-freshwater-pike-northern-juvenile" , "Got Fish"},
			{"fishraw-freshwater-arapaima-arapaima-juvenile" , "Got Fish"},
			{"fishraw-freshwater-arapaima-gigas-juvenile" , "Got Fish"},
            {"ingot-steel","Steel Age"} //placeholder win condition
		};

		public Dictionary<string, string> KillAchievements = new Dictionary<string, string>
		{
			{"bear-brown-adult-female","Defeat a Bear"},
			{"bear-brown-adult-male","Defeat a Bear"},
			{"bear-black-adult-female","Defeat a Bear"},
			{"bear-black-adult-male","Defeat a Bear"},
			{"bear-sun-adult-female","Defeat a Bear"},
			{"bear-sun-adult-male","Defeat a Bear"},
			{"bear-panda-adult-female","Defeat a Bear"},
			{"bear-panda-adult-male","Defeat a Bear"},
			{"bear-polar-adult-female","Defeat a Bear"},
			{"bear-polar-adult-male","Defeat a Bear"},
			{"shiver-surface","Defeat a Shiver"},
			{"shiver-deep","Defeat a Shiver"},
			{"shiver-tainted","Defeat a Shiver"},
			{"shiver-corrupt","Defeat a Shiver"},
			{"shiver-nightmare","Defeat a Shiver"},
			{"shiver-stilt","Defeat a Shiver"},
			{"shiver-bellhead","Defeat a Shiver"},
			{"shiver-deepsplit","Defeat a Shiver"},
			//	any corrupted : Against the Storm is handled manually in the kill check
		};


        // Called on server and client
        public override void Start(ICoreAPI api)
        {
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            sapi = api;
            sapi.Event.PlayerJoin += OnPlayerJoin;
            sapi.Event.DidUseBlock += OnDidUseBlock;
			sapi.Event.OnEntityDeath += OnEntityDeath;
			sapi.Event.OnPlayerInteractEntity += OnPlayerInteractEntity;
			sapi.Event.PlayerDisconnect += OnPlayerDisconnect;
			sapi.Event.RegisterGameTickListener(TimeAchievementCheck, 1000*60);
			sapi.Event.RegisterGameTickListener(APInventoryCheck, 1000*3);

			infranet.init_server(api);
		}

        public override void StartClientSide(ICoreClientAPI api)
        {
            capi = api; 
	    
			capi.ChatCommands
				.Create("apregister")
				.WithDescription("Send randomizer info to the server")
				.HandleWith(infranet.OnAPRegister);
		
			capi.ChatCommands
				.Create("aplogin")
				.WithDescription("Connect client to Archipelago")
				.HandleWith(infranet.login);

			capi.ChatCommands
				.Create("apname")
				.WithDescription("Set the Archipelago slotname")
				.WithArgs(capi.ChatCommands.Parsers.OptionalAll("apname"))
				.HandleWith(infranet.apname);
		
			capi.ChatCommands
				.Create("apip")
				.WithDescription("Set the Archipelago IP")
				.WithArgs(capi.ChatCommands.Parsers.Word("ip"))
				.HandleWith(infranet.set_ip);
		
			capi.ChatCommands
				.Create("apport")
				.WithDescription("Set the Archipelago Port")
				.WithArgs(capi.ChatCommands.Parsers.Int("port"))
				.HandleWith(infranet.set_port);

			capi.ChatCommands
				.Create("appassword")
				.WithDescription("Sets the Archipelago password")
				.WithArgs(capi.ChatCommands.Parsers.OptionalAll("password"))
				.HandleWith(infranet.set_pass);

			capi.ChatCommands
				.Create("aplocations")
				.WithDescription("Print a list of remaining checks")
				.HandleWith(infranet.print_remaining_locations);
	    
	    
	    
			capi.Event.RegisterGameTickListener(ap_client.Flush, 1000*3);	    
			capi.Event.RegisterGameTickListener(infranet.RequestAPItemReceive, 1000*3);	    
			capi.Event.LevelFinalize += OnLevelFinalize;
			capi.Event.LeaveWorld += OnLeaveWorld;
			capi.Event.InGameDiscovery += OnPlayerDiscovery;

			infranet.init_client(api, ap_client);
		}

		public void OnPlayerJoin(IServerPlayer player)
		{
			Dictionary<string, Dictionary<string, string>> spoilers = sapi.LoadModConfig<Dictionary<string, Dictionary<string, string>>>(player.PlayerName+"_spoilers.json");
			if(spoilers == null)
			{
				spoilers = new Dictionary<string, Dictionary<string, string>>();
				sapi.StoreModConfig(spoilers, player.PlayerName+"_spoilers.json");
			}
		}

		public void OnPlayerDisconnect(IServerPlayer player)
			{
				ap_client.APDisconnect();
			}

		public void APInventoryCheck(float dt)
		{ //Check every inventory of every online player for foreign AP items and turn them into location checks
			string[] inventories = ["backpack", "character", "hotbar"];
			foreach(IServerPlayer player in sapi.World.AllOnlinePlayers)
			{
				if(player.ConnectionState != EnumClientState.Playing)
					continue;
		
				foreach(var inv in player.InventoryManager.Inventories)
				{
					if(inv.Key.Contains("hotbar") || inv.Key.Contains("backpack") || inv.Key.Contains("character"))
					{
						foreach(ItemSlot slot in inv.Value)
						{
							if(slot != null && slot.Itemstack != null)
							{
								ItemStack stack = slot.Itemstack;
								if( stack.Attributes["ap_item"] != null && stack.Attributes.GetBool("ap_item", false))
								{
									TriggerAchievement(stack.Attributes.GetString("location"), player);
									slot.Itemstack = null;
									slot.MarkDirty();
								}
								else 
								{
									string code = "";
									switch(stack.Class)
									{
										case EnumItemClass.Block:
											code = stack.Block.Code.ToString();
											break;
										case EnumItemClass.Item:
											code = stack.Item.Code.ToString();
											break;
									}
									code = code.Split(':')[1];
									if(ItemPickupAchievements.Keys.Contains(code))
										TriggerAchievement("Achievement: " + ItemPickupAchievements[code], player);
								}
							}	
						}
				
					}
				}	
			}
		}

		public void TimeAchievementCheck(float dt)
		{ //Check for time-based achievements like season and 1 year
			int startYear = 1386;
			int yearsPassed = sapi.World.Calendar.Year - startYear;
			bool one_year = (yearsPassed >= 1);
			foreach(IServerPlayer p in sapi.World.AllOnlinePlayers)
			{
				BlockPos pos = p.Entity.Pos.AsBlockPos;
				string cheevo = "";
				switch(sapi.World.Calendar.GetSeason(pos))
				{
					case EnumSeason.Summer:
						cheevo = "Achievement: Summer";
						break;
					case EnumSeason.Fall:
						cheevo = "Achievement: Fall";
						break;
					case EnumSeason.Winter:
						cheevo = "Achievement: Winter";
						break;
					default:
						break;
				}
					if(cheevo != "") {
						TriggerAchievement(cheevo, p);
								}
				if(one_year)
					TriggerAchievement("Achievement: One Year", p);
			}
		}


		public void OnEntityDeath(Entity entity, DamageSource source)
		{	//Check for kill achievements like Defeat a Bear
			//Console.WriteLine(entity.Code.ToString());
			if(source.GetCauseEntity() is EntityPlayer ePlayer and not null)
				{
					string code = entity.Code.ToString().Split(':')[1];
					IServerPlayer player = (IServerPlayer)sapi.World.PlayerByUid(ePlayer.PlayerUID);
					if(KillAchievements.Keys.Contains(code))
						TriggerAchievement("Achievement: "+KillAchievements[code], player);
					if(code.Contains("corrupt"))
						TriggerAchievement("Achievement: Against the Storm", player);
				}
		}

		public void OnDidUseBlock(IServerPlayer byPlayer, BlockSelection blockSel)
		{	//Check for block use like anvils that lead to Age achievements
			string block = byPlayer.CurrentBlockSelection.Block.Code.ToString().Split(':')[1];
			if(UseBlockAchievements.Keys.Contains(block))
			{
				TriggerAchievement("Achievement: "+UseBlockAchievements[block], byPlayer);
			}
		}

		public void TriggerAchievement(string loc, IServerPlayer byPlayer)
		{	//Dispatch a packet from the server to the client, notifying that an achievement has been reached
			Dictionary<string, Dictionary<string, string>> loc_list = sapi.LoadModConfig<Dictionary<string, Dictionary<string, string>>>(byPlayer.PlayerName+"_spoilers.json");
			if(loc_list.ContainsKey(loc))
			{
				sapi.SendIngameDiscovery(byPlayer,"location", loc);
				infranet.ServerSendLocationCheck(loc, byPlayer.PlayerName);
				loc_list.Remove(loc);
				sapi.StoreModConfig(loc_list, byPlayer.PlayerName+"_spoilers.json");
			}
		}

		public void OnPlayerDiscovery(object sender, string discoverycode, string text)
		{   //When a player gets a lore discovery doesn't matter which send a check
			JsonObject APConfig = sapi.LoadModConfig("multiworld_config.json");
            Dictionary<string, Dictionary<string, string>> loc_list = sapi.LoadModConfig<Dictionary<string, Dictionary<string, string>>>(APConfig["VSName"] + "_spoilers.json");
			if (loc_list.Keys.Contains("lore-"))
			{
				Console.WriteLine("Did I even get here");	
			}

        }

		public void OnPlayerInteractEntity(Entity entity, IPlayer byPlayer, ItemSlot slot, Vec3d hitPosition, int mode, ref EnumHandling handling)
		{	//When a player interacts with a Trader, replace their selling slots with the player's foreign AP item list
			if(entity is EntityTrader trader)
			{
				Dictionary<string, Dictionary<string, string>> spoilers = sapi.LoadModConfig<Dictionary<string, Dictionary<string, string>>>(byPlayer.PlayerName+"_spoilers.json");
				JsonObject APConfig = sapi.LoadModConfig("multiworld_config.json");

				int defaultPrice = 10;
				int prices = APConfig["traderPrices"].AsInt(defaultPrice);

				var inv = trader.Inventory as InventoryTrader;

				foreach(var kvp in spoilers)
				{
					string match = "";
					switch(trader.Code.ToString().Split(':')[1])
					{
						case "trader-male-agriculture-cold":
						case "trader-female-agriculture-cold":
                        case "trader-male-agriculture-temperate":
                        case "trader-female-agriculture-temperate":
                        case "trader-male-agriculture-desert":
                        case "trader-female-agriculture-desert":
                            match = "Agriculture";
							break;
                        case "trader-male-artisan-cold":
                        case "trader-female-artisan-cold":
                        case "trader-male-artisan-temperate":
                        case "trader-female-artisan-temperate":
                        case "trader-male-artisan-desert":
                        case "trader-female-artisan-desert":
                            match = "Artisan";
							break;
                        case "trader-male-buildmaterials-cold":
                        case "trader-female-buildmaterials-cold":
                        case "trader-male-buildmaterials-temperate":
                        case "trader-female-buildmaterials-temperate":
                        case "trader-male-buildmaterials-desert":
                        case "trader-female-buildmaterials-desert":
                            match = "Building";
							break;
                        case "trader-male-clothing-cold":
                        case "trader-female-clothing-cold":
                        case "trader-male-clothing-temperate":
                        case "trader-female-clothing-temperate":
                        case "trader-male-clothing-desert":
                        case "trader-female-clothing-desert":
                            match = "Clothing";
							break;
                        case "trader-male-commodities-cold":
                        case "trader-female-commodities-cold":
                        case "trader-male-commodities-temperate":
                        case "trader-female-commodities-temperate":
                        case "trader-male-commodities-desert":
                        case "trader-female-commodities-desert":
                            match = "Commodities";
							break;
                        case "trader-male-furniture-cold":
                        case "trader-female-furniture-cold":
                        case "trader-male-furniture-temperate":
                        case "trader-female-furniture-temperate":
                        case "trader-male-furniture-desert":
                        case "trader-female-furniture-desert":
                            match = "Furniture";
							break;
                        case "trader-male-luxuries-cold":
                        case "trader-female-luxuries-cold":
                        case "trader-male-luxuries-temperate":
                        case "trader-female-luxuries-temperate":
                        case "trader-male-luxuries-desert":
                        case "trader-female-luxuries-desert":
                            match = "Luxuries";
							break;
                        case "trader-male-survivalgoods-cold":
                        case "trader-female-survivalgoods-cold":
                        case "trader-male-survivalgoods-temperate":
                        case "trader-female-survivalgoods-temperate":
                        case "trader-male-survivalgoods-desert":
                        case "trader-female-survivalgoods-desert":
                            match = "Survival";
							break;
                        case "trader-male-treasurehunter-cold":
                        case "trader-female-treasurehunter-cold":
                        case "trader-male-treasurehunter-temperate":
                        case "trader-female-treasurehunter-temperate":
                        case "trader-male-treasurehunter-desert":
                        case "trader-female-treasurehunter-desert":
                            match = "Treasure";
							break;
					}
					if(match == "")
						continue;	
					if(kvp.Key.Contains(match + " Trader"))
					{
						string[] numString = kvp.Key.Split(' ');
						int slotNum = 0;
						if(numString.Length > 2)
						slotNum = int.Parse(numString[2]) - 1;
						inv[slotNum].Itemstack = null;
						SetAPSellSlot(inv.SellingSlots[slotNum], kvp.Key, kvp.Value, prices);
					}
				}


            		ITreeAttribute tree = trader.WatchedAttributes["traderInventory"] as ITreeAttribute;
           	 		trader.Inventory.ToTreeAttributes(tree);
           			trader.WatchedAttributes.MarkAllDirty();
			}

		}

		public void SetAPSellSlot(ItemSlotTrade slot, string location, Dictionary<string, string> itemDict, int prices){ //Replace a Trader slot with a foreign AP item placeholder

			string title = itemDict["ItemDisplayName"] + " for " + itemDict["APOwnerName"];
			string code = "game:book-normal-brickred";
			switch(itemDict["Classification"])
			{
				case "Progression":
					title = "Progressive "+ title;
					code = "game:book-normal-purple";
					break;
				case "Useful":
					title = "Useful "+ title;
					code = "game:book-normal-purpleorange";
					break;
				case "Filler":
					title = "Filler "+ title;
					code = "game:book-normal-orangebrown";
					break;
				case "Trap":
					title = "Trap "+ title;
					code = "game:book-normal-gray";
					break;
			}

				Item i = sapi.World.GetItem(new AssetLocation(code));
				ItemStack stack = new ItemStack(i, 1);
				stack.Attributes["title"] = new StringAttribute(title);
				stack.Attributes["ap_item"] = new BoolAttribute(true);
				stack.Attributes["location"] = new StringAttribute(location);
				var resolved = new ResolvedTradeItem();
				resolved.Stack = stack.Clone();
				resolved.Price = prices;
				resolved.Stock = 1;
				resolved.Restock = new RestockOpts();
				resolved.SupplyDemand = new SupplyDemandOpts();

				slot.SetTradeItem(resolved);
				slot.MarkDirty();
			}
	
		public void OnLevelFinalize()
		{ //Wait until the world is really ready to do any AP networking
			ap_client.init(capi);
			TextCommandCallingArgs args = new TextCommandCallingArgs();
			infranet.OnAPRegister(args);
		}
		public void OnLeaveWorld()
		{
			
		}

    }
}
