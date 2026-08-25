using ProtoBuf;
using System;
using System.Collections.Generic;
using Vintagestory.API.Server;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;
using Vintagestory.Client.NoObf;
using Vintagestory.Server;
using Vintagestory.API.MathTools;
using Vintagestory.Common;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Vintagestory.API.Util;
using Vintagestory.API.Datastructures;

//this file houses utils for managing communication between VS server and client, such as protobuf packets and text commands
namespace Multiworld
{
    #region ProtoContracts
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class APNotification
	{
		public string message;
	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class ClientAPScoutPacket
	{
		public Dictionary<string, Dictionary<string, string>> ScoutData;
		public string Player;
	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class ClientAPItemReceivePacket
	{
		public List<string> Items = new List<string>();
		public string Player;
	}

	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class ClientDeathLinkReceived
	{
		public string deathlink;
	}


    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
	public class SendArchipelagoDiscovery
	{
		public string loc;
	}

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class LocationExists
    {
		public string loc;
        public string PlayerName;
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class TriggerCheevo
    {
        public string loc;
    }
    #endregion

    public class Infranet
	{
		IServerNetworkChannel serverChannel;
        IClientNetworkChannel clientChannel;

		ICoreServerAPI sapi;
		ICoreClientAPI capi;
		APClient ap_client;
		bool apgoal = false;
		
		public void init_server(ICoreServerAPI api)
		{
			sapi = api;
			serverChannel =
				api.Network.RegisterChannel("ap_notification")
				.RegisterMessageType(typeof(APNotification))
				.RegisterMessageType(typeof(ClientAPScoutPacket))
				.RegisterMessageType(typeof(ClientAPItemReceivePacket))
				.SetMessageHandler<ClientAPScoutPacket>(OnClientAPScoutPacket)
				.SetMessageHandler<ClientAPItemReceivePacket>(OnClientAPItemReceivePacket);
		}

		public void init_client(ICoreClientAPI api, APClient client)
		{
			capi = api;
			ap_client = client;
			clientChannel = 
			    api.Network.RegisterChannel("ap_notification")
			    .RegisterMessageType(typeof(APNotification))
			    .RegisterMessageType(typeof(ClientAPScoutPacket))
			    .RegisterMessageType(typeof(ClientAPItemReceivePacket))
			    .SetMessageHandler<APNotification>(OnServerMessage);
		}

		public TextCommandResult OnAPRegister(TextCommandCallingArgs args)
		{ //send your location:item list to the server
			clientChannel.SendPacket(new ClientAPScoutPacket()
			{
				Player = capi.World.Player.PlayerName,
				ScoutData = ap_client.GetScoutData()
			});
			return TextCommandResult.Success("Spoilers sent!");
		}

		public TextCommandResult login(TextCommandCallingArgs args)
		{
			ap_client.Login();
			OnAPRegister(args);
			return TextCommandResult.Success($"Attempting to connect to Archipelago {ap_client.config.APIP} on port {ap_client.config.APPort}");
		}

		public TextCommandResult apname(TextCommandCallingArgs args)
		{
			ap_client.config.APName = args[0].ToString();
			capi.StoreModConfig(ap_client.config, "multiworld_config.json");
			return TextCommandResult.Success($"APName set to {args[0]}");
		}

		public TextCommandResult set_ip(TextCommandCallingArgs args)
		{ //set your IP for Archipelago
			ap_client.config.APIP = args[0].ToString();
			capi.StoreModConfig(ap_client.config, "multiworld_config.json");
			return TextCommandResult.Success($"APIP set to {args[0]}");
		}

		public TextCommandResult set_port(TextCommandCallingArgs args)
		{ //set your IP for Archipelago
			ap_client.config.APPort = int.Parse(args[0].ToString());
			capi.StoreModConfig(ap_client.config, "multiworld_config.json");
			return TextCommandResult.Success($"APPort set to {args[0]}");
		}

		public TextCommandResult set_pass(TextCommandCallingArgs args)
		{ //sets your rooms password for Archipelago
			ap_client.config.Password = args[0].ToString();
			capi.StoreModConfig(ap_client.config, "multiworld_config.json");
			return TextCommandResult.Success($"APPassword set");
		}

		public TextCommandResult print_remaining_locations(TextCommandCallingArgs args)
		{ //print a list of unchecked locations
			Dictionary<string, Dictionary<string, string>> dict = ap_client.GetScoutData();
			string output = "";
			foreach(var kvp in dict)
			{
				output += kvp.Key + "\n";
			}
			return TextCommandResult.Success(output);
		}

		public void ServerSendLocationCheck(string loc, string player)
		{ //tells the clients to check a location
			serverChannel.BroadcastPacket(new APNotification(){ message = $"LocCheck;{loc};{player}" });
		}



		private void OnClientAPScoutPacket(IPlayer fromPlayer, ClientAPScoutPacket packet)
		{ //server stores a list of each registered client's locations:items
			if (packet.ScoutData != null)
			{
				sapi.StoreModConfig(packet.ScoutData, packet.Player + "_spoilers.json");
				sapi.SendMessageToGroup(GlobalConstants.GeneralChatGroup, packet.Player + "_spoilers.json saved", EnumChatType.Notification);
			}
			else
			{
                sapi.SendMessageToGroup(GlobalConstants.GeneralChatGroup, "Connection refused or lost", EnumChatType.Notification);
            }
		}

		public void RequestAPItemReceive(float dt)
		{
			if(ap_client.ap_items_to_receive.Count == 0)
				return;
			else
			{
				clientChannel.SendPacket(new ClientAPItemReceivePacket()
				{
					Player = capi.World.Player.PlayerName,
					Items = ap_client.ap_items_to_receive,
				});
				ap_client.ap_items_to_receive.Clear();
			}

		}

		private void OnClientAPItemReceivePacket(IPlayer fromPlayer, ClientAPItemReceivePacket packet)
		{ //give the player items listed without question
            string player = packet.Player;
			List<string> items = new List<string>(packet.Items);
			IPlayerInventoryManager inv = fromPlayer.InventoryManager;
			for(int i = 0; i < items.Count;i++)
			{
				string[] split = items[i].Split(':');
				Item newItem;
				Block newBlock;
				IBlockAccessor accessor;
				ItemStack stack = null;
				int count = 1;
				if(split[0] == "item" || split[0] == "quantity")
				{
					newItem = sapi.World.GetItem(new AssetLocation(split[2]));
					if(newItem == null)
					{
						Console.WriteLine($"{items[i]} ITEM CODE NOT FOUND SERVER SIDE");
						continue;
					}
					if(split[0] == "quantity")
						count = int.Parse(split[3]);
					stack = new ItemStack(newItem, count);
				}
				else if(split[0] == "block")
				{
					newBlock = sapi.World.GetBlock(new AssetLocation(split[2]));
					if(newBlock == null)
					{
						Console.WriteLine($"{items[i]} BLOCK CODE NOT FOUND SERVER SIDE");
						continue;
					}
					stack = new ItemStack(newBlock, 1);
				}
				else if (split[0] == "enemy")
				{
                    EntityProperties newEntityProp = sapi.World.GetEntityType(new AssetLocation(split[2]));
                    EntityPos xyzPos = null;
                    EntityPos enemyPos = null;
                    Random rand = new Random();
                    foreach (IServerPlayer serverplayer in sapi.World.AllOnlinePlayers)
                    {
                        xyzPos = serverplayer.Entity.Pos.Copy();
                        xyzPos.Pitch = 0f;
                        xyzPos.Roll = 0f;
                        xyzPos.Yaw = 0f;
                    }
                    for (int y = 0; y < 5; y++)
                    {
                        Entity entity = sapi.World.ClassRegistry.CreateEntity(newEntityProp);
                        enemyPos = xyzPos;
                        enemyPos.X += rand.Next(-8, 8);
                        enemyPos.Z += rand.Next(-8, 8);
                        enemyPos.Y = sapi.World.BlockAccessor.GetRainMapHeightAt(enemyPos.AsBlockPos) + 1;
                        if (entity != null)
                        {
                            entity.Pos.SetFrom(xyzPos);
                            entity.PositionBeforeFalling.Set(entity.Pos.X, entity.Pos.Y, entity.Pos.Z);
                            sapi.World.SpawnEntity(entity);
                        }

                    }
                }
				else if (split[0] == "stat")
				{
                    EntityStats playerstats = fromPlayer.Entity.Stats;
					DamageSource source = new()
					{
						Source = EnumDamageSource.Void,
						Type = EnumDamageType.Acid
					};
					ITreeAttribute hungerTree = fromPlayer.Entity.WatchedAttributes.GetTreeAttribute("hunger");
                    switch (split[2])
                    {
                        case "damage":
							fromPlayer.Entity.ReceiveDamage(source, 5f);
                            break;
                        case "satiety":
							float currentsatiety = hungerTree.GetFloat("currentsaturation");
							if (currentsatiety - 250f <= 0)
							{
								fromPlayer.Entity.ReceiveSaturation(-currentsatiety);
							}
							else
							{
                                fromPlayer.Entity.ReceiveSaturation(-250f);
                            }
                            break;
						default:
							string nutriType = split[2].Split("-")[1];
							float currentNutri = hungerTree.GetFloat($"{nutriType}Level");
							if(currentNutri - 250f <= 0)
							{
								hungerTree.SetFloat($"{nutriType}Level", 0f);
								fromPlayer.Entity.WatchedAttributes.Set("hunger", hungerTree);
							}
							else
							{
                                hungerTree.SetFloat($"{nutriType}Level", currentNutri - 250f);
                                fromPlayer.Entity.WatchedAttributes.Set("hunger", hungerTree);
                            }
							break;
                    }
					
				}
                else if (split[0] == "effect")
                {
					switch (split[2])
					{
						case "drunk":
							fromPlayer.Entity.WatchedAttributes.SetFloat("intoxication", 1.5f);
							break;
						case "psychedelic":
                            fromPlayer.Entity.WatchedAttributes.SetFloat("psychedelic", 2f);
                            break;
					}
                    sapi.SendMessageToGroup(GlobalConstants.GeneralChatGroup, split[2] + " should be applied", EnumChatType.Notification);
                }
				else if (split[0] == "trait")
				{
					CharacterSystem classSystem = sapi.ModLoader.GetModSystem<CharacterSystem>();
                    string characlass = fromPlayer.Entity.WatchedAttributes.GetString("characterClass");
					if (traits[split[2]] == null)
					{
						string[] extraTraits = fromPlayer.Entity.WatchedAttributes.GetStringArray("extraTraits", []);
						extraTraits.Append(split[2]);
						fromPlayer.Entity.WatchedAttributes.SetStringArray("gainedTraits", extraTraits);
                    }
					else
					{
						Dictionary<string, float> traiteffects = traits[split[2]];
						foreach (KeyValuePair<string, float> traitpart in traiteffects)
						{
							fromPlayer.Entity.Stats.Set(traitpart.Key, $"trait_{split[2]}", traitpart.Value);
						}
					}
					classSystem.setCharacterClass(fromPlayer.Entity, characlass, initializeGear: false);
                    sapi.SendMessageToGroup(GlobalConstants.GeneralChatGroup, split[2] + " should be applied", EnumChatType.Notification);
                }
				fromPlayer.Entity.WatchedAttributes.MarkAllDirty();
                if (stack == null)
					continue;

				if(!inv.TryGiveItemstack(stack, true))
				{
					sapi.World.SpawnItemEntity(stack, fromPlayer.Entity.Pos.XYZ);
				}
                sapi.SendMessageToGroup(GlobalConstants.GeneralChatGroup, items[i] + " should be delivered", EnumChatType.Notification);
            }
		}

		private void OnServerMessage(APNotification ap_notification)
		{
			if(ap_notification.message.Contains("LocCheck"))
			{
				string[] splitstring = ap_notification.message.Split(';');
				ap_client.LocationCheck(splitstring[1], splitstring[2]);
				string goal = ap_client.config.win_condition;
				if (splitstring[1] == goal)
				{
					ap_client.session.SetGoalAchieved();
					apgoal = true;
				}
			}
		}

		public static Dictionary<string, Dictionary<string, float>?> traits = new()
		{
			{"focused", new Dictionary<string,float>{ {"rangedWeaponsDamage", 0.2f }, { "rangedWeaponsAcc", 0.3f}, {"bowDrawingStrength", 0.2f} }},
			{"resourceful", new Dictionary<string,float>{ {"animalLootDropRate", 0.1f}, {"animalHarvestingTime", -0.25f} }},
			{"fleetfooted", new Dictionary<string,float>{ {"walkspeed", 0.1f} }},
			{"forager", new Dictionary<string,float>{ {"forageDropRate", 0.1f}, {"wildCropDropRate", 0.2f} }},
			{"pilferer", new Dictionary<string,float>{ {"vesselContentsDropRate", 0.15f}, {"rustyGearDropRate", 0.1f}, {"wholeVesselLootChance", 0.12f} }},
			{"furtive", new Dictionary<string,float>{ {"animalSeekingRange", -0.35f } }},
			{"precise", new Dictionary<string,float>{ {"mechanicalsDamage", 0.25f} }},
			{"technical", new Dictionary<string,float>{ {"temporalGearTLRepairCost", -1f} }},
			{"soldier", new Dictionary<string,float>{ {"meleeWeaponsDamage", 0.3f}, {"armorDurabilityLoss", -0.15f}, {"armorWalkSpeedAffectedness", -0.25f} }},
			{"hardy", new Dictionary<string,float>{ {"miningSpeedMul", 0.1f}, {"maxhealthExtraPoints", 5f} }},
			{"mender", new Dictionary<string,float>{ {"armorDurabilityLoss", -0.25f} }},
			{"improviser", null},
			{"tinkerer", null},
			{"merciless", null},
			{"clothier", null},
			{"bowyer", null}
		};
	}
}
