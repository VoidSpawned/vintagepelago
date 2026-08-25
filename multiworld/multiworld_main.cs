using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.ExceptionServices;


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
using Vintagestory.Server;

namespace Multiworld
{
	public class MultiworldSystem : ModSystem
	{
        ICoreClientAPI capi;
		ICoreServerAPI sapi;
		IPlayer Player;
		public static ICoreAPI dapi { get; protected set;}
		Infranet infranet = new Infranet();
		APClient ap_client = new APClient();
		public static Dictionary<string, string> deathLinkMessages = new()
		{
			//base damage types need to be capitalized
			{"Hunger", "thought food was optional."},
			{"Fire", "went up in flames."},
			{"Electricity", "lit up like a light bulb."},
			{"Suffocation", "forgot the need to breathe."},
			{"Gravity", "thought we could fly."},
			{"Explosion", "panicked after igniting a bomb."},
			{"Poison", "didn't know they applied as a food taster."},
			{"Acid", "received a trap with not enough health left."},
			//these can be lowercase considering creature codes are all lowercase
			{"wolf", "was wolf bait."},
			{"pig-eurasian", "messed around with a boar."},
			{"pig-redriver", "messed around with a hog."},
			{"pig-warthog", "messed around with a warthog."},
            {"bear-black", "misremembered what to do and played dead."},
            {"bear-brown", "suffered a grizzly death."},
            {"bear-polar", "found out that polar bears are dangerous."},
			{"bear-sun", "didn't praise the sun(bear) enough."},
            {"bear-panda", "became the endangered species instead."},
            {"drifter", "tried to hug a drifter."},
			{"shiver", "shivered their last"},
			{"bowtorn", "got impaled by a bone arrow."},
            {"eidolon", "got crushed by an immobilized eidolon."},
			{"erel", "mistook themselves as the stone that killed the bird."},
            {"sheep", "got horned to death."},
            {"locust-bronze", "has locusts all over their dead body."},
			{"locust-corrupt", "has corrupt locusts all over their dead body."},
            {"locust-corrupt-sawblade", "got sawed into pieces by a sawblade locust."},
            {"goat-muskox", "was killed by the GOAT goat."},
            {"goat-takin", "'s life was taken by a takin."},
            {"goat", "was killed by a baaaad goat."},
            {"deer-moose", "got killed by a moose on the loose." },
            {"deer-elk", "was killed by an elk." },
            {"tameddeer-elk", "was backstabbed by their elk."},
            {"deer", "was killed by a deer, oh dear."},
        };

        #region Achievement dicts
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

		public Dictionary<string, string> BreakCropAchievements = new Dictionary<string, string>
		{
			{"crop-carrot-7", "Carrot Farmer"},
            {"crop-flax-9", "Flax Farmer"},
            {"crop-onion-7", "Onion Farmer"},
            {"crop-spelt-9", "Spelt Farmer"},
            {"crop-turnip-5", "Turnip Farmer"},
            {"crop-parsnip-8", "Parsnip Farmer"},
            {"crop-rice-10", "Rice Farmer"},
            {"crop-rye-9", "Rye Farmer"},
            {"crop-soybean-11", "Soybean Farmer"},
            {"crop-cabbage-12", "Cabbage Farmer"},
            {"crop-pineapple-16", "Pineapple Farmer Hot Crop"},
            {"crop-cassava-9", "Cassava Farmer Hot Crop"},
            {"crop-amaranth-9", "Amaranth Farmer Hot Crop"},
            {"crop-peanut-9", "Peanut Farmer Hot Crop"},
            {"crop-sunflower-12", "Sunflower Farmer"},
            {"crop-fenne;-9", "Fennel Farmer"},
            {"crop-licorice-9", "Licorice Farmer"},
        };

		public Dictionary<string, string> BreakBlockAchievements = new Dictionary<string, string>
		{
			{"locustnest-stalagmite-main", "Cave Cleaning"},
            {"locustnest-stalagmite-small1", "Cave Cleaning"},
            {"locustnest-stalagmite-small2", "Cave Cleaning"},
            {"locustnest-cage", "Cave Cleaning"},
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
			{"seeds-carrot" , "Carrot Propagation"},
            {"seeds-flax" , "Flax Propagation"},
            {"seeds-onion" , "Onion Propagation"},
            {"seeds-spelt" , "Spelt Propagation"},
            {"seeds-turnip" , "Turnip Propagation"},
            {"seeds-parsnip" , "Parsnip Propagation"},
            {"seeds-rice" , "Rice Propagation"},
            {"seeds-rye" , "Rye Propagation"},
            {"seeds-soybean" , "Soybean Propagation"},
            {"seeds-cabbage" , "Cabbage Propagation"},
            {"seeds-pumpkin" , "Pumpkin Propagation"},
            {"seeds-pineapple" , "Pineapple Propagation Hot Crop"},
            {"seeds-cassava" , "Cassava Propagation Hot Crop"},
            {"seeds-amaranth" , "Amaranth Propagation Hot Crop"},
            {"seeds-peanut" , "Peanut Propagation Hot Crop"},
            {"seeds-sunflower" , "Sunflower Propagation"},
            {"seeds-fennel" , "Fennel Propagation"},
            {"seeds-licorice" , "Licorice Propagation"},
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
			{"ore-bountiful-nativecopper", "Copper Bounty"},
            {"ore-bountiful-malachite", "Copper Bounty"},
            {"ore-bountiful-cassiterite", "Tin Bounty"},
            {"ore-bountiful-sphalerite", "Zinc Bounty"},
            {"ore-bountiful-bismuthinite", "Bismuth Bounty"},
            {"ore-bountiful-quartz_nativegold", "Gold Bounty"},
            {"ore-bountiful-quartz_nativesilver", "Silver Bounty"},
            {"ore-bountiful-galena", "Lead Bounty"},
            {"ore-bountiful-limonite", "Iron Bounty"},
            {"ore-bountiful-magnetite", "Iron Bounty"},
            {"ore-bountiful-hematite", "Iron Bounty"},
            {"ore-bountiful-chromite", "Chromite Bounty"},
            {"ore-bountiful-ilmenite", "Titanium Bounty"},
            {"ore-bountiful-pentlandite", "Nickel Bounty"},
            {"schematic-glider", "Glider Schematics"},
			{"jonaslens-north", "Lens Got"},
			{"schematic-customtranslocator", "Lens Delivered"},
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
			{"pig-eurasian-adult-male", "Pig Rancher"},
            {"pig-eurasian-adult-female", "Pig Rancher"},
            {"pig-redriver-adult-male", "Pig Rancher"},
            {"pig-redriver-adult-female", "Pig Rancher"},
            {"pig-warthog-adult-male", "Pig Rancher"},
            {"pig-warthog-adult-female", "Pig Rancher"},
            {"pig-eurasian-elder-male", "Pig Rancher"},
            {"pig-eurasian-elder-female", "Pig Rancher"},
            {"chicken-rooster", "Chicken Rancher"},
            {"chicken-hen", "Chicken Rancher"},
            {"goat-angora-adult-male", "Goat Rancher"},
            {"goat-angora-adult-female", "Goat Rancher"},
            {"goat-markhor-adult-male", "Goat Rancher"},
            {"goat-markhor-adult-female", "Goat Rancher"},
            {"goat-mountain-adult-male", "Goat Rancher"},
            {"goat-mountain-adult-female", "Goat Rancher"},
            {"goat-nubian-adult-male", "Goat Rancher"},
            {"goat-nubian-adult-female", "Goat Rancher"},
            {"goat-sirohi-adult-male", "Goat Rancher"},
            {"goat-sirohi-adult-female", "Goat Rancher"},
            {"goat-turdag-adult-male", "Goat Rancher"},
            {"goat-turdag-adult-female", "Goat Rancher"},
            {"goat-valais-adult-male", "Goat Rancher"},
            {"goat-valais-adult-female", "Goat Rancher"},
            {"sheep-bighorn-adult-male", "Sheep Rancher"},
            {"sheep-bighorn-adult-female", "Sheep Rancher"},
            {"sheep-mouflon-adult-male", "Sheep Rancher"},
            {"sheep-mouflon-adult-female", "Sheep Rancher"},
            {"hare-arctic-adult-male", "Hare Rancher"},
            {"hare-arctic-adult-female", "Hare Rancher"},
            {"hare-cape-adult-male", "Hare Rancher"},
            {"hare-cape-adult-female", "Hare Rancher"},
            {"hare-european-adult-male", "Hare Rancher"},
            {"hare-european-adult-female", "Hare Rancher"},
            {"hare-indian-adult-male", "Hare Rancher"},
            {"hare-indian-adult-female", "Hare Rancher"},
            {"hare-scrub-adult-male", "Hare Rancher"},
            {"hare-scrub-adult-female", "Hare Rancher"},
			{"eidolon-immobilized", "Mechanical Menace"},
			//	any corrupted : Against the Storm is handled manually in the kill check
		};
        #endregion

        // Called on server and client
        public override void Start(ICoreAPI api)
        {
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
			sapi = api;

			sapi.Network.RegisterChannel("sendDiscovery")
				.RegisterMessageType<SendArchipelagoDiscovery>()
				.SetMessageHandler<SendArchipelagoDiscovery>(SendInGameDiscovery);

			sapi.Network.RegisterChannel("triggerCheevo")
				.RegisterMessageType<TriggerCheevo>()
				.SetMessageHandler<TriggerCheevo>(TriggerCheevo);

			sapi.Network.RegisterChannel("locationVerified")
				.RegisterMessageType<LocationExists>();

			sapi.Network.RegisterChannel("DeathlinkReceived")
				.RegisterMessageType<ClientDeathLinkReceived>()
				.SetMessageHandler<ClientDeathLinkReceived>(KillPlayer);

			sapi.Event.PlayerJoin += OnPlayerJoin;
            sapi.Event.DidUseBlock += OnDidUseBlock;
			sapi.Event.OnEntityDeath += OnEntityDeath;
			sapi.Event.OnPlayerInteractEntity += OnPlayerInteractEntity;
			sapi.Event.PlayerDisconnect += OnPlayerDisconnect;
			sapi.Event.BreakBlock += OnBreakBlock;
			sapi.Event.RegisterGameTickListener(TimeAchievementCheck, 1000*60);
			sapi.Event.RegisterGameTickListener(APInventoryCheck, 1000*3);
            sapi.Event.PlayerDeath += OnPlayerDeath;
			

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

			capi.Network.RegisterChannel("sendDiscovery")
					.RegisterMessageType<SendArchipelagoDiscovery>();

			capi.Network.RegisterChannel("triggerCheevo")
				.RegisterMessageType<TriggerCheevo>();

			capi.Network.RegisterChannel("locationVerified")
				.RegisterMessageType<LocationExists>()
				.SetMessageHandler<LocationExists>(ClientSendsLocation);

			capi.Network.RegisterChannel("DeathlinkReceived")
				.RegisterMessageType<ClientDeathLinkReceived>();

			capi.Event.RegisterGameTickListener(ap_client.Flush, 1000*3);	    
			capi.Event.RegisterGameTickListener(infranet.RequestAPItemReceive, 1000*3);
            capi.Event.RegisterEventBusListener(OnReceivedDeath, 1, "DeathLink");
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

		public void OnPlayerDeath(IServerPlayer player, DamageSource source)
		{
			if(source == null)
			{
				return;
			}
			Entity? entitySource = source.GetCauseEntity();
			string entityCode = "PreventEntityMsg";
			string deathMsg = "";
			if (entitySource != null) 
			{ 
				entityCode = entitySource.Code.ToString().Split(":")[1];
			}
			string damageType = source.Type.ToString();
			foreach(var msg in deathLinkMessages)
			{
				if (entityCode == "locust-corrupt-sawblade")
				{
					if (msg.Key != "locust-corrupt-sawblade")
					{
						continue;
					}
                    deathMsg = msg.Value;
                    break;
                }
				if (entityCode.Contains(msg.Key))
				{
					deathMsg = msg.Value;
					break;
                }
				else if (msg.Key.Equals(damageType))
				{
                    deathMsg = msg.Value;
					break;
                }
				else
				{
					deathMsg = "died from an unknown source";
				}

			}
			string finalDeathMessage = ($" {deathMsg}");
            APClient.SendDeathLink(player.PlayerName.ToString(), finalDeathMessage);
		}

		public void OnReceivedDeath(string eventName, ref EnumHandling handling, IAttribute data)
		{
            string JsonDeathLink = data.ToJsonToken();
			string[] splitString = JsonDeathLink.Split('"');
			string deathLink = $"{splitString[1]} {splitString[3]}";
			foreach (IPlayer player in capi.World.AllOnlinePlayers)
			{
				if (player.Entity.Alive == true)
				{
                    capi.Network.GetChannel("DeathlinkReceived")
						.SendPacket(new ClientDeathLinkReceived { deathlink = deathLink});
				}
			}
			handling = EnumHandling.PreventSubsequent;
		}

        private void KillPlayer(IServerPlayer fromPlayer, ClientDeathLinkReceived packet)
        {
			fromPlayer.Entity.Die(EnumDespawnReason.Death);
            sapi.SendMessageToGroup(GlobalConstants.GeneralChatGroup, packet.ToString(), EnumChatType.Notification);
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
									TriggerAchievement(stack.Attributes.GetString("location"), player.PlayerName);
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
									if (code.Contains("bountiful"))
									{
										var splitcode = code.Split("-");
										code = splitcode[0] + "-" + splitcode[1] + "-" + splitcode[2];
                                    }
									if(ItemPickupAchievements.Keys.Contains(code))
										if (code.Contains("ore"))
										{
                                            TriggerAchievement(ItemPickupAchievements[code], player.PlayerName);
                                        }
										else
										{
                                            TriggerAchievement("Achievement: " + ItemPickupAchievements[code], player.PlayerName);
                                        }
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
						TriggerAchievement(cheevo, p.PlayerName);
								}
				if(one_year)
					TriggerAchievement("Achievement: One Year", p.PlayerName);
			}
		}


		public void OnEntityDeath(Entity entity, DamageSource source)
		{   //Check for kill achievements like Defeat a Bear
			int generation;
			if(entity.Code == "game:blockfalling")
			{
				return;
			}
			if (source.GetCauseEntity() is EntityPlayer ePlayer and not null)
			{
				string code = entity.Code.ToString().Split(':')[1];
				IServerPlayer player = (IServerPlayer)sapi.World.PlayerByUid(ePlayer.PlayerUID);
				if (KillAchievements.Keys.Contains(code))
				{
					TriggerAchievement("Achievement: " + KillAchievements[code], player.PlayerName);
				}
				if (entity.WatchedAttributes.Keys.Contains("generation"))
				{
					if (Int32.TryParse(entity.WatchedAttributes["generation"].GetValue().ToString(), out int gen))
					{
						if (gen >= 1 && KillAchievements.Keys.Contains(code))
						{
							TriggerAchievement("Achievement: " + KillAchievements[code], player.PlayerName);
						}
					}
				}
				if (code.Contains("corrupt"))
				{
					TriggerAchievement("Achievement: Against the Storm", player.PlayerName);
				}
			}
		}

		public void OnDidUseBlock(IServerPlayer byPlayer, BlockSelection blockSel)
		{	//Check for block use like anvils that lead to Age achievements
			string block = byPlayer.CurrentBlockSelection.Block.Code.ToString().Split(':')[1];
			if(UseBlockAchievements.Keys.Contains(block))
			{
				TriggerAchievement("Achievement: "+UseBlockAchievements[block], byPlayer.PlayerName);
			}
		}

        private void OnBreakBlock(IServerPlayer byPlayer, BlockSelection blockSel, ref float dropQuantityMultiplier, ref EnumHandling handling)
        {
			string block = blockSel.Block.Code.ToString().Split(":")[1];
			List<string> southCrops = ["Pineapple", "Casava", "Amaranth", "Peanut"];
			var downPos = blockSel.Position.DownCopy();
			Block blockBelow = sapi.World.BlockAccessor.GetBlock(downPos);
			if (blockBelow.ToString().Contains("farmland") && BreakCropAchievements.Keys.Contains(block))
			{
				if(southCrops.Any(crop => crop.Contains(block)))
				{
                    TriggerAchievement("Achievement: " + BreakCropAchievements[block], byPlayer.PlayerName);
                }
				else
				{
                    TriggerAchievement("Achievement: " + BreakCropAchievements[block], byPlayer.PlayerName);
                }
			}
			else if (BreakBlockAchievements.Keys.Contains(block))
			{
                TriggerAchievement("Achievement: " + BreakBlockAchievements[block], byPlayer.PlayerName);
            }
			
        }

        public void TriggerAchievement(string loc, string byPlayer, INetworkMessage message = null)
		{	//Dispatch a packet from the server to the client, notifying that an achievement has been reached
			Dictionary<string, Dictionary<string, string>> loc_list = sapi.LoadModConfig<Dictionary<string, Dictionary<string, string>>>(byPlayer+"_spoilers.json");
			if(loc_list.ContainsKey(loc))
			{
                foreach(IServerPlayer p in sapi.World.AllOnlinePlayers)
				{
					if (p.PlayerName == byPlayer){
                        infranet.ServerSendLocationCheck(loc, byPlayer);
                        sapi.SendIngameDiscovery(p, "location", loc);
                    }
				}
                loc_list.Remove(loc);
                sapi.StoreModConfig(loc_list, byPlayer + "_spoilers.json");
            }
		}

		public void SendInGameDiscovery(IServerPlayer byPlayer, SendArchipelagoDiscovery pkt)
		{
			sapi.SendIngameDiscovery(byPlayer, "location", pkt.loc);
		}

		public void ClientSendsLocation(LocationExists pkt)
		{
			capi.Network.GetChannel("sendDiscovery")
					.SendPacket(new SendArchipelagoDiscovery { loc = pkt.loc, });
		}

		public void TriggerCheevo(IServerPlayer player, TriggerCheevo pkt)
		{
			TriggerAchievement(pkt.loc, player.PlayerName);
		}

		public void OnPlayerDiscovery(object sender, string discoverycode, string text)
		{   //When a player gets a lore discovery doesn't matter which send a check
            JsonObject APConfig = capi.LoadModConfig("multiworld_config.json");
            Dictionary<string, Dictionary<string, string>> loc_list = capi.LoadModConfig<Dictionary<string, Dictionary<string, string>>>(APConfig["VSName"] + "_spoilers.json");
			string loc_list_first = loc_list.First().Key;
            if(discoverycode.Split("-")[0] == "lore" && loc_list_first.Contains("lore-"))
			{
				capi.Network.GetChannel("triggerCheevo")
					.SendPacket(new TriggerCheevo {loc = loc_list_first });
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
					string traderCode = trader.Code.ToString().Split(':')[1];
					if (traderCode.Contains("agriculture"))
					{ match = "Agriculture"; }
                    if (traderCode.Contains("artisan")) 
					{ match = "Artisan"; }
                    if (traderCode.Contains("buildmaterials")) 
					{ match = "Building"; }
                    if (traderCode.Contains("clothing")) 
					{ match = "Clothing"; }
                    if (traderCode.Contains("commodities")) 
					{ match = "Commodities"; }
                    if (traderCode.Contains("furniture")) 
					{ match = "Furniture"; }
                    if (traderCode.Contains("luxuries")) 
					{ match = "Luxuries"; }
                    if (traderCode.Contains("survivalgoods")) 
					{ match = "Survival"; }
                    if (traderCode.Contains("treasurehunter")) 
					{ match = "Treasure"; }
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
