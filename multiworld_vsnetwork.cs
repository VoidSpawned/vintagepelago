using ProtoBuf;
using System;
using System.Collections.Generic;
using Vintagestory.API.Server;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

//this file houses utils for managing communication between VS server and client, such as protobuf packets and text commands
namespace Multiworld
{
	
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
	public class Infranet
	{
		IServerNetworkChannel serverChannel;
		IClientNetworkChannel clientChannel;

		ICoreServerAPI sapi;
		ICoreClientAPI capi;
		APClient ap_client;
		
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
			sapi.StoreModConfig(packet.ScoutData, packet.Player+"_spoilers.json");
			sapi.SendMessageToGroup(GlobalConstants.GeneralChatGroup, packet.Player+"_soilers.json saved", EnumChatType.Notification);
		}

		public void RequestAPItemReceive(float dt)
		{
			//Console.WriteLine($"requesting: {string.Join(Environment.NewLine, ap_client.ap_items_to_receive)}");
			if(ap_client.ap_items_to_receive.Count == 0)
				return;
			else
			{
				//Console.WriteLine("sending");
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
				
				if(stack == null)
					continue;

				if(!inv.TryGiveItemstack(stack, true))
				{
					sapi.World.SpawnItemEntity(stack, fromPlayer.Entity.Pos.XYZ);
				}
				sapi.SendMessageToGroup(GlobalConstants.GeneralChatGroup, items[i] + "should be delivered", EnumChatType.Notification);
			}
		}

		private void OnServerMessage(APNotification ap_notification)
		{
			if(ap_notification.message.Contains("LocCheck"))
			{
				string[] splitstring = ap_notification.message.Split(';');
				ap_client.LocationCheck(splitstring[1], splitstring[2]);
				string goal = "Achievement: " + ap_client.config.win_condition + " Age";
				if(splitstring[1] == goal)
					ap_client.session.SetGoalAchieved();	
			}
		}
	}
}
