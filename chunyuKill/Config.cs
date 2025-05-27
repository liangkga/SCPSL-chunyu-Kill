using System;
using Exiled.API.Interfaces;
using System.Collections.Generic;

namespace ServerKillPlugin
{
	/// <summary>
	/// 插件配置类，包含所有可配置的选项喵~
	/// </summary>
	public class Config : IConfig
	{
		/// <summary>
		/// 是否启用插件，默认为true喵~
		/// </summary>
		public bool IsEnabled { get; set; } = true;

		/// <summary>
		/// 是否启用调试模式，用于输出详细日志喵~
		/// </summary>
		public bool Debug { get; set; } = true;

		/// <summary>
		/// 是否启用MVP功能，显示击杀排行榜喵~
		/// </summary>
		public bool IsEnableMVP { get; set; } = true;

		/// <summary>
		/// 对局结束时是否启用友伤，允许队友互相伤害喵~
		/// </summary>
		public bool IsEnableRoundEndedFF { get; set; } = true;

		/// <summary>
		/// 击杀列表配置信息，包含玩家的击杀记录和显示设置喵~
		/// </summary>
		public List<KillList> Infos { get; set; } = new List<KillList>()
		{
			// 默认配置示例喵~
			new KillList
			{
				// 玩家Steam64位ID，用于唯一标识玩家喵~
				UserId = "chunyu.wiki@steam",
				// 击杀广播消息模板，{Attacker}会被替换为击杀者名称喵~
				Broadcast = "你被 {Attacker} 击杀了！",
				// 是否仅击杀者自己可见广播消息喵~
				OnlyMe = false,
				// 该玩家的SCP击杀数统计喵~
				KillSCPs = 0,
				// 该玩家的人类击杀数统计喵~
				KillHuman = 0
			}
		};


	}
}
