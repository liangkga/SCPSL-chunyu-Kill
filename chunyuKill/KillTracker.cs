using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using ServerHandlers = Exiled.Events.Handlers.Server;
using PlayerHandlers = Exiled.Events.Handlers.Player;

namespace ServerKillPlugin
{
	/// <summary>
	/// 击杀追踪器主类，负责处理玩家击杀统计和数据管理喵~
	/// </summary>
	public class MainPlugin
	{
		/// <summary>
		/// 启用击杀追踪器，注册所有必要的事件处理器喵~
		/// </summary>
		public void OnEnabled()
		{
			// 注册服务器等待玩家事件，用于加载数据喵~
			ServerHandlers.WaitingForPlayers += new CustomEventHandler(MainPlugin.JSONReader);
			// 注册玩家验证事件，用于初始化玩家数据喵~
			PlayerHandlers.Verified += new CustomEventHandler<VerifiedEventArgs>(this.OnVerified);
			// 注册玩家死亡事件，用于统计击杀数据喵~
			PlayerHandlers.Dying += new CustomEventHandler<DyingEventArgs>(this.OnDying);
			// 注册玩家离开事件，用于保存数据喵~
			PlayerHandlers.Left += new CustomEventHandler<LeftEventArgs>(this.OnLeft);
		}

		/// <summary>
		/// 禁用击杀追踪器，注销所有事件处理器喵~
		/// </summary>
		public void OnDisabled()
		{
			// 注销所有已注册的事件处理器喵~
			ServerHandlers.WaitingForPlayers -= new CustomEventHandler(MainPlugin.JSONReader);
			PlayerHandlers.Verified -= new CustomEventHandler<VerifiedEventArgs>(this.OnVerified);
			PlayerHandlers.Dying -= new CustomEventHandler<DyingEventArgs>(this.OnDying);
			PlayerHandlers.Left -= new CustomEventHandler<LeftEventArgs>(this.OnLeft);
		}

		/// <summary>
		/// 玩家验证完成事件处理器，初始化或加载玩家击杀数据喵~
		/// </summary>
		/// <param name="ev">玩家验证事件参数</param>
		public void OnVerified(VerifiedEventArgs ev)
		{
			// 查找是否已存在该玩家的击杀记录喵~
			KillList killList = MainPlugin.Infos.FirstOrDefault((KillList x) => x.UserId == ev.Player.UserId);
			if (killList != null)
			{
				// 已存在的玩家，从文件加载击杀数据到内存字典喵~
				MainPlugin.KillSCP[ev.Player.UserId] = killList.KillSCPs;
				MainPlugin.KillHuman[ev.Player.UserId] = killList.KillHuman;
				// 更新玩家昵称显示击杀统计喵~
				this.CheckNickname(ev.Player);
			}
			else
			{
				// 新玩家，创建新的击杀记录喵~
				KillList newKillList = new KillList
				{
					UserId = ev.Player.UserId,
					MusicPath = "",
					Broadcast = "",
					OnlyMe = false,
					KillSCPs = 0,
					KillHuman = 0,
					CustomTag = ""
				};
				// 添加到击杀记录列表喵~
				MainPlugin.Infos.Add(newKillList);
				// 初始化内存字典中的击杀数据喵~
				MainPlugin.KillSCP[ev.Player.UserId] = 0;
				MainPlugin.KillHuman[ev.Player.UserId] = 0;
				// 设置初始昵称显示喵~
				this.CheckNickname(ev.Player);
			}
		}

		/// <summary>
		/// 玩家离开服务器事件处理器，保存玩家击杀数据喵~
		/// </summary>
		/// <param name="ev">玩家离开事件参数</param>
		public void OnLeft(LeftEventArgs ev)
		{
			// 查找玩家的击杀记录喵~
			KillList killList = MainPlugin.Infos.FirstOrDefault((KillList x) => x.UserId == ev.Player.UserId);
			if (killList != null)
			{
				// 将内存中的击杀数据同步到记录对象喵~
				killList.KillSCPs = MainPlugin.KillSCP[ev.Player.UserId];
				killList.KillHuman = MainPlugin.KillHuman[ev.Player.UserId];
				// 保存所有击杀数据到文件喵~
				MainPlugin.JsonWriter();
			}
		}

		/// <summary>
		/// 玩家死亡事件处理器，统计击杀数据并显示击杀信息喵~
		/// </summary>
		/// <param name="ev">玩家死亡事件参数</param>
		public void OnDying(DyingEventArgs ev)
		{
			// 如果没有攻击者（自杀、环境伤害等），不统计击杀喵~
			if (ev.Attacker == null)
			{
				return;
			}
			
			// 查找攻击者的击杀记录喵~
			KillList killList = MainPlugin.Infos.FirstOrDefault((KillList x) => x.UserId == ev.Attacker.UserId);
			if (killList != null)
			{
				// 判断被击杀的是SCP还是人类喵~
				if (ev.Player.IsScp)
				{
					// 击杀SCP，增加SCP击杀数喵~
					this.AddKill(KillType.SCP, ev.Attacker);
					// 向所有其他玩家显示收容信息，使用Hint避免与mvp.dll冲突喵~
					foreach (Player player in Player.List.Where(x => x != ev.Player))
					{
						player.ShowHint(string.Concat(new string[]
						{
							"<size=20><color=red>",
							ev.Attacker.Nickname,
							" 收容了 ",
							ev.Player.Nickname,
							" !</color></size>"
						}), 3);
					}
					// 向被击杀的SCP显示收容信息和自定义消息喵~
					ev.Player.ShowHint(string.Concat(new string[]
					{
						"<size=18><color=orange>你已被 ",
						ev.Attacker.Nickname,
						" 收容!\n",
						ev.Attacker.Nickname,
						" 对你说:\n",
						killList.Broadcast.Replace("{Attacker}", ev.Attacker.Nickname ?? "").Replace("{Target}", ev.Player.Nickname ?? ""),
						"</color></size>"
					}), 4);
				}
				else
				{
					// 击杀人类，增加人类击杀数喵~
					this.AddKill(KillType.Human, ev.Attacker);
					// 向被击杀的人类显示击杀信息和自定义消息喵~
					ev.Player.ShowHint(string.Concat(new string[]
					{
						"<size=18><color=red>你已被 ",
						ev.Attacker.Nickname,
						" 击杀!\n",
						ev.Attacker.Nickname,
						" 对你说:\n",
						killList.Broadcast.Replace("{Attacker}", ev.Attacker.Nickname ?? "").Replace("{Target}", ev.Player.Nickname ?? ""),
						"</color></size>"
					}), 4);
				}
			}
		}

		/// <summary>
		/// 增加玩家击杀数统计，并更新昵称显示喵~
		/// </summary>
		/// <param name="type">击杀类型（SCP或人类）</param>
		/// <param name="player">进行击杀的玩家</param>
		public void AddKill(KillType type, Player player)
		{
			// 防御性编程：确保玩家击杀数据在字典中存在喵~
			if (!MainPlugin.KillSCP.ContainsKey(player.UserId))
			{
				MainPlugin.KillSCP[player.UserId] = 0;
			}
			if (!MainPlugin.KillHuman.ContainsKey(player.UserId))
			{
				MainPlugin.KillHuman[player.UserId] = 0;
			}
			
			// 根据击杀类型增加对应的击杀数喵~
			if (type != KillType.Human)
			{
				if (type == KillType.SCP)
				{
					// 增加SCP击杀数喵~
					MainPlugin.KillSCP[player.UserId]++;
				}
			}
			else
			{
				// 增加人类击杀数喵~
				MainPlugin.KillHuman[player.UserId]++;
			}
			// 更新玩家昵称显示最新击杀统计喵~
			this.CheckNickname(player);
			// 保存击杀数据到文件喵~
			MainPlugin.JsonWriter();
		}

		/// <summary>
		/// 更新玩家昵称，在昵称前显示击杀统计信息喵~
		/// </summary>
		/// <param name="player">要更新昵称的玩家</param>
		public void CheckNickname(Player player)
		{
			// 检查玩家是否有有效的64位Steam ID喵~
			if (string.IsNullOrEmpty(player.UserId) || player.UserId.Length < 17 || !player.UserId.All(char.IsDigit))
			{
				// 没有64位ID的玩家不修改名称，保持原样喵~
				return;
			}
			
			// 防御性编程：确保玩家击杀数据在字典中存在喵~
			if (!MainPlugin.KillSCP.ContainsKey(player.UserId))
			{
				MainPlugin.KillSCP[player.UserId] = 0;
			}
			if (!MainPlugin.KillHuman.ContainsKey(player.UserId))
			{
				MainPlugin.KillHuman[player.UserId] = 0;
			}
			
			// 格式化昵称：[SCP:击杀数 人类:击杀数] 原昵称喵~
			player.DisplayNickname = string.Format("[SCP:{0} 玩家:{1}] {2}", MainPlugin.KillSCP[player.UserId], MainPlugin.KillHuman[player.UserId], player.Nickname);
		}

		/// <summary>
		/// 存储所有玩家人类击杀数的字典，键为玩家UserId，值为击杀数喵~
		/// </summary>
		public static Dictionary<string, int> KillHuman { get; set; } = new Dictionary<string, int>();

		/// <summary>
		/// 存储所有玩家SCP击杀数的字典，键为玩家UserId，值为击杀数喵~
		/// </summary>
		public static Dictionary<string, int> KillSCP { get; set; } = new Dictionary<string, int>();

		/// <summary>
		/// 存储所有玩家击杀信息的列表，包含详细的击杀记录和配置喵~
		/// </summary>
		public static List<KillList> Infos { get; set; } = new List<KillList>();

		/// <summary>
		/// 从文件读取击杀数据，在服务器等待玩家时调用喵~
		/// </summary>
		public static void JSONReader()
		{
			// 构建击杀数据文件路径喵~
			string filePath = Path.Combine(Paths.Configs, "KillData.txt");
			try
			{
				// 检查击杀数据文件是否存在喵~
				if (File.Exists(filePath))
				{
					// 读取文件所有行喵~
					string[] lines = File.ReadAllLines(filePath);
					// 初始化击杀信息列表喵~
					MainPlugin.Infos = new List<KillList>();
					// 逐行解析击杀数据喵~
					foreach (string line in lines)
					{
						// 跳过空行喵~
						if (!string.IsNullOrEmpty(line))
						{
							// 按管道符分割数据：UserId|SCP击杀数|人类击杀数喵~
							string[] parts = line.Split('|');
							if (parts.Length >= 3)
							{
								// 创建击杀记录对象喵~
								KillList killData = new KillList
								{
									UserId = parts[0],
									// 安全解析SCP击杀数，失败则默认为0喵~
									KillSCPs = int.TryParse(parts[1], out int scpKills) ? scpKills : 0,
									// 安全解析人类击杀数，失败则默认为0喵~
									KillHuman = int.TryParse(parts[2], out int humanKills) ? humanKills : 0
								};
								// 添加到击杀信息列表喵~
								MainPlugin.Infos.Add(killData);
							}
						}
					}
					return;
				}
			}
			catch (Exception ex)
			{
				// 读取失败时记录错误日志喵~
				Log.Error($"读取击杀数据失败: {ex.Message}");
			}
			// 文件不存在或读取失败时初始化空列表喵~
			MainPlugin.Infos = new List<KillList>();
		}

		/// <summary>
		/// 将击杀数据保存到文件，使用管道符分隔的纯文本格式喵~
		/// </summary>
		public static void JsonWriter()
		{
			// 构建击杀数据文件路径喵~
			string filePath = Path.Combine(Paths.Configs, "KillData.txt");
			try
			{
				// 创建文本行列表用于保存喵~
				List<string> lines = new List<string>();
				// 遍历所有击杀记录，格式化为文本行喵~
				foreach (KillList killData in MainPlugin.Infos)
				{
					// 格式：UserId|SCP击杀数|人类击杀数喵~
					lines.Add($"{killData.UserId}|{killData.KillSCPs}|{killData.KillHuman}");
				}
				// 将所有行写入文件喵~
				File.WriteAllLines(filePath, lines);
				// 记录保存成功日志喵~
				Log.Info($"击杀数据已保存到: {filePath} (共{lines.Count}条记录)");
			}
			catch (Exception ex)
			{
				// 保存失败时记录错误日志喵~
				Log.Error($"保存击杀数据失败: {ex.Message}");
			}
		}
	}
}
