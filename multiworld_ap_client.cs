using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Packets;
using Archipelago.MultiClient.Net.Models;

using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;

//this file manages the VS client's connection to AP, it will have utils for handling the session, item receipt, etc, any direct communication to AP
namespace Multiworld
{
    	public class ClientConfig
    	{
		public string APName = "Vintage";
		public string APIP = "localhost";
		public int APPort = 38281;
		public string VSName = "playwordsofpower";
		public string win_condition = "Iron";
		public int APReceivedIndex = 0;
		public Dictionary<string, Dictionary<string, string>> APData;
    	}
    
	public class APItem
	{
		public long LocationId = -50;
		public string ItemDisplayName;
		public string APOwnerName;
		public string Classification = "Filler";
	}

	public class APClient
	{
		public ClientConfig config;
		public ArchipelagoSession session;
		public ICoreClientAPI capi;
		public List<string> ap_chat_queue = new List<string>();
		public List<string> ap_items_to_receive = new List<string>();

		public void init(ICoreClientAPI api)
		{
			capi = api;
			Login();
		}

		public void OnMessageReceived(LogMessage message)
		{
			ap_chat_queue.Add("[Archipelago] "+message.ToString());
		}

		public void Flush(float dt)
		{
			for(int i = 0; i < ap_chat_queue.Count;i++)
			{
				capi.ShowChatMessage(ap_chat_queue[i]);
			}
			ap_chat_queue.Clear();
		}
		
		public void LoadClientConfig()
		{
			config = capi.LoadModConfig<ClientConfig>("multiworld_config.json");
            		if(config == null)
            		{
                		config = new ClientConfig();
				config.VSName = capi.World.Player.PlayerName;
                		capi.StoreModConfig(config, "multiworld_config.json");
           		 }
		}

		public void ConnectSignals()
		{
			session.Items.ItemReceived += OnItemReceived;
        	        session.MessageLog.OnMessageReceived += OnMessageReceived;
        	        session.Socket.PacketReceived += OnPacketReceived;
		}	

		public void Login()
		{ 
			LoadClientConfig();
			session = ArchipelagoSessionFactory.CreateSession(config.APIP, config.APPort);
			ConnectSignals(); //must sub to events before logging in
            		LoginResult result = session.TryConnectAndLogin("Vintage Story", config.APName, ItemsHandlingFlags.AllItems, requestSlotData: true);
			if(result.Successful)
				session.Socket.SendPacket(new SyncPacket());
		}

		public void Disconnect()
		{
			//session.Socket.Disconnect(); is listed in the API but doesn't exist?
		}

		public void LocationCheck(string loc, string vsPlayer)
		{ //if you are this player, convert string loc to id number and mark as checked
			if(vsPlayer == config.VSName)
			{
				session.Locations.CompleteLocationChecks(session.Locations.GetLocationIdFromName("Vintage Story", loc));
			}
		}

		public Dictionary<string, Dictionary<string, string>> GetScoutData()
		{ //build a dictionary of location:item that is specific to this client, include extra item data for placement logic
			Dictionary<long, ScoutedItemInfo> raw_dict = session.Locations.ScoutLocationsAsync(session.Locations.AllMissingLocations.ToArray()).Result;
			Dictionary<string, Dictionary<string, string>> final_dict = new Dictionary<string, Dictionary<string, string>>();
			
			foreach(var kvp in raw_dict)
			{
				Dictionary<string, string> vitem = new Dictionary<string, string>();
				
				vitem["LocationId"] = kvp.Key.ToString();
				vitem["ItemDisplayName"] = kvp.Value.ItemDisplayName;
				vitem["APOwnerName"] = kvp.Value.Player.Name;
				switch((int)kvp.Value.Flags)
				{
					case 1:
						vitem["Classification"] = "Progression";
						break;
					case 2:
						vitem["Classification"] = "Useful";
						break;
					case 3:
						vitem["Classification"] = "Trap";
						break;
					default:
						vitem["Classification"] = "Filler";
						break;
				}
				final_dict[session.Locations.GetLocationNameFromId(kvp.Key)] = vitem;
				//Console.WriteLine($"[Scouted] {session.Locations.GetLocationNameFromId(kvp.Key)}: {vitem["ItemDisplayName"]}");
				config.APData = final_dict;
				capi.StoreModConfig(config, "multiworld_config.json");
			}
			return final_dict;
		}

		public void OnItemReceived( ReceivedItemsHelper receivedItemsHelper )
	        {
			ProcessAPItemQueue();	
	        }

		public void ProcessAPItemQueue()
		{
			IReceivedItemsHelper receivedItemsHelper = session.Items;
			ItemInfo test = session.Items.PeekItem();
			//Console.WriteLine($"Items {config.APReceivedIndex} / {receivedItemsHelper.Index}");
			while( config.APReceivedIndex < receivedItemsHelper.Index )
			{
		  		  ap_items_to_receive.Add(ResolveItemCode(receivedItemsHelper.PeekItem().ItemDisplayName));
		  		  config.APReceivedIndex++;
			}
	          		  capi.StoreModConfig(config, "multiworld_config.json");
	          		  receivedItemsHelper.DequeueItem();

		}

		public string ResolveItemCode(string displayName)
		{
			string code = displayName;
			if(APItemToLocal.items.Keys.Contains(displayName))
			{
				code = "item:game:"+APItemToLocal.items[displayName];
			}
			if(APItemToLocal.blocks.Keys.Contains(displayName))
			{
				code = "block:game:"+APItemToLocal.blocks[displayName];
			}
			return code;
		}	
		
    public class APItemToLocal
    {
        public static Dictionary<string, string> items = new Dictionary<string, string>
        {
            {"Hunter Backpack", "hunterbackpack"},
            {"Copper Pickaxe", "pickaxe-copper"},
            {"Copper Saw", "saw-copper"},
            {"Copper Hammer", "hammer-copper"},
            {"Linen Sack", "linensack"},
            {"Black Bronze Pickaxe", "pickaxe-blackbronze"},
            {"Iron Pickaxe", "pickaxe-iron"},
            {"Steel Pickaxe", "pickaxe-steel"},
            {"Steel Falx", "blade-falx-steel"},
            {"Prospecting Pick", "prospectingpick-copper"},
	    {"Copper Chisel", "chisel-copper"},
	    {"Flax Seeds", "seeds-flax"},
	    {"Charcoal", "charcoal"},
	    {"Temporal Gear", "gear-temporal"},
	    {"Rusty Gear", "gear-rusty"},
            //{"Fireclay x 64", "clay-fire"},//these 3 need quantity
	    //{"Clay x 24", "clay-blue"},
	    //{"Lime x24", "lime"},
	    
        };

	public static Dictionary<string, string> blocks = new Dictionary<string, string>
	{
	    //{"Pie", "pie-perfect"}, //needs attributes
	    {"Storage Vessel", "storagevessel-blue-fired"},
	    {"Ore Bomb", "bomb-ore"},
	    {"Cookpot", "claypot-blue-fired"},
	    {"Crock", "crock-blue-fired"},
            {"Bowl", "bowl-blue-fired"},
            {"Barrel", "barrel"},
            {"Quern", "quern-granite"},
            {"Crucible", "crucible-blue-fired"},
            {"Chest", "chest-east"},
            {"Bucket", "woodbucket"},

	};
    }

	public void OnPacketReceived(ArchipelagoPacketBase packet)
	        { //Function for debugging
			switch(packet)
			{
				case DataPackagePacket dataPacket:
					break;
				case RoomInfoPacket roomPacket:
					break;
				case ConnectedPacket connPacket:
					bool steel = (connPacket.SlotData["steel"].ToString() == "1");
					if(steel == true)
						config.win_condition = "Steel";
					capi.ShowChatMessage("Connected to Archipelago Server");
					break;
				case ReceivedItemsPacket itemPacket:
				//	foreach(NetworkItem item in itemPacket.Items)
				//	{
				//		Console.WriteLine($"[ItemPacket] ItemID={item.Item}");
				//	}
					break;
				default:
					break;
			}
			//Console.WriteLine($"[PacketDump] {packet}");
	        }
	}
}

